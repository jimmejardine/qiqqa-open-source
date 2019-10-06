using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using tessnet2;
using Utilities;
using Utilities.Encryption;
using Utilities.OCR;
using Utilities.PDF.Sorax;
using Word = tessnet2.Word;

namespace QiqqaOCR
{
    class OCREngine
    {
        static string pdf_filename;
        static int page_number;
        static string ocr_output_filename;
        static string pdf_user_password;
        static string language;

        static Thread thread_ocr = null;

        static WordList word_list_ocr = null;
        static bool has_exited_ocr = false;
        static Exception exception_ocr = null;

        static object global_vars_access_lock = new object();

        // Warning CA1812	'OCREngine' is an internal class that is apparently never instantiated.
        // If this class is intended to contain only static methods, consider adding a private constructor 
        // to prevent the compiler from generating a default constructor.
        private OCREngine()
        { }

        internal static void MainEntry(string[] args, bool no_kill)
        {
            // Check that we were given the right number of parameters
            if (args.Length < 6)
            {
                throw new Exception("Not enough command line arguments");
            }

            // Get the parameters
            pdf_filename = args[1];
            page_number = Convert.ToInt32(args[2]);
            ocr_output_filename = args[3];
            pdf_user_password = ReversibleEncryption.Instance.DecryptString(args[4]);
            language = args[5];

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
            DateTime start_time_app = DateTime.UtcNow;
            DateTime kill_time = start_time_app.AddSeconds(180);

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
                    thread_ocr.Start();
                }

                // --- TEST FOR COMPLETION ------------------------------------------------------------------------------------------------------------------------------------------------

                // Do we have any OCR results
                lock (global_vars_access_lock)
                {
                    if (null != word_list_ocr)
                    {
                        Logging.Info("We have an OCR word list of length {0}", word_list_ocr.Count);
                        break;
                    }
                }

                // --- TEST FOR PROBLEMS ------------------------------------------------------------------------------------------------------------------------------------------------

                // Have we been running for too long?
                if (DateTime.UtcNow > kill_time && !no_kill)
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

            // Check that we have something to write
            lock (global_vars_access_lock)
            {
                if (null != word_list_ocr)
                {
                    Logging.Info("+Writing OCR to file {0}", ocr_output_filename);
                    Dictionary<int, WordList> word_lists = new Dictionary<int, WordList>();
                    word_lists[page_number] = word_list_ocr;
                    WordList.WriteToFile(ocr_output_filename, word_lists, "OCR");
                    Logging.Info("-Writing OCR to file {0}", ocr_output_filename);
                }
                else
                {
                    Logging.Info("+Writing empty OCR to file {0}", ocr_output_filename);
                    Dictionary<int, WordList> word_lists = new Dictionary<int, WordList>();
                    word_lists[page_number] = new WordList();
                    WordList.WriteToFile(ocr_output_filename, word_lists, "OCR-Failed");
                    Logging.Info("-Writing empty OCR to file {0}", ocr_output_filename);
                }
            }
        }


