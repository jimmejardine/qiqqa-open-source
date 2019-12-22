using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Utilities.Files
{
    public class FileTools
    {
        public static void Delete(string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There was a problem deleting file {0}", filename);
            }
        }

        // https://stackoverflow.com/questions/17612/how-do-you-place-a-file-in-recycle-bin-instead-of-delete
        private static class Recycle
        {
            private const int FO_DELETE = 3;
            private const int FOF_ALLOWUNDO = 0x40;
            private const int FOF_NOCONFIRMATION = 0x0010;

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
            public struct SHFILEOPSTRUCT
            {
                public IntPtr hwnd;
                [MarshalAs(UnmanagedType.U4)]
                public int wFunc;
                public string pFrom;
                public string pTo;
                public short fFlags;
                [MarshalAs(UnmanagedType.Bool)]
                public bool fAnyOperationsAborted;
                public IntPtr hNameMappings;
                public string lpszProgressTitle;
            }

            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

            public static void DeleteFileOperation(string filePath)
            {
                SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT();
                fileop.wFunc = FO_DELETE;
                fileop.pFrom = filePath + '\0' + '\0';
                fileop.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;

                SHFileOperation(ref fileop);
            }
        }

        // Delete file to RecycleBin, i.e. ensure file is recoverable after delete, if user desires such.
        public static void DeleteToRecycleBin(string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    Recycle.DeleteFileOperation(filename);
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "There was a problem deleting file {0} to the Recycle Bin.", filename);
            }
        }

        public static bool MoveSafelyWithoutOverwriting(string source, string target)
        {
            if (File.Exists(source))
            {
                if (!File.Exists(target))
                {
                    File.Move(source, target);
                    return true;

                }
                else
                {
                    File.Delete(source);
                    return false;
                }
            }

            return false;
        }

        public static bool MoveSafelyWithOverwriting(string source, string target)
        {
            if (File.Exists(source))
            {
                if (File.Exists(target))
                {
                    File.Delete(target);
                }

                File.Move(source, target);
                return true;
            }

            return false;
        }

        public static bool isValidFilenameCharacter(char c)
        {
            if (Char.IsLetterOrDigit(c)) return true;
            if ('-' == c) return true;
            if (' ' == c) return true;
            if ('.' == c) return true;
            if (',' == c) return true;
            if ('(' == c) return true;
            if (')' == c) return true;
            if ('_' == c) return true;
            if ('\'' == c) return true;

            return false;
        }


        public static string MakeSafeFilename(string source)
        {
            string target = "";
            for (int i = 0; i < source.Length; ++i)
            {
                char c = source[i];
                if (isValidFilenameCharacter(c))
                {
                    target = target + c;
                }
                else
                {
                    target = target + '-';
                }
            }

            if (target.Length > 128)
            {
                // TODO: make name unique by including a bit of filename hash, for example
                target = target.Substring(0, 128);
            }

            return target;
        }

        public static void BrowseToFileInExplorer(string filename)
        {
            string argument = String.Format("/select,{0}", filename);
            Process.Start("explorer.exe", argument);
        }
    }
}
