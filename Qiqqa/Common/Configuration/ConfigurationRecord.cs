using System;
using Utilities;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Common.Configuration
{
    [Serializable]
    public class ConfigurationRecord : DictionaryBasedObject
    {
        public ConfigurationRecord()
        {
            Account_Username = Guid.NewGuid().ToString();
            Account_Password = Guid.NewGuid().ToString();
        }

        public string Account_Username
        {
            get
            {
                string account_username = this["Account_Username"] as string;

                // Try to use the old guid if thats all we have...
                if (String.IsNullOrEmpty(account_username))
                {
                    account_username = this["Account_GUID"] as string;
                }

                return account_username;
            }
            set => this["Account_Username"] = value;
        }
        public string Account_Password
        {
            get => this["Account_Password"] as string;
            set => this["Account_Password"] = value;
        }
        public string Account_Nickname
        {
            get
            {
                string nick = this["Account_Nickname"] as string;
                if (String.IsNullOrEmpty(nick))
                {
                    nick = "Guest-" + ConfigurationManager.Instance.ConfigurationRecord.Account_Username.Substring(0, 3);
                }
                return nick;
            }
            set => this["Account_Nickname"] = value;
        }

        public bool Feedback_UtilisationInfo
        {
            get => (this["Feedback_UtilisationInfo"] as bool?) ?? true;
            set => this["Feedback_UtilisationInfo"] = value;
        }

        [Obsolete("Do not use this attribute, but keep it in the class definition for backwards compatibility of the serialization", true)]
        public string Feedback_GATrackingCode
        {
            get => this["Feedback_GATrackingCode"] as string;
            set => this["Feedback_GATrackingCode"] = value;
        }

        [Obsolete("Do not use this attribute, but keep it in the class definition for backwards compatibility of the serialization", true)]
        public int? System_TempAge
        {
            get => this["System_TempAge"] as int?;
            set => this["System_TempAge"] = value;
        }

        public int? System_NumOCRProcesses
        {
            get => this["System_NumOCRProcesses"] as int?;
            set => this["System_NumOCRProcesses"] = value;
        }

        public DateTime? FeatureTrackingLastSentToServer
        {
            get => this["FeatureTrackingLastSentToServer"] as DateTime?;
            set => this["FeatureTrackingLastSentToServer"] = value;
        }

        public string System_LastLibraryExportFolder
        {
            get => this["System_LastLibraryExportFolder"] as string;
            set => this["System_LastLibraryExportFolder"] = value;
        }

        public string System_LastBibTexExportFile
        {
            get => this["System_LastBibTexExportFile"] as string;
            set => this["System_LastBibTexExportFile"] = value;
        }

        public string System_LastWord2007ExportFile
        {
            get => this["System_LastWord2007ExportFile"] as string;
            set => this["System_LastWord2007ExportFile"] = value;
        }

        public string System_OverrideDirectoryForPDFs
        {
            get => this["System_OverrideDirectoryForPDFs"] as string;
            set => this["System_OverrideDirectoryForPDFs"] = value;
        }

        public string System_OverrideDirectoryForOCRs
        {
            get => this["System_OverrideDirectoryForOCRs"] as string;
            set => this["System_OverrideDirectoryForOCRs"] = value;
        }

        public string InCite_LastStyleFile
        {
            get
            {
                string filename = this["InCite_LastStyleFile"] as string;
                if (String.IsNullOrEmpty(filename))
                {
                    filename = Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"InCite/styles/harvard1.csl");
                }
                return Path.GetFullPath(filename);
            }
            set => this["InCite_LastStyleFile"] = value;
        }

        public string InCite_LastLibrary
        {
            get => this["InCite_LastLibrary"] as string;
            set => this["InCite_LastLibrary"] = value;
        }

        public string InCite_WinWordLocation
        {
            get => this["InCite_WinWordLocation"] as string;
            set => this["InCite_WinWordLocation"] = value;
        }

        public bool InCite_UseAbbreviations
        {
            get => (this["InCite_UseAbbreviations"] as bool?) ?? false;
            set => this["InCite_UseAbbreviations"] = value;
        }

        public string InCite_CustomAbbreviationsFilename
        {
            get => this["InCite_CustomAbbreviationsFilename"] as string;
            set => this["InCite_CustomAbbreviationsFilename"] = value;
        }

        public bool Library_OCRDisabled
        {
            get => (this["Library_OCRDisabled"] as bool?) ?? false;
            set => this["Library_OCRDisabled"] = value;
        }

        public bool System_UseExternalWebBrowser
        {
            get => (this["System_UseExternalWebBrowser"] as bool?) ?? false;
            set => this["System_UseExternalWebBrowser"] = value;
        }

        public bool System_DisableSSL
        {
            get => (this["System_DisableSSL"] as bool?) ?? false;
            set
            {
                this["System_DisableSSL"] = value;
                Logging.Debug特("DisableSSL = " + value);
            }
        }

        public bool Wizard_HasSeenIntroWizard
        {
            get => (this["Wizard_HasSeenIntroWizard"] as bool?) ?? false;
            set => this["Wizard_HasSeenIntroWizard"] = value;
        }

        public bool Wizard_HasSeenSearchWizard
        {
            get => (this["Wizard_HasSeenSearchWizard"] as bool?) ?? false;
            set => this["Wizard_HasSeenSearchWizard"] = value;
        }

        public bool Metadata_AutomaticallyAssociateBibTeX
        {
            get => (this["Metadata_AutomaticallyAssociateBibTeX"] as bool?) ?? true;
            set => this["Metadata_AutomaticallyAssociateBibTeX"] = value;
        }

        public bool Metadata_UseBibTeXSnifferWizard
        {
            get => (this["Metadata_UseBibTeXSnifferWizard"] as bool?) ?? true;
            set => this["Metadata_UseBibTeXSnifferWizard"] = value;
        }

        public string Metadata_UserDefinedBibTeXFields
        {
            get => this["Metadata_UserDefinedBibTeXFields"] as string;
            set => this["Metadata_UserDefinedBibTeXFields"] = value;
        }

        public string GUI_UserDefinedSearchStrings
        {
            get => this["GUI_UserDefinedSearchStrings"] as string;
            set => this["GUI_UserDefinedSearchStrings"] = value;
        }

#region --- Proxy ------------------------------------------------------------------------------------

        public bool Proxy_UseProxy
        {
            get => (this["Proxy_UseProxy"] as bool?) ?? false;
            set => this["Proxy_UseProxy"] = value;
        }

        public string Proxy_Hostname
        {
            get => this["Proxy_Hostname"] as string;
            set => this["Proxy_Hostname"] = value;
        }

        public int Proxy_Port
        {
            get => this["Proxy_Port"] as int? ?? 0;
            set => this["Proxy_Port"] = value;
        }

        public string Proxy_Username
        {
            get => this["Proxy_Username"] as string;
            set => this["Proxy_Username"] = value;
        }

        public string Proxy_Password
        {
            get => this["Proxy_Password"] as string;
            set => this["Proxy_Password"] = value;
        }

        public string Web_UserAgentOverride
        {
            get => this["Web_UserAgentOverride"] as string;
            set => this["Web_UserAgentOverride"] = value;
        }

        public string GetWebUserAgent()
        {
            string ua = Web_UserAgentOverride;
            bool overruled = true;

            if (String.IsNullOrEmpty(ua))
            {
                ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:56.0) Gecko/20100101 Firefox/56.0 Waterfox/56.2.14";
                overruled = false;
#if false
                String.Format(
                "Mozilla/5.0 (Windows; {0}; rv:13.0) Gecko/13.0 Firefox/13.0.0",
                    Environment.OSVersion
                );
#endif
            }

            Logging.Info("Using {1}user agent: {0}", ua, (overruled ? "overridden " : "default"));

            return ua;
        }

        public bool GoogleScholar_DoExtraBackgroundQueries
        {
            get => (this["GoogleScholar_DoExtraBackgroundQueries"] as bool?) ?? false;
            set => this["GoogleScholar_DoExtraBackgroundQueries"] = value;
        }

        public string Proxy_EZProxy
        {
            get => this["Proxy_EZProxy"] as string;
            set => this["Proxy_EZProxy"] = value;
        }


