using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows;
using Qiqqa.WebBrowsing.GeckoStuff;
using Utilities;
using Utilities.Files;
using Utilities.Internet;
using Utilities.Reflection;
using Utilities.Shutdownable;
using Utilities.Strings;
using UConf = Utilities.Configuration;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using Utilities.Misc;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Qiqqa.Common.Configuration
{
    public class ConfigurationManager
    {
        // https://stackoverflow.com/questions/10465961/potential-pitfalls-with-static-constructors-in-c-sharp
        private static readonly Lazy<ConfigurationManager> __instance = new Lazy<ConfigurationManager>(() =>
        {
            return new ConfigurationManager();
        });
        public static ConfigurationManager Instance
        {
            get
            {
                return __instance.Value;
            }
        }

        public static bool IsInitialized
        {
            get
            {
                if (!__instance.IsValueCreated) return false;

                if (!Instance.__BaseDirectoryForQiqqa.IsValueCreated) return false;

                try
                {
                    return (Instance.__BaseDirectoryForQiqqa.Value != null);
                }
                catch
                {
                    // error doesn't matter; the fact that there IS an error is enough
                    return false;
                }
            }
        }

        private string user_guid;
        private bool is_guest;

        private readonly Lazy<string> __startupDirectoryForQiqqa = new Lazy<string>(() => UnitTestDetector.StartupDirectoryForQiqqa);
        public string StartupDirectoryForQiqqa => __startupDirectoryForQiqqa.Value;

        private readonly Lazy<string> __BaseDirectoryForQiqqa = new Lazy<string>(() =>
        {
            // Command-line parameters override the Registry:
            string[] args = Environment.GetCommandLineArgs();
            Exception ex_to_report = null;

            // argv[0] is the executable itself; commandline parameters start at index 1
            //
            // TODO: make this part of a proper commandline parse, where we also look at other options.
            for (int i = 1; i < args.Length; i++)
            {
                string p = args[i];
                try
                {
                    string dp = Path.GetFullPath(p);
                    if (Directory.Exists(dp))
                    {
                        return dp;
                    }
                    else
                    {
                        // if directory does not exist (and we've made sure we're not looking at a commandline option argument)
                        // we create it on the spot: commandline parameter overrides registry setting.
                        if (!p.StartsWith("-"))
                        {
                            Directory.CreateDirectory(dp);
                            if (Directory.Exists(dp))
                            {
                                return dp;
                            }
                            else
                            {
                                ex_to_report = new ApplicationException(String.Format("There was a problem creating the commandline-specified base directory '{0}'.", p));
                                throw ex_to_report;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // ignore but report all errors:
                    Logging.Error(ex, "There was a problem creating the commandline-overridden base directory '{0}'", p);
                }
                if (ex_to_report != null)
                {
                    throw ex_to_report;
                }
            }

            // Check the Windows Registry for the path setting:
            string override_path = RegistrySettings.Instance.Read(RegistrySettings.BaseDataDirectory);
            if (!String.IsNullOrEmpty(override_path))
            {
                override_path = override_path.Trim();
                if (!String.IsNullOrEmpty(override_path))
                {
                    string p = Path.GetFullPath(override_path);

                    // Check that the path is reasonable
                    try
                    {
                        Directory.CreateDirectory(p);
                        return p;
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was a problem creating the user-overridden base directory '{0}', so reverting to default", p);
                    }
                }
            }

            // If we get here, use the default path
            return Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Quantisle/Qiqqa"));
        });
        public string BaseDirectoryForQiqqa
        {
            get => __BaseDirectoryForQiqqa.Value;
        }

        public string BaseDirectoryForUser
        {
            get => Path.GetFullPath(Path.Combine(BaseDirectoryForQiqqa, user_guid));
        }

        private string ConfigFilenameForUser
        {
            get => Path.Combine(BaseDirectoryForUser, @"Qiqqa.configuration");
        }

        private string SearchHistoryFilename
        {
            get => Path.Combine(BaseDirectoryForUser, @"Qiqqa.search_history");
        }
        public string Program7ZIP
        {
            get => Path.Combine(StartupDirectoryForQiqqa, @"7za.exe");
        }

        public string ProgramHTMLToPDF
        {
            get => Path.Combine(StartupDirectoryForQiqqa, @"wkhtmltopdf.exe");
        }

        public string DeveloperTestSettingsFilename
        {
            get => Path.Combine(BaseDirectoryForQiqqa, @"Qiqqa.Developer.Settings.json5");
        }

        private Dictionary<string, object> developer_test_settings = null;
        private ConfigurationRecord configuration_record;
        private AugmentedBindable<ConfigurationRecord> configuration_record_bindable;

        private ConfigurationManager()
        {
            ShutdownableManager.Instance.Register(Shutdown);

            UConf.GetWebUserAgent = () => ConfigurationManager.Instance.ConfigurationRecord.GetWebUserAgent();
            UConf.GetProxy = () => ConfigurationManager.Instance.Proxy;

            ResetConfigurationRecordToGuest();
        }

        private void Shutdown()
        {
            Logging.Info("ConfigurationManager is saving the configuration at shutdown");
            SaveConfigurationRecord(force_save: true);
            SaveSearchHistory();
        }

        private void ResetConfigurationRecord(string user_guid_, bool is_guest_)
        {
            Logging.Info("Resetting configuration settings to {0}", user_guid_);

            if (null != configuration_record_bindable)
            {
                configuration_record_bindable.PropertyChanged -= configuration_record_bindable_PropertyChanged;
            }

            // Create the new user_guid
            user_guid = user_guid_;
            is_guest = is_guest_;

            // Create the base directory in case it doesn't exist
            Directory.CreateDirectory(BaseDirectoryForUser);

            // Try loading any pre-existing config file
            try
            {
                if (File.Exists(ConfigFilenameForUser))
                {
                    Logging.Info("Loading configuration file {0}", ConfigFilenameForUser);
                    configuration_record = (ConfigurationRecord)ObjectSerializer.LoadObject(ConfigFilenameForUser);
                    Logging.Info("Loaded configuration file {0}", ConfigFilenameForUser);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem loading configuration file {0}", ConfigFilenameForUser);
            }

            if (null == configuration_record)
            {
                Logging.Info("There is no existing configuration, so creating one from scratch. Is configuration file {0} missing?", ConfigFilenameForUser);
                configuration_record = new ConfigurationRecord();
            }

#if TEST_FIRST_TIME_QIQQA_EXECUTION
            // Testing....
            configuration_record.SyncTermsAccepted = false;
#endif

            // Attach a bindable to the configuration record so that GUIs can update it
            configuration_record_bindable = new AugmentedBindable<ConfigurationRecord>(configuration_record);
            configuration_record_bindable.PropertyChanged += configuration_record_bindable_PropertyChanged;

#if false
            // Make sure we have a GA tracking ID
            if (String.IsNullOrEmpty(configuration_record.Feedback_GATrackingCode))
            {
                configuration_record.Feedback_GATrackingCode = Guid.NewGuid().ToString();
            }
#endif

            // Also see if we have a Developer Test Settings file, which contains development/test environment overrides:
            try
            {
                if (File.Exists(DeveloperTestSettingsFilename))
                {
                    Logging.Info("Loading developer test settings file {0}", DeveloperTestSettingsFilename);

                    // see also https://www.newtonsoft.com/json/help/html/SerializationErrorHandling.htm

                    List<string> errors = new List<string>();

                    developer_test_settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(
                        File.ReadAllText(DeveloperTestSettingsFilename),
                        new JsonSerializerSettings
                        {
                            Error = delegate (object sender, ErrorEventArgs args)
                            {
                                errors.Add(args.ErrorContext.Error.Message);
                                args.ErrorContext.Handled = true;
                            },
                            //Converters = { new IsoDateTimeConverter() }
                        });

                    Logging.Info("Loaded developer test settings file {0}: {1}", DeveloperTestSettingsFilename, errors.Count == 0 ? "no errors" : errors.ToString());
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem loading developer test settings file {0}", DeveloperTestSettingsFilename);
            }
        }

        private void configuration_record_bindable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == configuration_record_bindable)
            {
                // Saving the config on property change is non-essential as another save action will be triggered
                // from the application shutdown handler anyway. Therefor we delegate the inherent file I/O to
                // a background task:
                SafeThreadPool.QueueUserWorkItem(o =>
                {
                    SaveConfigurationRecord();
                }, skip_task_at_app_shutdown: true);

                GeckoManager.SetupProxyAndUserAgent(false);
            }
            else
            {
                Logging.Warn("Not saving configuration record from old bindable wrapper");
            }
        }

        private Stopwatch lastSaveTimestamp = null;

        public void SaveConfigurationRecord(bool force_save = false)
        {
            // do not even attempt to save a configuration which has not been properly loaded/initialized yet:
            if (configuration_record == null)
            {
                return;
            }
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

#region --- Public initialisation ----------------------------------------------------------------------------------------

        public void ResetConfigurationRecordToGuest()
        {
            ResetConfigurationRecord("Guest", true);
        }

        public void ResetConfigurationRecordToUser(string user_guid_)
        {
            ResetConfigurationRecord(user_guid_, false);
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
                    ResetConfigurationRecordToGuest();
                }
                return configuration_record;
            }
        }

        public static bool IsEnabled(string key)
        {
            if (null != Instance.developer_test_settings)
            {
                if (Instance.developer_test_settings.TryGetValue(key, out var val))
                {
                    bool? rv = val as bool?;
                    return rv ?? true;
                }
            }
            return true;
        }

        public static void ThrowWhenActionIsNotEnabled(string key)
        {
            if (!IsEnabled(key))
            {
                throw new ApplicationException($"{key} is not enabled in the developer test settings.");
            }
        }

        public AugmentedBindable<ConfigurationRecord> ConfigurationRecord_Bindable => configuration_record_bindable;

        /// <summary>
        /// Uses the proxy settings specified by the user.  Otherwise defaults to the system proxy.
        /// </summary>
        public IWebProxy Proxy
        {
            get
            {
                string hostname = ConfigurationRecord.Proxy_Hostname;
                if (!String.IsNullOrEmpty(hostname))
                {
                    hostname = hostname.Trim();
                    hostname = hostname.ToLower();
                    hostname = hostname.Replace("http://", "");
                    hostname = hostname.Replace("https://", "");
                }

                string username = ConfigurationRecord.Proxy_Username;
                if (!String.IsNullOrEmpty(username)) username = username.Trim();

                string password = ConfigurationRecord.Proxy_Password;
                if (!String.IsNullOrEmpty(password)) password = password.Trim();

                return ProxyTools.CreateProxy(ConfigurationRecord.Proxy_UseProxy, hostname, ConfigurationRecord.Proxy_Port, username, password);
            }
        }

#endregion
    }
}
