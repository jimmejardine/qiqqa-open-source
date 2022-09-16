using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using tessnet2;
using Utilities;
using Utilities.Encryption;
using Utilities.OCR;
#if !HAS_MUPDF_PAGE_RENDERER
using Utilities.PDF.Sorax;
#else
using Utilities.PDF.MuPDF;
#endif
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using Word = tessnet2.Word;


namespace QiqqaOCR
{
    internal static class OCREngine
    {
        private static string pdf_filename;
        private static List<int> page_numbers;
        private static string ocr_output_filename;
        private static string pdf_user_password;
        private static string language;
        private static bool no_kill = false;
        private static Thread thread_ocr = null;
        private static bool has_exited_ocr = false;
        private static Exception exception_ocr = null;
        private static object global_vars_access_lock = new object();

        internal static void MainEntry(string[] args, bool _no_kill)
        {
            // Check that we were given the right number of parameters
            if (args.Length < 6)
            {
                throw new Exception("Not enough command line arguments");
            }

            // Get the parameters
            pdf_filename = args[1];

            string pages = args[2];
            List<string> pns = new List<string>(pages.Split(','));
            HashSet<int> pnx = new HashSet<int>();
            foreach (var pn in pns)
            {
                List<string> prng = new List<string>(pn.Split('-'));

                if (prng.Count > 1)
                {
                    int start = Convert.ToInt32(prng[0]);
                    int finish = Convert.ToInt32(prng[1]);

                    for (var i = start; i <= finish; i++)
                    {
                        pnx.Add(i);
                    }
                }
                else
                {
                    pnx.Add(Convert.ToInt32(pn));
                }
            }
            page_numbers = new List<int>(pnx);
            page_numbers.Sort();

            ocr_output_filename = args[3];
            pdf_user_password = ReversibleEncryption.Instance.DecryptString(args[4]);
            language = args[5];
            no_kill = _no_kill;

            // sanity check the arguments:
            if (!no_kill && page_numbers.Count != 1)
            {
                throw new Exception($"OCR engine 'page number' parameter only accepts a *single* page number in production. Erroneous parameter value: {pages}");
            }

            // Check that the PDF exists
            if (!File.Exists(pdf_filename))
            {
                throw new Exception(String.Format("Input PDF '{0}' does not exist", pdf_filename));
            }

            // Check that we have a language
            if (String.IsNullOrEmpty(language))
            {
                Logging.Info("Defaulting to language eng");
                language = "eng";
            }

            // When should the various processes die?
            Stopwatch clk = Stopwatch.StartNew();

            while (true)
            {
                // --- TEST FOR STARTUP ------------------------------------------------------------------------------------------------------------------------------------------------

                // Do we need to start the OCR extractor thread?
                bool must_start_thread;
                lock (global_vars_access_lock)
                {
                    must_start_thread = (null == thread_ocr);
                }

                if (must_start_thread)
                {
                    Logging.Info("Starting the OCR thread");
                    thread_ocr = new Thread(ThreadOCRMainEntry);
                    thread_ocr.Name = "ThreadOCR";
                    thread_ocr.IsBackground = true;
                    thread_ocr.Priority = ThreadPriority.BelowNormal;
                    thread_ocr.Start();
                }

                // --- TEST FOR COMPLETION ------------------------------------------------------------------------------------------------------------------------------------------------

                // Do we have any OCR results
                lock (global_vars_access_lock)
                {
                    if (has_exited_ocr)
                    {
                        break;
                    }
                }

                // --- TEST FOR PROBLEMS ------------------------------------------------------------------------------------------------------------------------------------------------

                // Have we been running for too long?
                if (clk.ElapsedMilliseconds > Constants.MAX_WAIT_TIME_MS_FOR_QIQQA_OCR_TASK_TO_TERMINATE && !no_kill)
                {
                    Logging.Error("We have been running for too long, so exiting");
                    break;
                }

                lock (global_vars_access_lock)
                {
                    // Has the process had an exception?
                    if (null != exception_ocr)
                    {
                        Logging.Info("Both text extract and OCR have had an exception, so exiting");
                        break;
                    }

                    // Has the process somehow finished without writing a result?
                    if (has_exited_ocr)
                    {
                        Logging.Info("Both text extract and OCR have exited, so exiting");
                        break;
                    }
                }

                // Do some sleeping before iterating
                Thread.Sleep(250);
            }

            // properly terminate/abort the thread:
            if (null != thread_ocr)
            {
                if (!thread_ocr.Join(500))
                {
                    thread_ocr.Abort();
                    thread_ocr.Join(100);
                }
            }

            // propagate any exception thrown by the worker process
            if (null != exception_ocr)
            {
                throw new Exception("Failure", exception_ocr);
            }
        }

