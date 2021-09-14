using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Utilities;
using Utilities.Encryption;
using Utilities.OCR;
using Utilities.PDF.MuPDF;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaOCR
{
    internal static class TextExtractEngine
    {
        private static string pdf_filename;
        private static string page_numbers;
        private static string ocr_output_filename;
        private static string pdf_user_password;

        // Warning CA1823  It appears that field 'TextExtractEngine.language' is never used or is only ever assigned to. 
        // Use this field or remove it.	
        private static string language;
        private static Thread thread_text_extract = null;
        private static Dictionary<int, WordList> word_lists_text_extract = null;
        private static bool has_exited_text_extract = false;
        private static Exception exception_text_extract = null;
        private static object global_vars_access_lock = new object();

        public static bool DEBUG = false;

        internal static void MainEntry(string[] args, bool no_kill)
        {
            // Check that we were given the right number of parameters
            if (args.Length < 6)
            {
                throw new Exception("Not enough command line arguments");
            }

            // Get the parameters
            pdf_filename = args[1];
            page_numbers = args[2];
            ocr_output_filename = args[3];
            pdf_user_password = ReversibleEncryption.Instance.DecryptString(args[4]);
            language = args[5];

            // Check that the PDF exists
            if (!File.Exists(pdf_filename))
            {
                throw new Exception(String.Format("Input PDF '{0}' does not exist", pdf_filename));
            }

            // When should the various processes die?
            Stopwatch clk = Stopwatch.StartNew();

            while (true)
            {
                // --- TEST FOR STARTUP ------------------------------------------------------------------------------------------------------------------------------------------------

                // Do we need to start the word list extractor thread?
                bool must_start_thread;
                lock (global_vars_access_lock)
                {
                    must_start_thread = (null == thread_text_extract);
                }

                if (must_start_thread)
                {
                    Logging.Info("Starting the text extract thread");
                    thread_text_extract = new Thread(ThreadTextExtractMainEntry);
                    thread_text_extract.Name = "ThreadTextExtract";
                    thread_text_extract.IsBackground = true;
                    thread_text_extract.Priority = ThreadPriority.BelowNormal;
                    thread_text_extract.Start();
                }

                // --- TEST FOR COMPLETION ------------------------------------------------------------------------------------------------------------------------------------------------

                // Do we have some reasonable text_extract results?
                lock (global_vars_access_lock)
                {
                    if (null != word_lists_text_extract)
                    {
                        int pageCount = word_lists_text_extract.Count;
                        int[] wordCountPerPage = new int[Math.Max(1, pageCount)];

                        int pg = 0;
                        foreach(var el in word_lists_text_extract)
                        {
                            var wl = el.Value;
                            wordCountPerPage[pg++] = wl.Count;
                        }

                        Logging.Info("We have a text extract word list: page count: {0}, word count: {1}", pageCount, pageCount > 0 ? String.Join(",", wordCountPerPage) : "---");
                        break;
                    }
                }

                // --- TEST FOR PROBLEMS ------------------------------------------------------------------------------------------------------------------------------------------------

                // Have we been running for too long?
                if (clk.ElapsedMilliseconds >= Constants.MAX_WAIT_TIME_MS_FOR_QIQQA_OCR_TASK_TO_TERMINATE && !no_kill)
                {
                    Logging.Error("We have been running for too long, so exiting");
                    break;
                }

                lock (global_vars_access_lock)
                {
                    // Has the process had an exception?
                    if (null != exception_text_extract)
                    {
                        Logging.Error("Both text extract and OCR have had an exception, so exiting");
                        break;
                    }

                    // Has the process somehow without writing a result?
                    if (has_exited_text_extract)
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
                if (null != word_lists_text_extract && word_lists_text_extract.Count > 0)
                {
                    Logging.Info("+Writing OCR to file {0}", ocr_output_filename);
                    WordList.WriteToFile(ocr_output_filename, word_lists_text_extract, "PDFText");
                    // And *verify* the written OCR text format:
                    WordList.ReadFromFile(ocr_output_filename);
                    Logging.Info("-Writing OCR to file {0}", ocr_output_filename);
                }
                else
                {
                    throw new Exception("We have no wordlist to write!");
                }
            }

            // properly terminate/abort the thread:
            if (null != thread_text_extract)
            {
                if (!thread_text_extract.Join(500))
                {
                    thread_text_extract.Abort();
                    thread_text_extract.Join(100);
                }
            }

            // propagate any exception thrown by the worker process
            if (null != exception_text_extract)
            {
                throw new Exception("Failure", exception_text_extract);
            }
        }

        private static void ThreadTextExtractMainEntry(object arg)
        {
            try
            {
                Dictionary<int, WordList> word_lists = DoOCR(pdf_filename, page_numbers, pdf_user_password, DEBUG ? $"{ ocr_output_filename }.dbg" : null);
                bool word_lists_text_extract_credible = WordListCredibility.Instance.IsACredibleWordList(word_lists);
                if (word_lists_text_extract_credible)
                {
                    lock (global_vars_access_lock)
                    {
                        word_lists_text_extract = word_lists;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Problem while doing text extract for file {0}", pdf_filename);
                lock (global_vars_access_lock)
                {
                    exception_text_extract = ex;
                }
            }
            finally
            {
                lock (global_vars_access_lock)
                {
                    has_exited_text_extract = true;
                }
            }
        }

        public static Dictionary<int, WordList> DoOCR(string pdf_filename, string page_numbers, string pdf_user_password, string dbg_output_file_template = null)
        {
            List<MuPDFRenderer.TextChunk> text_chunks = MuPDFRenderer.GetEmbeddedText(pdf_filename, page_numbers, pdf_user_password, ProcessPriorityClass.BelowNormal, dbg_output_file_template);
            Dictionary<int, WordList> word_lists = ConvertToWordList(text_chunks, pdf_filename);
            return word_lists;
        }

        private static Dictionary<int, WordList> ConvertToWordList(List<MuPDFRenderer.TextChunk> text_chunks, string pdf_filename)
        {
            Dictionary<int, WordList> word_lists = new Dictionary<int, WordList>();
            int current_page = 0;
            WordList current_word_list = null;

            foreach (MuPDFRenderer.TextChunk text_chunk in text_chunks)
            {
                // Check if we have moved onto a new page
                if (null == current_word_list || text_chunk.page != current_page)
                {
                    current_page = text_chunk.page;
                    current_word_list = new WordList();
                    word_lists[current_page] = current_word_list;
                }

                // Create the new word
                Word word = new Word();
                word.Text = text_chunk.text;
                if (!String.IsNullOrEmpty(text_chunk.post_diagnostic))
                {
                    word.Text = word.Text.PadRight(32) + "\t\t\t" + text_chunk.post_diagnostic;
                }
                word.Confidence = 1.0;
                word.Left = text_chunk.x0;
                word.Top = text_chunk.y0;
                word.Width = text_chunk.x1 - text_chunk.x0;
                word.Height = text_chunk.y1 - text_chunk.y0;
                if (word.Width < MuPDFRenderer.MINIMUM_SANE_WORD_WIDTH || word.Height < MuPDFRenderer.MINIMUM_SANE_WORD_WIDTH)
                {
                    throw new Exception(String.Format("OCR file '{0}': format error: zero word width/height @PAGE {1}", pdf_filename, current_page));
                }

                // And add it to the word list
                current_word_list.Add(word);
            }

            return word_lists;
        }
    }
}
