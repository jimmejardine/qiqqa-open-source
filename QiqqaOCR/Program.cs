using System;
using System.Text;
using System.Threading;
using Utilities;

namespace QiqqaOCR
{
    internal class Program
    {
        /// <summary>
        /// Arguments are:
        /// 
        /// 1) mode: GROUP
        /// 2) pdf_filename
        /// 3) page number(s) - comma separated 
        /// 4) ocr_output_filename - where the extracted word list info is stored
        /// 5) pdf_user_password - encrypted
        /// 6) language - (unused)
        /// 
        /// or:
        /// 
        /// 1) mode: SINGLE
        /// 2) pdf_filename
        /// 3) page number - only one page per run 
        /// 4) ocr_output_filename - where the extracted word list info is stored
        /// 5) pdf_user_password - encrypted
        /// 6) language - default is 'eng'
        /// 
        /// or:
        /// 
        /// 1) mode: SINGLE-FAKE
        /// 2) pdf_filename
        /// 3) page number - only one page per run 
        /// 4) ocr_output_filename - where the extracted word list info is stored
        /// 
        /// 7) NOKILL (optional)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static int Main(string[] args)
        {
            // This is used to return any errors to the OS
            int exit_code = 0;
            bool no_kill = (args.Length > 6 && 0 == args[6].ToUpper().CompareTo("NOKILL"));

            try
            {
                string on_your_conscience =
                    "Qiqqa is Copyright © Quantisle 2010-2019.  All rights reserved." +
                    "If you are reading this in a dissasembler, you know you are doing evil and will probably always have to look over your shoulder..."
                    ;
                on_your_conscience = "Main";

                Thread.CurrentThread.Name = on_your_conscience;

                // Check that we were given the right number of parameters
                if (args.Length < 1)
                {
                    throw new Exception("Not enough command line arguments");
                }

                string mode_switch = args[0];
                switch (mode_switch)
                {
                    case "GROUP":
                        TextExtractEngine.MainEntry(args, no_kill);
                        break;

                    case "SINGLE":
                        OCREngine.MainEntry(args, no_kill);
                        break;

                    case "SINGLE-FAKE":
                        FakeEngine.MainEntry(args, no_kill);
                        break;

                    default:
                        throw new Exception("Unknown mode switch: " + mode_switch);
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("--- Parameters ---");
                foreach (string arg in args)
                {
                    sb.Append(arg);
                    sb.Append(" ");
                }
                sb.AppendLine();

                sb.AppendLine("--- Exception ---");
                sb.AppendLine(ex.ToString());

                Logging.Error("There was an error in QiqqaOCR:\n{0}", sb.ToString());
                exit_code = -1;
            }

            // Check if we should exit
            if (no_kill)
            {
                Logging.Error("PAUSED");
                Console.ReadKey();
            }

            // This must be the last line the application executes, EVAR!
            Logging.ShutDown();

            return exit_code;
        }
    }
}
