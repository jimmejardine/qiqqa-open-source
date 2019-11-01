using System;
using System.Runtime.InteropServices;

namespace Utilities.GUI
{
    public class Console
    {
        public static Console Instance = new Console();

        static Console()
        {
            AllocConsole();
        }

        private Console()
        {
        }

        ~Console()
        {
            FreeConsole();
        }

        [DllImport("kernel32.dll")]
        static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        static extern Boolean FreeConsole();
        [DllImport("kernel32.dll")]
        static extern Boolean SetConsoleOutputCP(uint codepage);


        public void Init()
        {
        }

        public void UseRussianCodePage()
        {
            SetConsoleOutputCP(65001);
        }
    }
}
