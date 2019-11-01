using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Utilities.GUI
{
    public class ScreenTools
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }



        [DllImport("user32", ExactSpelling = true, SetLastError = true)]
        internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref Point pt, [MarshalAs(UnmanagedType.U4)] int cPoints);




        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);


        public static Rect GetWindowPoint(IntPtr windowPtr)
        {
            RECT rct = new RECT();

            GetWindowRect(windowPtr, ref rct);

            return new Rect(rct.Left, rct.Top, (int)rct.Right - rct.Left, rct.Bottom - rct.Top);

            /*

            //http://stackoverflow.com/questions/18034975/how-do-i-find-position-of-a-win32-control-window-relative-to-its-parent-window

            var pt = new Point();

            MapWindowPoints( windowPtr, IntPtr.Zero , ref pt, 2);

            System.Diagnostics.Debug.WriteLine(pt.Y);
            //return new Point2D(pt.X, pt.Y);

            */
        }
    }
}