#endregion

#region --- Terms and conditions ------------------------------------------------------------------------------------

        public bool SyncTermsAccepted
        {
            get => (this["SyncTermsAccepted_20100615"] as bool?) ?? true;
            set => this["SyncTermsAccepted_20100615"] = value;
        }

#endregion

#region --- Import stuff ------------------------------------------------------------------------------------

        public bool ImportFromFolderImportTagsFromSubfolderNames
        {
            get => (this["ImportFromFolderImportTagsFromSubfolderNames"] as bool?) ?? true;
            set => this["ImportFromFolderImportTagsFromSubfolderNames"] = value;
        }

        public bool ImportFromFolderRecurseSubfolders
        {
            get => (this["ImportFromFolderRecurseSubfolders"] as bool?) ?? true;
            set => this["ImportFromFolderRecurseSubfolders"] = value;
        }

        public string ImportFromFolderLastFolderImported
        {
            get => (this["ImportFromFolderLastFolderImported"] as string);
            set => this["ImportFromFolderLastFolderImported"] = value;
        }

        public bool ImportFromMendeleyAutoDisabled
        {
            get => (this["ImportFromMendeleyAutoDisabled"] as bool?) ?? false;
            set => this["ImportFromMendeleyAutoDisabled"] = value;
        }

        public bool ImportFromEndnoteAutoDisabled
        {
            get => (this["ImportFromEndnoteAutoDisabled"] as bool?) ?? false;
            set => this["ImportFromEndnoteAutoDisabled"] = value;
        }

