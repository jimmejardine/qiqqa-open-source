using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Utilities.GUI
{
    public class WindowTools
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg,
                int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public static void CauseWindowToBeDragged(Window window)
        {
            SendMessage(new WindowInteropHelper(window).Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
    }
}
