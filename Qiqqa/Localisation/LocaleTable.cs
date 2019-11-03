using System;
using System.Collections.Generic;
using Utilities;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Localisation
{
    [Serializable]
    public class LocaleTable : Dictionary<string, string>
    {
        private const char LINE_SPLITTER = ':';
        private static readonly char[] LINE_SPLITTER_ARRAY = new char[] { LINE_SPLITTER };

        public static LocaleTable Load(string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    LocaleTable locale_table = new LocaleTable();

                    string[] lines = File.ReadAllLines(filename);
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(LINE_SPLITTER_ARRAY, 2);
                        if (2 == parts.Length)
                        {
                            parts[1] = parts[1].Replace("\\n", Environment.NewLine);
                            locale_table[parts[0]] = String.Format(parts[1]);
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(line))
                            {
                                Logging.Warn("Ignoring locale line: " + line);
                            }
                        }
                    }

                    return locale_table;
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error loading locale file " + filename);
            }

            return null;
        }

        public void Save(string filename)
        {
            List<string> lines = new List<string>();

            List<string> keys_sorted = new List<string>(Keys);
            keys_sorted.Sort();

            foreach (string key in keys_sorted)
            {
                lines.Add(key + LINE_SPLITTER + this[key]);
            }

            File.WriteAllLines(filename, lines.ToArray());
        }
    }
}
