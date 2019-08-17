using System;
using System.Reflection;
using Utilities;
using Utilities.Misc;

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
            set { this["Account_Username"] = value; }
        }
        public string Account_Password
        {
            get { return this["Account_Password"] as string; }
            set { this["Account_Password"] = value; }
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
            set { this["Account_Nickname"] = value; }
        }

        public bool Feedback_UtilisationInfo
        {
            get { return (this["Feedback_UtilisationInfo"] as bool?) ?? true; }
            set { this["Feedback_UtilisationInfo"] = value; }
        }

        public string Feedback_GATrackingCode
        {
            get { return this["Feedback_GATrackingCode"] as string; }
            set { this["Feedback_GATrackingCode"] = value; }
        }

        [Obsolete("Do not use this attribute, but keep it in the class definition for backwards compatibility of the serialization", true)]
        public int? System_TempAge
        {
            get { return this["System_TempAge"] as int?; }
            set { this["System_TempAge"] = value; }
        }

        public int? System_NumOCRProcesses
        {
            get { return this["System_NumOCRProcesses"] as int?; }
            set { this["System_NumOCRProcesses"] = value; }
        }

        public DateTime? FeatureTrackingLastSentToServer
        {
            get { return this["FeatureTrackingLastSentToServer"] as DateTime?; }
            set { this["FeatureTrackingLastSentToServer"] = value; }
        }

        public string System_LastLibraryExportFolder
        {
            get { return this["System_LastLibraryExportFolder"] as string; }
            set { this["System_LastLibraryExportFolder"] = value; }
        }

        public string System_LastBibTexExportFile
        {
            get { return this["System_LastBibTexExportFile"] as string; }
            set { this["System_LastBibTexExportFile"] = value; }
        }

        public string System_LastWord2007ExportFile
        {
            get { return this["System_LastWord2007ExportFile"] as string; }
            set { this["System_LastWord2007ExportFile"] = value; }
        }

        public string System_OverrideDirectoryForPDFs
        {
            get { return this["System_OverrideDirectoryForPDFs"] as string; }
            set { this["System_OverrideDirectoryForPDFs"] = value; }
        }

        public string System_OverrideDirectoryForOCRs
        {
            get { return this["System_OverrideDirectoryForOCRs"] as string; }
            set { this["System_OverrideDirectoryForOCRs"] = value; }
        }

        public string InCite_LastStyleFile
        {
            get 
            {
                string filename = this["InCite_LastStyleFile"] as string;
                if (String.IsNullOrEmpty(filename))
                {
                    filename = ConfigurationManager.Instance.StartupDirectoryForQiqqa + @"\InCite\styles\harvard1.csl";
                }
                return filename;
            }
            set { this["InCite_LastStyleFile"] = value; }
        }

        public string InCite_LastLibrary
        {
            get { return this["InCite_LastLibrary"] as string; }
            set { this["InCite_LastLibrary"] = value; }
        }

        public string InCite_WinWordLocation
        {
            get { return this["InCite_WinWordLocation"] as string; }
            set { this["InCite_WinWordLocation"] = value; }
        }

        public bool InCite_UseAbbreviations
        {
            get { return (this["InCite_UseAbbreviations"] as bool?) ?? false; }
            set { this["InCite_UseAbbreviations"] = value; }
        }

        public string InCite_CustomAbbreviationsFilename
        {
            get { return this["InCite_CustomAbbreviationsFilename"] as string; }
            set { this["InCite_CustomAbbreviationsFilename"] = value; }
        }

        public bool Library_OCRDisabled
        {
            get { return (this["Library_OCRDisabled"] as bool?) ?? false; }
            set { this["Library_OCRDisabled"] = value; }
        }

        public bool System_UseExternalWebBrowser
        {
            get { return (this["System_UseExternalWebBrowser"] as bool?) ?? false; }
            set { this["System_UseExternalWebBrowser"] = value; }
        }

        public bool System_DisableSSL
        {
            get { return (this["System_DisableSSL"] as bool?) ?? false; }
            set 
            { 
                this["System_DisableSSL"] = value;
                Logging.Info("DisableSSL = " + value);
            }
        }

        public bool Wizard_HasSeenIntroWizard
        {
            get { return (this["Wizard_HasSeenIntroWizard"] as bool?) ?? false; }
            set { this["Wizard_HasSeenIntroWizard"] = value; }
        }

        public bool Wizard_HasSeenSearchWizard
        {
            get { return (this["Wizard_HasSeenSearchWizard"] as bool?) ?? false; }
            set { this["Wizard_HasSeenSearchWizard"] = value; }
        }

        public bool Metadata_AutomaticallyAssociateBibTeX
        {
            get { return (this["Metadata_AutomaticallyAssociateBibTeX"] as bool?) ?? true; }
            set { this["Metadata_AutomaticallyAssociateBibTeX"] = value; }
        }

        public bool Metadata_UseBibTeXSnifferWizard
        {
            get { return (this["Metadata_UseBibTeXSnifferWizard"] as bool?) ?? true; }
            set { this["Metadata_UseBibTeXSnifferWizard"] = value; }
        }

        public string Metadata_UserDefinedBibTeXFields
        {
            get { return this["Metadata_UserDefinedBibTeXFields"] as string; }
            set { this["Metadata_UserDefinedBibTeXFields"] = value; }
        }

        public string GUI_UserDefinedSearchStrings
        {
            get { return this["GUI_UserDefinedSearchStrings"] as string; }
            set { this["GUI_UserDefinedSearchStrings"] = value; }
        }

        #region --- Proxy ------------------------------------------------------------------------------------

        public bool Proxy_UseProxy
        {
            get { return (this["Proxy_UseProxy"] as bool?) ?? false; }
            set { this["Proxy_UseProxy"] = value; }
        }

        public string Proxy_Hostname
        {
            get { return this["Proxy_Hostname"] as string; }
            set { this["Proxy_Hostname"] = value; }
        }

        public int Proxy_Port
        {
            get { return this["Proxy_Port"] as int? ?? 0; }
            set { this["Proxy_Port"] = value; }
        }

        public string Proxy_Username
        {
            get { return this["Proxy_Username"] as string; }
            set { this["Proxy_Username"] = value; }
        }

        public string Proxy_Password
        {
            get { return this["Proxy_Password"] as string; }
            set { this["Proxy_Password"] = value; }
        }

        public string Web_UserAgentOverride
        {
            get { return this["Web_UserAgentOverride"] as string; }
            set { this["Web_UserAgentOverride"] = value; }
        }

        public string Proxy_EZProxy
        {
            get { return this["Proxy_EZProxy"] as string; }
            set { this["Proxy_EZProxy"] = value; }
        }


        #endregion

        #region --- Terms and conditions ------------------------------------------------------------------------------------

        public bool TermsAndConditionsAccepted
        {
            get { return (this["TermsAndConditionsAccepted_20110517"] as bool?) ?? false; }
            set { this["TermsAndConditionsAccepted_20110517"] = value; }
        }

        public bool SyncTermsAccepted
        {
            get { return (this["SyncTermsAccepted_20100615"] as bool?) ?? false; }
            set { this["SyncTermsAccepted_20100615"] = value; }
        }

        #endregion

        #region --- Import stuff ------------------------------------------------------------------------------------

        public bool ImportFromFolderImportTagsFromSubfolderNames
        {
            get { return (this["ImportFromFolderImportTagsFromSubfolderNames"] as bool?) ?? true; }
            set { this["ImportFromFolderImportTagsFromSubfolderNames"] = value; }
        }

        public bool ImportFromFolderRecurseSubfolders
        {
            get { return (this["ImportFromFolderRecurseSubfolders"] as bool?) ?? true; }
            set { this["ImportFromFolderRecurseSubfolders"] = value; }
        }

        public string ImportFromFolderLastFolderImported
        {
            get { return (this["ImportFromFolderLastFolderImported"] as string); }
            set { this["ImportFromFolderLastFolderImported"] = value; }
        }

        public bool ImportFromMendeleyAutoDisabled
        {
            get { return (this["ImportFromMendeleyAutoDisabled"] as bool?) ?? false; }
            set { this["ImportFromMendeleyAutoDisabled"] = value; }
        }

        public bool ImportFromEndnoteAutoDisabled
        {
            get { return (this["ImportFromEndnoteAutoDisabled"] as bool?) ?? false; }
            set { this["ImportFromEndnoteAutoDisabled"] = value; }
        }

        #endregion

        public DateTime Premium_LastChecked
        {
            get { return (this["Premium_LastChecked"] as DateTime?) ?? DateTime.MinValue; }
            set { this["Premium_LastChecked"] = value; }
        }

        public DateTime Premium_LastNotificationBarReminder
        {
            get { return (this["Premium_LastNotificationBarReminder"] as DateTime?) ?? DateTime.MinValue; }
            set { this["Premium_LastNotificationBarReminder"] = value; }
        }

        public DateTime Marketing_LastNotificationOfAlternativeTo
        {
            get { return (this["Marketing_LastNotificationOfAlternativeTo"] as DateTime?) ?? DateTime.MinValue; }
            set { this["Marketing_LastNotificationOfAlternativeTo"] = value; }
        }

        public bool GUI_IsNovice
        {
            get { return (this["GUI_IsNovice"] as bool?) ?? true; }
            set { this["GUI_IsNovice"] = value; }
        }

        public string GUI_LastSelectedLibraryId
        {
            get { return (this["GUI_LastSelectedLibraryId"] as string) ?? ""; }
            set { this["GUI_LastSelectedLibraryId"] = value; }
        }
        
        public bool GUI_AdvancedMenus
        {
            get { return (this["GUI_AdvancedMenus"] as bool?) ?? false; }
            set { this["GUI_AdvancedMenus"] = value; }
        }

        public double GUI_AnnotationPrintTransparency
        {
            get { return (this["GUI_AnnotationPrintTransparency"] as double?) ?? 0.25; }

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
            get
            {
                return (this["GUI_AnnotationScreenTransparency"] as double?) ?? 0.25;
            }

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
            get
            {
                return (this["GUI_HighlightScreenTransparency"] as double?) ?? 0.25;
            }

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
            get
            {
                return (this["GUI_InkScreenTransparency"] as double?) ?? 0.25;
            }

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
            get { return this["Localisation_ForcedLocale"] as string; }
            set { this["Localisation_ForcedLocale"] = value; }
        }

        public bool GUI_AskOnExit
        {
            get { return (this["GUI_AskOnExit"] as bool?) ?? true; }
            set { this["GUI_AskOnExit"] = value; }
        }

        public bool GUI_RestoreWindowsAtStartup
        {
            get { return (this["GUI_RestoreWindowsAtStartup"] as bool?) ?? false; }
            set { this["GUI_RestoreWindowsAtStartup"] = value; }
        }

        public bool GUI_RestoreLocationAtStartup
        {
            get { return (this["GUI_RestoreLocationAtStartup"] as bool?) ?? false; }
            set { this["GUI_RestoreLocationAtStartup"] = value; }
        }

        public string GUI_RestoreLocationAtStartup_Position
        {
            get { return this["GUI_RestoreLocationAtStartup_Position"] as string; }
            set { this["GUI_RestoreLocationAtStartup_Position"] = value; }
        }

        public string GUI_LastPagesUp // Can be 1 2 N or W
        {
            get { return this["GUI_LastPagesUp"] as string; }
            set { this["GUI_LastPagesUp"] = value; }
        }

        public bool SpeedRead_PreambleVisible
        {
            get { return (this["SpeedRead_PreambleVisible"] as bool?) ?? false; }
            set { this["SpeedRead_PreambleVisible"] = value; }
        }

        public bool SpeedRead_PostambleVisible
        {
            get { return (this["SpeedRead_PostambleVisible"] as bool?) ?? false; }
            set { this["SpeedRead_PostambleVisible"] = value; }
        }

        public DateTime AutomaticAccountDetails_LibrarySyncLastDate
        {
            get { return (this["AutomaticAccountDetails_LibrarySyncLastDate"] as DateTime?) ?? DateTime.MinValue; }
            set { this["AutomaticAccountDetails_LibrarySyncLastDate"] = value; }
        }

        public DateTime AutomaticAccountDetails_LibraryMembershipLastDate
        {
            get { return (this["AutomaticAccountDetails_LibraryMembershipLastDate"] as DateTime?) ?? DateTime.MinValue; }
            set { this["AutomaticAccountDetails_LibraryMembershipLastDate"] = value; }
        }

        public DisableAllBackgroundTasks
        {
            get { return (this["DisableAllBackgroundTasks"] as bool?) ?? false; }
            set { this["DisableAllBackgroundTasks"] = value; }
        }
    }
}