#endregion

        public DateTime Premium_LastChecked
        {
            get => (this["Premium_LastChecked"] as DateTime?) ?? DateTime.MinValue;
            set => this["Premium_LastChecked"] = value;
        }

        public DateTime Premium_LastNotificationBarReminder
        {
            get => (this["Premium_LastNotificationBarReminder"] as DateTime?) ?? DateTime.MinValue;
            set => this["Premium_LastNotificationBarReminder"] = value;
        }

        public DateTime Marketing_LastNotificationOfAlternativeTo
        {
            get => (this["Marketing_LastNotificationOfAlternativeTo"] as DateTime?) ?? DateTime.MinValue;
            set => this["Marketing_LastNotificationOfAlternativeTo"] = value;
        }

        public bool GUI_IsNovice
        {
            get => (this["GUI_IsNovice"] as bool?) ?? true;
            set => this["GUI_IsNovice"] = value;
        }

        public string GUI_LastSelectedLibraryId
        {
            get => (this["GUI_LastSelectedLibraryId"] as string) ?? "";
            set => this["GUI_LastSelectedLibraryId"] = value;
        }

        public bool GUI_AdvancedMenus
        {
            get => (this["GUI_AdvancedMenus"] as bool?) ?? false;
            set => this["GUI_AdvancedMenus"] = value;
        }

        public double GUI_AnnotationPrintTransparency
        {
            get => (this["GUI_AnnotationPrintTransparency"] as double?) ?? 0.25;

            set
            {
                double transparency = value;
                transparency = Math.Max(0, transparency);
                transparency = Math.Min(1, transparency);
                this["GUI_AnnotationPrintTransparency"] = transparency;
            }
        }

        public double GUI_AnnotationScreenTransparency
        {
            get => (this["GUI_AnnotationScreenTransparency"] as double?) ?? 0.25;

            set
            {
                double transparency = value;
                transparency = Math.Max(0, transparency);
                transparency = Math.Min(1, transparency);
                this["GUI_AnnotationScreenTransparency"] = transparency;
            }
        }

        public double GUI_HighlightScreenTransparency
        {
            get => (this["GUI_HighlightScreenTransparency"] as double?) ?? 0.25;

            set
            {
                double transparency = value;
                transparency = Math.Max(0, transparency);
                transparency = Math.Min(1, transparency);
                this["GUI_HighlightScreenTransparency"] = transparency;
            }
        }

        public double GUI_InkScreenTransparency
        {
            get => (this["GUI_InkScreenTransparency"] as double?) ?? 0.25;

            set
            {
                double transparency = value;
                transparency = Math.Max(0, transparency);
                transparency = Math.Min(1, transparency);
                this["GUI_InkScreenTransparency"] = transparency;
            }
        }

        public string Localisation_ForcedLocale
        {
            get => this["Localisation_ForcedLocale"] as string;
            set => this["Localisation_ForcedLocale"] = value;
        }

        public bool GUI_AskOnExit
        {
            get => (this["GUI_AskOnExit"] as bool?) ?? true;
            set => this["GUI_AskOnExit"] = value;
        }

        public bool GUI_RestoreWindowsAtStartup
        {
            get => (this["GUI_RestoreWindowsAtStartup"] as bool?) ?? false;
            set => this["GUI_RestoreWindowsAtStartup"] = value;
        }

        public bool GUI_RestoreLocationAtStartup
        {
            get => (this["GUI_RestoreLocationAtStartup"] as bool?) ?? false;
            set => this["GUI_RestoreLocationAtStartup"] = value;
        }

        public string GUI_RestoreLocationAtStartup_Position
        {
            get => this["GUI_RestoreLocationAtStartup_Position"] as string;
            set => this["GUI_RestoreLocationAtStartup_Position"] = value;
        }

        public string GUI_LastPagesUp // Can be 1 2 N or W
        {
            get => this["GUI_LastPagesUp"] as string;
            set => this["GUI_LastPagesUp"] = value;
        }

        public bool SpeedRead_PreambleVisible
        {
            get => (this["SpeedRead_PreambleVisible"] as bool?) ?? false;
            set => this["SpeedRead_PreambleVisible"] = value;
        }

        public bool SpeedRead_PostambleVisible
        {
            get => (this["SpeedRead_PostambleVisible"] as bool?) ?? false;
            set => this["SpeedRead_PostambleVisible"] = value;
        }

        public DateTime AutomaticAccountDetails_LibrarySyncLastDate
        {
            get => (this["AutomaticAccountDetails_LibrarySyncLastDate"] as DateTime?) ?? DateTime.MinValue;
            set => this["AutomaticAccountDetails_LibrarySyncLastDate"] = value;
        }

        public DateTime AutomaticAccountDetails_LibraryMembershipLastDate
        {
            get => (this["AutomaticAccountDetails_LibraryMembershipLastDate"] as DateTime?) ?? DateTime.MinValue;
            set => this["AutomaticAccountDetails_LibraryMembershipLastDate"] = value;
        }

        [NonSerialized]
        private bool? disable_all_background;
        public bool DisableAllBackgroundTasks
        {
            get
            {
                if (disable_all_background.HasValue)
                {
                    return disable_all_background.Value;
                }

                disable_all_background = RegistrySettings.Instance.IsSet(RegistrySettings.SuppressDaemon);
                return disable_all_background.Value;
            }
            set
            {
                disable_all_background = value;

                RegistrySettings.Instance.Write(RegistrySettings.SuppressDaemon, value ? "yes" : "no");
            }
        }

        [NonSerialized]
        private bool? snap_to_pixels;
        public bool SnapToPixels
        {
            get
            {
                if (snap_to_pixels.HasValue)
                {
                    return snap_to_pixels.Value;
                }

                snap_to_pixels = RegistrySettings.Instance.IsSet(RegistrySettings.SnapToPixels);
                return snap_to_pixels.Value;
            }
            set
            {
                snap_to_pixels = value;

                RegistrySettings.Instance.Write(RegistrySettings.SnapToPixels, value ? "yes" : "no");
            }
        }
    }
}
