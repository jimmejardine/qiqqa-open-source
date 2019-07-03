using System.Collections.Generic;
using System.IO;
using Qiqqa.Common.Configuration;
using Utilities;

namespace Qiqqa.WebBrowsing
{
    class WebSearcherPreferenceManager
    {
        public static readonly WebSearcherPreferenceManager Instance = new WebSearcherPreferenceManager();

        private WebSearcherPreferenceManager()
        {
        }

        private static string PREFERENCES_FILENAME
        {
            get
            {
                return ConfigurationManager.Instance.BaseDirectoryForUser + "\\" + "Qiqqa.web_searcher_preferences";
            }
        }

        private HashSet<string> GetDefaultPreferences()
        {
            HashSet<string> preferences = new HashSet<string>();

            preferences.Add(WebSearchers.SCHOLAR_KEY);
            preferences.Add(WebSearchers.PUBMED_KEY);
            preferences.Add("ARXIV");
            preferences.Add("JSTOR");
            preferences.Add("IEEEXPLORE");
            preferences.Add("SCIVERSE");
            preferences.Add("MSACADEMIC");
            preferences.Add("WIKIPEDIA");

            return preferences;
        }

        public HashSet<string> LoadPreferences()
        {
            // If they have a preferences file
            if (File.Exists(PREFERENCES_FILENAME))
            {
                HashSet<string> preferences = new HashSet<string>();
                using (StreamReader sr = new StreamReader(PREFERENCES_FILENAME))
                {
                    while (true)
                    {
                        string line = sr.ReadLine();
                        if (null == line)
                        {
                            break;
                        }

                        preferences.Add(line.ToUpper());
                    }
                }

                // We always want Google Scholar!!
                preferences.Add(WebSearchers.SCHOLAR_KEY);

                return preferences;
            }


            // If they have no preferences file, then return the default
            {
                Logging.Info("Returning default WebSearcher preferences because no preferences have been saved.");
                return GetDefaultPreferences();
            }
        }

        public void SavePreferences(HashSet<string> preferences)
        {
            using (StreamWriter sw = new StreamWriter(PREFERENCES_FILENAME))
            {
                foreach (string preference in preferences)
                {
                    sw.WriteLine(preference.ToUpper());
                }
            }
        }
    }
}
