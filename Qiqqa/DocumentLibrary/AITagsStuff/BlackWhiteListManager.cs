using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utilities;

namespace Qiqqa.DocumentLibrary.AITagsStuff
{
    public class BlackWhiteListManager
    {
        Library library;

        public BlackWhiteListManager(Library library)
        {
            this.library = library;
        }

        public string Filename_Store
        {
            get
            {
                return library.LIBRARY_BASE_PATH + "Qiqqa.aitag_blackwhitelists";
            }
        }

        public void WriteList(List<BlackWhiteListEntry> list)
        {
            StringBuilder sw = new StringBuilder();
            sw.AppendLine("# Qiqqa AutoTag black/whitelist");
            sw.AppendLine("# Version: 1");
            sw.AppendLine("#");
            sw.AppendLine("# Format is: <word>|<list_type>|<deleted>");
            sw.AppendLine("#");
            foreach (var entry in list)
            {
                string line = entry.ToFileString();
                sw.AppendLine(line);
            }

            File.WriteAllText(Filename_Store, sw.ToString());
        }

        public List<BlackWhiteListEntry> ReadList()
        {
            List<BlackWhiteListEntry> results = new List<BlackWhiteListEntry>();

            try
            {
                string[] lines = File.ReadAllLines(Filename_Store);
                foreach (string line in lines)
                {
                    if (String.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    if (line.StartsWith("#"))
                    {
                        continue;
                    }

                    // Process the line
                    try
                    {
                        BlackWhiteListEntry entry = new BlackWhiteListEntry(line);
                        results.Add(entry);
                    }
                    catch (Exception ex2)
                    {
                        Logging.Warn(ex2, "There was a problem processing aitag blacklist/whitelist line: {0}", line);
                    }
                }
            }
            catch (Exception)
            {
                Logging.Info("No file {0} found, so starting afresh.", Filename_Store);
            }

            return results;
        }
    }
}
