using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using System.Diagnostics;

namespace QiqqaLegacyFileFormats          // namespace Qiqqa.Common.Configuration
{

#if SAMPLE_LOAD_CODE

    public class ConfigurationManager
    {
        public static readonly ConfigurationManager Instance = new ConfigurationManager();
        private string user_guid;
        private bool is_guest;

        public string StartupDirectoryForQiqqa => null; //  UnitTestDetector.StartupDirectoryForQiqqa;

        private string base_directory_for_qiqqa = null;
        public string BaseDirectoryForQiqqa
        {
            get
            {
                //...

                    // If we get here, use the default path
                    if (null == base_directory_for_qiqqa)
                    {
                        base_directory_for_qiqqa = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Quantisle/Qiqqa"));
                    }

                return base_directory_for_qiqqa;
            }
        }

        public string BaseDirectoryForUser => Path.GetFullPath(Path.Combine(BaseDirectoryForQiqqa, user_guid));

        private string ConfigFilenameForUser => Path.Combine(BaseDirectoryForUser, @"Qiqqa.configuration");

        private string SearchHistoryFilename => Path.Combine(BaseDirectoryForUser, @"Qiqqa.search_history");

        public string Program7ZIP => Path.Combine(StartupDirectoryForQiqqa, @"7za.exe");

        public string ProgramHTMLToPDF => Path.Combine(StartupDirectoryForQiqqa, @"wkhtmltopdf.exe");

        private ConfigurationRecord configuration_record;

        private ConfigurationManager()
        {
            ResetConfigurationRecordToGuest();
        }

        private Stopwatch lastSaveTimestamp = null;

        public void SaveConfigurationRecord(bool force_save = false)
        {
            if (lastSaveTimestamp == null)
            {
                lastSaveTimestamp = Stopwatch.StartNew();
                force_save = true;
            }
            else
            {
                long elapsed = lastSaveTimestamp.ElapsedMilliseconds;
                lastSaveTimestamp.Restart();
                if (!force_save)
                {
                    // only update the stored config file every minute or so, no matter how many changes are made.
                    force_save = (elapsed >= 60 * 1000);
                }
            }
            // if the flag hasn't been set by now, we're looking at a too frequent request to save the qiqqa config file.
            if (!force_save)
            {
                return;
            }

            try
            {
                Logging.Info("Saving configuration");
                ObjectSerializer.SaveObject(ConfigFilenameForUser, configuration_record);
                Logging.Info("Saved configuration");
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem saving the configuration.");
            }
        }

#region --- Search history ----------------------------------------------------------------------------------------

        private HashSet<string> search_history = new HashSet<string>();
        private HashSet<string> search_history_from_disk = null;
        public HashSet<string> SearchHistory
        {
            get
            {
                // Add any user specified searches
                var user_specified_searches = ConfigurationManager.Instance.ConfigurationRecord.GUI_UserDefinedSearchStrings;
                if (!String.IsNullOrEmpty(user_specified_searches))
                {
                    foreach (string search in StringTools.splitAtNewline(user_specified_searches))
                    {
                        search_history.Add(search);
                    }
                }

                if (null == search_history_from_disk)
                {
                    search_history_from_disk = new HashSet<string>();
                    // Try to load it from disk (if we are premium or premium+)
                    try
                    {
                        if (File.Exists(SearchHistoryFilename))
                        {
                            foreach (string search in File.ReadAllLines(SearchHistoryFilename))
                            {
                                search_history.Add(search);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was a problem loading the search history.");
                    }
                }
                foreach (string search in search_history_from_disk)
                {
                    search_history.Add(search);
                    if (search_history.Count > 100) break;
                }

                return search_history;
            }
        }

        private void SaveSearchHistory()
        {
            try
            {
                File.WriteAllLines(SearchHistoryFilename, SearchHistory);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem saving the search history.");
            }
        }

#endregion

#region --- Public accessors ----------------------------------------------------------------------------------------

        public bool IsGuest => is_guest;

        public Visibility NoviceVisibility => ConfigurationRecord.GUI_IsNovice ? Visibility.Collapsed : Visibility.Visible;

        public ConfigurationRecord ConfigurationRecord
        {
            get
            {
                if (null == configuration_record)
                {
                    Logging.Warn("Accessing ConfigurationRecord before it has been initialized by Qiqqa: running as Guest for now");
                }
                return configuration_record;
            }
        }

#endregion
    }

#endif

}
