using System;
using System.Runtime.InteropServices;

namespace Utilities.PDF.Sorax
{
    class SoraxDLL
    {
        static SoraxDLL()
        {
            SPD_RegisterComp("JamesJardine", "DKqv7cv8d6UW49Gq");
        }

        [StructLayout(LayoutKind.Sequential)]
        public class Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public int X;
            public int Y;

            public override string ToString()
            {
                return String.Format("{0}x{1}", X, Y);
            }
        }

        [DllImport("SPdf.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SPD_ResetConfig([MarshalAs(UnmanagedType.LPStr)]string lpszFileName);

        [DllImport("SPdf.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void SPD_RegisterComp([MarshalAs(UnmanagedType.LPStr)]string lpszUserName, [MarshalAs(UnmanagedType.LPStr)]string lpszRegKey);

        [DllImport("SPdf.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SPD_Open([MarshalAs(UnmanagedType.LPStr)]string lpszFileName, [MarshalAs(UnmanagedType.LPStr)]string lpszUserPwd, [MarshalAs(UnmanagedType.LPStr)]string lpszOwnerPwd);

        [DllImport("SPdf.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SPD_Close(IntPtr hDoc);

        [DllImport("SPdf.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SPD_GetPageCount(IntPtr hDoc);

        [DllImport("SPdf.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SPD_GetPageBitmap(IntPtr hDoc, IntPtr hDC, int nPage, int nRot, float fDPI);

        [DllImport("SPdf.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SPD_Export(IntPtr hDoc, [MarshalAs(UnmanagedType.LPStr)]string lpszFileName, int nFromPage, int nToPage, int nExpType, IntPtr pCb, IntPtr pvParam);

        [DllImport("SPdf.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool SPD_GetPageSizeEx(IntPtr hDoc, int nPage, float fDPI, [MarshalAs(UnmanagedType.Struct)] ref Size lpSize);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr window_handle);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr window_handle);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr window_handle, IntPtr hdc);
        
        //[DllImport("gdi32.dll")]
        //public static extern IntPtr CreateCompatibleDC(IntPtr handle);

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteDC(IntPtr HDC_HDC);
    }
}
