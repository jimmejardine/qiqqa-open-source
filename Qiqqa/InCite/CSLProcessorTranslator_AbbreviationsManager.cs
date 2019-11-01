using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Qiqqa.Common.Configuration;
using Utilities;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.InCite
{
    internal class CSLProcessorTranslator_AbbreviationsManager
    {
        internal static Dictionary<string, string> GetAbbreviations()
        {
            Dictionary<string, string> abbreviations = new Dictionary<string, string>();

            if (ConfigurationManager.Instance.ConfigurationRecord.InCite_UseAbbreviations)
            {
                foreach (var pair in LoadDefaultAbbreviations())
                {
                    abbreviations[pair.Key] = pair.Value;
                }

                foreach (var pair in LoadCustomAbbreviations())
                {
                    abbreviations[pair.Key] = pair.Value;
                }
            }

            return abbreviations;
        }


        private static Dictionary<string, string> LoadCustomAbbreviations()
        {
            Logging.Info("+Loading custom abbreviations.");

            Dictionary<string, string> abbreviations = new Dictionary<string, string>();

            string filename = ConfigurationManager.Instance.ConfigurationRecord.InCite_CustomAbbreviationsFilename;
            if (!String.IsNullOrEmpty(filename))
            {
                try
                {
                    using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        LoadAbbreviationsFromStream(stream, abbreviations);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex, "There was a problem with the custom abbreviation file.");
                }
            }

            Logging.Info("-Loaded {0} custom abbreviations.", abbreviations.Count);

            return abbreviations;
        }

        internal static Dictionary<string, string> default_abbreviations = null;
        private static Dictionary<string, string> LoadDefaultAbbreviations()
        {
            if (null == default_abbreviations)
            {
                Logging.Info("+Loading default abbreviations.");

                Dictionary<string, string> abbreviations = new Dictionary<string, string>();

                string citation_resources_subdirectory = CSLProcessor.CITATION_RESOURCES_SUBDIRECTORY;
                string filename = Path.GetFullPath(Path.Combine(citation_resources_subdirectory, @"default_abbreviations.txt.gz"));
                using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (GZipStream compressed_stream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        LoadAbbreviationsFromStream(compressed_stream, abbreviations);
                    }
                }

                Logging.Info("-Loaded {0} default abbreviations.", abbreviations.Count);

                default_abbreviations = abbreviations;
            }

            return default_abbreviations;
        }

        private static void LoadAbbreviationsFromStream(Stream stream, Dictionary<string, string> abbreviations)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                while (true)
                {
                    string line = sr.ReadLine();
                    if (null == line) break;

                    string[] parts = line.Split('\t');
                    abbreviations[parts[0].ToLower()] = parts[1];
                }
            }
        }
    }
}
