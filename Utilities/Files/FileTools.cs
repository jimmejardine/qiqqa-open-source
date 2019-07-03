using System;
using System.Diagnostics;
using System.IO;

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

        public static String getFilenameFromFullPathname(String longname)
		{
			int last_separator_pos = longname.LastIndexOf('\\');
			if (-1 == last_separator_pos) return longname;
			if (last_separator_pos+1 < longname.Length)
			{
				return longname.Substring(last_separator_pos+1);
			}
			else
			{
				return "";
			}
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
