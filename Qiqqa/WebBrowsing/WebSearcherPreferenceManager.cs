using System.Collections.Generic;
using System.IO;
using Qiqqa.Common.Configuration;
using Utilities;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.WebBrowsing
{
    internal class WebSearcherPreferenceManager
    {
        public static readonly WebSearcherPreferenceManager Instance = new WebSearcherPreferenceManager();

        private WebSearcherPreferenceManager()
        {
        }

        private static string PREFERENCES_FILENAME => Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.BaseDirectoryForUser, @"Qiqqa.web_searcher_preferences"));

        private HashSet<string> GetDefaultPreferences()
        {
            HashSet<string> preferences = new HashSet<string>();

            preferences.Add(WebSearchers.SCHOLAR_KEY);
#if DEBUG
            // Are we looking at this dialog in the Visual Studio Designer?
            bool include_more = !Utilities.Misc.Runtime.IsRunningInVisualStudioDesigner;
#else
            const bool include_more = true;
#endif
            if (include_more)
            {
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
            }

            return preferences;
        }

        /// <summary>
        /// Load WebSearcher Preferences: the set of search engines (websites) which should be listed.
        /// 
        /// When any mandatory web searchers are specified through the <code>mandatory_web_searchers</code>
        /// argument, then these will be OR-combined with the set stored as preferences.
        /// </summary>
        /// <param name="mandatory_web_searchers">May be NULL or a set of required web searchers; these will be 
        /// added to the returned set, next to the preferences set.</param>
        /// <returns>A set of websearcher identifiers.</returns>
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


#if DEBUG
            // Are we looking at this dialog in the Visual Studio Designer?
            bool load_from_file = !Utilities.Misc.Runtime.IsRunningInVisualStudioDesigner;
#else
            const bool load_from_file = true;
#endif

            // If they have a preferences file
            if (load_from_file && File.Exists(PREFERENCES_FILENAME))
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
#if DEBUG
            // Are we looking at this dialog in the Visual Studio Designer?
            if (Utilities.Misc.Runtime.IsRunningInVisualStudioDesigner)
                return;
#endif

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