        static void ThreadOCRMainEntry(object arg)
        {
            string fname = "???"; 
            int pgnum = 0;

            try
            {
                lock (global_vars_access_lock)
                {
                    fname = pdf_filename;
                    pgnum = page_number;
                }

                WordList word_list = DoOCR(fname, pgnum);

                lock (global_vars_access_lock)
                {
                    word_list_ocr = word_list;
                }
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
            SoraxPDFRenderer renderer = new SoraxPDFRenderer(pdf_filename, pdf_user_password, pdf_user_password);
            using (MemoryStream ms = new MemoryStream(renderer.GetPageByDPIAsImage(page_number, 200)))
            { 
                Bitmap bitmap = (Bitmap)Image.FromStream(ms);
				
		        Logging.Info("-Rendering page #{0}", page_number);

		        Logging.Info("Startup directory is {0}", Environment.CurrentDirectory);
		        Logging.Info("Language is '{0}'", language);

		        using (Tesseract ocr = new Tesseract())
				{
		            ocr.Init(null, language, false);

		            Logging.Info("+Doing OCR");

                    const int MIN_WIDTH = 0;

                    // Build a list of all the rectangles to process
                    PDFRegionLocator pdf_region_locator = new PDFRegionLocator(bitmap);
		            PDFRegionLocator.Region last_region = pdf_region_locator.regions[0];
		            List<Rectangle> rectangles = new List<Rectangle>();
                    Rectangle last_rectangle = new Rectangle();
                    foreach (PDFRegionLocator.Region region in pdf_region_locator.regions)
		            {
                        int rect_height = region.y - last_region.y;
                        bool alarming_height = (rect_height <= 0);

                        Rectangle rectangle = new Rectangle();

                        if (last_region.state == PDFRegionLocator.SegmentState.BLANKS)
		                {
		                    // LHS
		                    {
		                        rectangle = new Rectangle(0, last_region.y, bitmap.Width / 2, Math.Max(MIN_WIDTH, rect_height));
		                    }
		                    // RHS
		                    {
		                        rectangle = new Rectangle(bitmap.Width / 2, last_region.y, bitmap.Width / 2, Math.Max(MIN_WIDTH, rect_height));
		                    }
		                }
		                else if (last_region.state == PDFRegionLocator.SegmentState.PIXELS)
		                {
		                    // Full column
		                    {
		                        rectangle = new Rectangle(0, last_region.y, bitmap.Width, Math.Max(MIN_WIDTH, rect_height));
		                    }
		                }

                        if (alarming_height || rectangle.Height <= 0)
                        {
                            Logging.Warn("Calculated region height is negative or zero: {0} :: Calculated region {1} <-- CURRENT:{2} - LAST:{3}", rect_height, rectangle, region, last_region);

                            // skip rectangle
                        }
                        else if (last_rectangle.X == rectangle.X && last_rectangle.Y == rectangle.Y)
                        {
                            Logging.Warn("Overlapping subsequent rectangles will be merged :: CURRENT:{0} - LAST:{1}", rectangle, last_rectangle);
                            last_rectangle.Width = Math.Max(last_rectangle.Width, rectangle.Width);
                            last_rectangle.Height = Math.Max(last_rectangle.Height, rectangle.Height);
                            Logging.Warn("--> Updated 'last' rectangle:{0}", last_rectangle);
                        }
                        else
                        { 
                            rectangles.Add(rectangle);
                            last_rectangle = rectangle;
                        }

                        last_region = region;
		            }

                    // DEBUG CODE: Draw in the region rectangles
#if DEBUG_OCR
                    {
                        string bitmap_diag_path = pdf_filename + "." + page_number + @"-ocr.png";

                        Logging.Debug特("Dumping page {0} PNG image to file {1}", page_number, bitmap_diag_path);
                        Graphics g = Graphics.FromImage(bitmap);
		                foreach (Rectangle rectangle in rectangles)
		                {
                            if (rectangle.Width <= MIN_WIDTH && rectangle.Height > MIN_WIDTH)
                            {
                                g.DrawRectangle(Pens.Purple, rectangle);
                            }
                            else if (rectangle.Width > MIN_WIDTH && rectangle.Height <= MIN_WIDTH)
                            {
                                g.DrawRectangle(Pens.PowderBlue, rectangle);
                            }
                            else if (rectangle.Width <= MIN_WIDTH && rectangle.Height <= MIN_WIDTH)
                            {
                                g.DrawRectangle(Pens.Red, rectangle);
                            }
                            else
                            {
                                g.DrawRectangle(Pens.LawnGreen, rectangle);
                            }
		                }

		                bitmap.Save(bitmap_diag_path, ImageFormat.Png);
		            }
#endif

                    // Do the OCR on each of the rectangles
                    WordList word_list = new WordList();
		            foreach (Rectangle rectangle in rectangles)
		            {
		                if (0 == rectangle.Width || 0 == rectangle.Height)
		                {
		                    Logging.Info("Skipping zero extent rectangle {0}", rectangle.ToString());
		                    continue;
		                }

		                Logging.Info("Doing OCR for region {0} on bitmap WxH: {1}x{2}", rectangle.ToString(), bitmap.Width, bitmap.Height);
		                List<Word> result = ocr.DoOCR(bitmap, rectangle);
		                Logging.Info("Got {0} words", result.Count);
		                word_list.AddRange(ConvertToWordList(result, rectangle, bitmap));
		            }

		            Logging.Info("-Doing OCR");


		            Logging.Info("Found {0} words ({1} @ #{2})", word_list.Count, pdf_filename, page_number);

#if false
                    Logging.Info("+Reordering words for columns");
                    WordList word_list_ordered = ColumnWordOrderer.ReorderWords(word_list);
		            Logging.Info("-Reordering words for columns");
		            word_list_ordered.WriteToFile(ocr_output_filename);
#endif

		            return word_list;
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
