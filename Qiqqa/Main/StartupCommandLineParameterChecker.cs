using System;
using System.Windows;
using Qiqqa.Common;
using Utilities.GUI;

namespace Qiqqa.Main
{
    internal class StartupCommandLineParameterChecker
    {
        internal static void Check()
        {
            string[] command_line_args = Environment.GetCommandLineArgs();
            if (1 < command_line_args.Length)
            {
                string filename = command_line_args[1];

                WPFDoEvents.InvokeInUIThread(() =>
                {
                    MainWindowServiceDispatcher.Instance.ProcessCommandLineFile(filename);
                }
                );
            }
        }
    }
}
