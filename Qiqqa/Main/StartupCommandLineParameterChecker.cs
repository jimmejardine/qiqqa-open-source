using System;
using System.Windows;
using Qiqqa.Common;

namespace Qiqqa.Main
{
    class StartupCommandLineParameterChecker
    {
        internal static void Check()
        {
            string[] command_line_args = Environment.GetCommandLineArgs();
            if (1 < command_line_args.Length)
            {
                string filename = command_line_args[1];

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    MainWindowServiceDispatcher.Instance.ProcessCommandLineFile(filename);
                }
                ));
            }
        }
    }
}
