#if false

using System;
using System.IO;
using System.Text;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Utilities.ConsoleRedirector
{
    public class ConsoleRedirector : TextWriter
    {
        private StringBuilder sb = new StringBuilder();
        private ConsoleForm cf = new ConsoleForm();

        public ConsoleRedirector()
        {
            cf.Show();
        }

        public override void Write(char value)
        {
            sb.Append(value);
            cf.setText(sb.ToString());
        }

        public override Encoding Encoding => Encoding.ASCII;

        public static void CaptureConsole()
        {
            ConsoleRedirector cr = new ConsoleRedirector();
            Console.SetOut(cr);
        }
    }
}

#endif
