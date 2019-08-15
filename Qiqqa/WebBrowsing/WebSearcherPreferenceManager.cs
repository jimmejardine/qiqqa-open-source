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
            preferences.Add(WebSearchers.ARXIV_KEY);
            preferences.Add(WebSearchers.JSTOR_KEY);
            preferences.Add(WebSearchers.IEEEXPLORE_KEY);
            preferences.Add(WebSearchers.SCIVERSE_KEY);
            preferences.Add(WebSearchers.MSACADEMIC_KEY);
            preferences.Add(WebSearchers.WIKIPEDIA_KEY);
            //preferences.Add(WebSearchers.PUBMEDXML_KEY);
            preferences.Add(WebSearchers.GOOGLE_US_KEY);
            //preferences.Add(WebSearchers.GOOGLE_UK_KEY);

            return preferences;
        }

        public HashSet<string> LoadPreferences(HashSet<string> mandatory_web_searchers = null)
        {
            HashSet<string> preferences = new HashSet<string>();

            // mix in the required set:
            if (null != mandatory_web_searchers)
            {
                foreach (string line in mandatory_web_searchers)
                {
                    preferences.Add(line.ToUpper());
                }
            }

            // If they have a preferences file
            if (File.Exists(PREFERENCES_FILENAME))
            {
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
            Logging.Info("Returning default WebSearcher preferences because no preferences have been saved.");
            foreach (string line in GetDefaultPreferences())
            {
                preferences.Add(line.ToUpper());
            }
            return preferences;
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
