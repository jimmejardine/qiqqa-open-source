using System;
using System.Collections.Generic;
using System.IO;

namespace Utilities.Files
{
	public class TextFile
	{
        public static void SaveTextFile(String filename, String lines)
        {
            SaveTextFile(filename, new String[] { lines });
        }

        public static void SaveTextFile(String filename, List<string> lines)
        {
            SaveTextFile(filename, lines.ToArray());
        }

        public static void SaveTextFile(String filename, String[] lines)
		{
			using (StreamWriter sr = new StreamWriter(filename, false))
            {
				foreach (String line in lines)
				{
					sr.WriteLine(line);
				}
            }
		}

		public static String[] LoadTextFile(String filename)
		{
			return LoadTextFile(filename, false, false);
		}

		public static String[] LoadTextFile(String filename, bool skip_comments, bool skip_empties)
		{
            List<string> result = new List<string>();

            using (StreamReader sr = FileOpener.openStreamReaderWithLocalCheck(filename))
            {
                string next_line = null;
                while (null != (next_line = sr.ReadLine()))
                {
                    // Shall we skip this line
                    bool skip = (skip_comments && isComment(next_line)) || (skip_empties && (0 == next_line.Length));

                    // Store only the non-skipped lines
                    if (!skip)
                    {
                        result.Add(next_line);
                    }
                }
            }
			
			return result.ToArray();
		}

		public static bool isComment(string source)
		{
			return (source.StartsWith("//") || source.StartsWith("#"));
		}
	}
}
