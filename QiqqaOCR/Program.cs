using System;
using System.Text;
using System.Threading;
using Utilities;

namespace QiqqaOCR
{
    class Program
    {
        /// <summary>
        /// Arguments are:
        /// 1) mode
        /// 2) pdf filename
        /// 3) page number(s)
        /// 4) ocr result filename
        /// 5) NOKILL (optional)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static int Main(string[] args)
        {
            // This is used to return any errors to the OS
            int exit_code = 0;
            bool no_kill = (args.Length > 6 && 0 == args[6].CompareTo("NOKILL"));

            try
            {
                string on_your_conscience =
                    "Qiqqa is Copyright © Quantisle 2010-2016.  All rights reserved." +
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
                if (false) { }
                else if ("GROUP" == mode_switch)
                {
                    TextExtractEngine.MainEntry(args, no_kill);
                }
                else if ("SINGLE" == mode_switch)
                {
                    OCREngine.MainEntry(args, no_kill);
                }
                else
                {
                    throw new Exception("Unknown mode switch " + mode_switch);
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

                Logging.Error(ex, "There was an error in QiqqaOCR:\n{0}", sb.ToString());
                exit_code = -1;
            }

            // Check if we should exit
            if (no_kill)
            {
                Logging.Error("PAUSED");
                Console.ReadKey();
            }

            return exit_code;
        }
    }
}
