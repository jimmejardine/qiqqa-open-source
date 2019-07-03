using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Utilities.ProcessTools
{
    /// <summary>
    /// Allows only one process of the current processes name to be running.
    /// </summary>
    public class ProcessSingleton
    {
        [DllImport("User32.dll")]
        public static extern int ShowWindowAsync(IntPtr hWnd, ShowWindowConstants swCommand);
        public enum ShowWindowConstants
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        };

        public static bool IsProcessUnique(bool bring_other_process_to_front_if_it_exists)
        {
            Process this_qiqqa_process = Process.GetCurrentProcess();
            foreach (Process qiqqa_process in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
            {
                if (qiqqa_process.Id != this_qiqqa_process.Id && qiqqa_process.SessionId == this_qiqqa_process.SessionId)
                {
                    if (bring_other_process_to_front_if_it_exists)
                    {
                        ShowWindowAsync(qiqqa_process.MainWindowHandle, ShowWindowConstants.SW_SHOWMINIMIZED);
                        ShowWindowAsync(qiqqa_process.MainWindowHandle, ShowWindowConstants.SW_RESTORE);
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
