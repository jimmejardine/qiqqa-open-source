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
using Utilities.PDF.Sorax;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace QiqqaOCR
{
    internal static class FakeEngine
    {
        private static string pdf_filename;
        private static List<int> page_numbers;
        private static string ocr_output_filename;
        private static bool no_kill = false;

        internal static void MainEntry(string[] args, bool _no_kill)
        {
            // Check that we were given the right number of parameters
            if (args.Length < 4)
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

            // When should the various processes die?
            Stopwatch clk = Stopwatch.StartNew();

            string fname = "???";
            List<int> pgnums;
            int pgnum = 0;

            try
            {
                fname = pdf_filename;
                pgnums = page_numbers;

                Dictionary<int, WordList> word_lists = new Dictionary<int, WordList>();
                Dictionary<int, bool> page_ocr_successes = new Dictionary<int, bool>();

                foreach (var p in pgnums)
                {
                    pgnum = p;

                    WordList word_list = DoOCR(fname, pgnum);

                    Logging.Info("We have an OCR word list of length {0} for page {1}", word_list?.Count, pgnum);

                    word_lists[pgnum] = word_list;
                    page_ocr_successes[pgnum] = true;
                }

                // Check that we have something to write
                Logging.Info("Writing OCR to file {0}", ocr_output_filename);
                WordList.WriteToFile(ocr_output_filename, word_lists, "OCR-Faked");
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Problem while doing OCR for file {0} @ page {1}", fname, pgnum);

                throw;
            }
        }

        public static WordList DoOCR(string pdf_filename, int page_number)
        {
            Logging.Info("+Rendering page {1} for PDF file {0}", pdf_filename, page_number);

            // Do the OCR on each of the rectangles
            WordList word_list = new WordList();

            word_list.AddRange(ConvertToWordList());

            Logging.Info("-Doing OCR");
            
            Logging.Info("Faked {0} words ({1} @ #{2})", word_list.Count, pdf_filename, page_number);

#if false
            Logging.Info("+Reordering words for columns");
            WordList word_list_ordered = ColumnWordOrderer.ReorderWords(word_list);
		    Logging.Info("-Reordering words for columns");
		    word_list_ordered.WriteToFile(ocr_output_filename);
#endif

            return word_list;
        }

        public static WordList ConvertToWordList()
        {
            WordList word_list = new WordList();

            for (int i = 0; i < 3; i++)
            {
                Utilities.OCR.Word word = new Utilities.OCR.Word();
                word.Text = String.Format("*QiqqaOCRFailedFakedWord.{0}*", i);   // no real word will have stars and/or dots in it, so this can be safely used as a nonsense 'word' for failed OCR processes.
                word.Confidence = 0.985;
                word.Left = 0.1;
                word.Top = 0.1;
                word.Width = 0.8;
                word.Height = 0.8;
                word_list.Add(word);
            }

            return word_list;
        }
    }
}