        private static void ThreadOCRMainEntry(object arg)
        {
            string fname = "???";
            List<int> pgnums;
            int pgnum = 0;

            try
            {
                lock (global_vars_access_lock)
                {
                    fname = pdf_filename;
                    pgnums = page_numbers;
                }

                Dictionary<int, WordList> word_lists = new Dictionary<int, WordList>();
                Dictionary<int, bool> page_ocr_successes = new Dictionary<int, bool>();

                foreach (var p in pgnums)
                {
                    pgnum = p;

                    WordList word_list = DoOCR(fname, pgnum);

                    Logging.Info("We have an OCR word list of length {0} for page {1}", word_list?.Count, pgnum);

                    // Check that we have something to write
                    if (null != word_list && word_list.Count > 0)
                    {
                        word_lists[pgnum] = word_list;
                        page_ocr_successes[pgnum] = true;
                    }
                    else
                    {
                        // FAKE a word list to shut up Qiqqa for the time being!
                        word_lists[pgnum] = FakeEngine.ConvertToWordList(); // new WordList();
                        page_ocr_successes[pgnum] = false;
                    }
                }

                // Check that we have something to write
                Logging.Info("Writing OCR to file {0}", ocr_output_filename);
                WordList.WriteToFile(ocr_output_filename, word_lists, page_ocr_successes[pgnums[0]] ? "OCR" : "OCR-Failed");
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Problem while doing OCR for file {0} @ page {1}", fname, pgnum);

                lock (global_vars_access_lock)
                {
                    exception_ocr = ex;
                }
            }
            finally
            {
                lock (global_vars_access_lock)
                {
                    has_exited_ocr = true;
                }
            }
        }

        public static WordList DoOCR(string pdf_filename, int page_number)
        {
            Logging.Info("+Rendering page {1} for PDF file {0}", pdf_filename, page_number);
            return new WordList();
        }

        private static void DrawRectangleOutline(Graphics g, Pen baseColorPen, Rectangle rect)
        {
            const int ALPHA = (int)(0.5 * 256);
            const int ALPHA_FILL = (int)(0.1 * 256);

            Color baseColor = baseColorPen.Color;

            Color c = Color.FromArgb(ALPHA, baseColor.R, baseColor.G, baseColor.B);
            using (Pen pen = new Pen(c, 2))
            {
                Color fill_c = Color.FromArgb(ALPHA_FILL, baseColor.R, baseColor.G, baseColor.B);
                using (SolidBrush br = new SolidBrush(fill_c))
                {
                    // create a new rectangle that's an *outline* of the given rectangle; reckon with the pen width too!
                    Rectangle rc = new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Height + 2);
                    g.FillRectangle(br, rc);
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        private static WordList ConvertToWordList(List<Word> results, Rectangle rectangle, Bitmap bitmap)
        {
            WordList word_list = new WordList();
            foreach (Word tword in results)
            {
                Utilities.OCR.Word word = new Utilities.OCR.Word();
                word.Text = tword.Text;
                word.Confidence = tword.Confidence;
                word.Left = (tword.Left + rectangle.Left) / (double)bitmap.Width;
                word.Top = (tword.Top + rectangle.Top) / (double)bitmap.Height;
                word.Width = (tword.Right - tword.Left + 1) / (double)bitmap.Width;
                word.Height = (tword.Bottom - tword.Top + 1) / (double)bitmap.Height;
                word_list.Add(word);
            }

            return word_list;
        }
    }
}
