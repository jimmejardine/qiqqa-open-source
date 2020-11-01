using System;
using System.Collections.Generic;
using System.Text;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.AITagsStuff
{
    public class BlackWhiteListManager
    {
        private TypedWeakReference<WebLibraryDetail> web_library_detail;
        public WebLibraryDetail LibraryRef => web_library_detail?.TypedTarget;

        public BlackWhiteListManager(WebLibraryDetail web_library_detail)
        {
            this.web_library_detail = new TypedWeakReference<WebLibraryDetail>(web_library_detail);
        }

        public string Filename_Store => Path.GetFullPath(Path.Combine(LibraryRef.LIBRARY_BASE_PATH, @"Qiqqa.aitag_blackwhitelists"));

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
                if (!File.Exists(Filename_Store))
                {
                    Logging.Info("No file {0} found, so starting afresh.", Filename_Store);
                }
                else
                {
                    string[] lines = File.ReadAllLines(Filename_Store);
                    foreach (string line in lines)
                    {
                        if (String.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        // skip comments
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
                            Logging.Warn(ex2, "There was a problem processing AutoTags blacklist/whitelist line:\n\t{0}\n\tFile: {1}", line, Filename_Store);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error loading WhiteList file {0}.", Filename_Store);
            }

            return results;
        }
    }
}
