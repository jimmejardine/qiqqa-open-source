using System;
using System.Text;
using System.Threading;
using Utilities;
using Utilities.PDF.MuPDF;
using Utilities.Shutdownable;

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
            bool debug = (args.Length > 6 && 0 == args[6].ToUpper().CompareTo("DEBUG"));
            Logging.Info("\n\n============================================== Starting QiqqaOCR ===============================================\n");

            // This must be the last line the application executes, EVAR!
            Logging.ShutDown();

            return exit_code;
        }
    }
}
