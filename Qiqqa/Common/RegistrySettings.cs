using System;
using Utilities.Misc;

namespace Qiqqa.Common
{
    public class RegistrySettings : QuantisleUserRegistry
    {
        public static RegistrySettings Instance = new RegistrySettings();

        /// <summary>
        /// Output logging to Windows debug console (available in all builds)
        /// </summary>
        public static readonly string DebugConsole = "DebugConsole";

        /// <summary>
        /// Show the Welcome web page on first use. The Registry entry only checks for non-null content,
        /// but stores the version when Qiqqa was first installed and run.
        /// </summary>
        public static readonly string FirstInstallNotification = "FirstInstallNotification";

        /// <summary>
        ///
        /// </summary>
        public static readonly string DisableGlobalKeyHook = "DisableGlobalKeyHook";

        /// <summary>
        ///
        /// </summary>
        public static readonly string AllowMultipleQiqqaInstances = "AllowMultipleQiqqaInstances";

        [Obsolete("Do not use this attribute, only used in Commercial Qiqqa", true)]
        public static readonly string SkipAdverts = "SkipAdverts";                                  // only used in Commercial Qiqqa

        /// <summary>
        /// When set to true/yes/y/t, every new Brainstorm sessions will be filled with some sample content to start with.
        /// </summary>
        public static readonly string SampleBrainstorm = "SampleBrainstorm";

        /// <summary>
        /// Points at qiqqa.com (or alternative website) which serves as the base for many URLs linked to by Qiqqa.
        /// </summary>
        public static readonly string UrlWebRoot = "UrlWebRoot";  //e.g. test.qiqqa.com

        /// <summary>
        /// true: disables several background tasks in Qiqqa. This is the old way of turning those off.
        /// Qiqqa Open Source uses the `ConfigurationRecord.DisableAllBackgroundTasks` setting.
        /// </summary>
        public static readonly string SuppressDaemon = "SuppressDaemon";                            // (deprecated in v82pre5)

        [Obsolete("Do not use this attribute, only used in Commercial Qiqqa", true)]
        public static readonly string LastLoginUsername = "LastLoginUsername";                      // only used in Commercial Qiqqa
        [Obsolete("Do not use this attribute, only used in Commercial Qiqqa", true)]
        public static readonly string LastLoginGuid = "LastLoginGuid";                              // only used in Commercial Qiqqa

        /// <summary>
        /// Base URL for the bibtexsearch.com / search1.bibtexsearch.com web services, where all gathered BibTeX is collected (and produced on demand)
        /// </summary>
        public static readonly string BibTeXSearchSearchUrl = "BibTeXSearchSearchUrl";

        [Obsolete("Do not use this attribute, only used in Commercial Qiqqa", true)]
        public static readonly string PremiumForceYes = "PremiumForceYes";                          // only used in Commercial Qiqqa
        [Obsolete("Do not use this attribute, only used in Commercial Qiqqa", true)]
        public static readonly string PremiumForceNo = "PremiumForceNo";                            // only used in Commercial Qiqqa
        [Obsolete("Do not use this attribute, only used in Commercial Qiqqa", true)]
        public static readonly string PremiumPlusForceYes = "PremiumPlusForceYes";                  // only used in Commercial Qiqqa
        [Obsolete("Do not use this attribute, only used in Commercial Qiqqa", true)]
        public static readonly string PremiumPlusForceNo = "PremiumPlusForceNo";                    // only used in Commercial Qiqqa
        [Obsolete("Do not use this attribute, only used in Commercial Qiqqa", true)]
        public static readonly string ForceGuest = "ForceGuest";                                    // only used in Commercial Qiqqa

        /// <summary>
        /// Old way to set this configuration option.
        /// Qiqqa Open Source uses the `ConfigurationRecord.SnapToPixels` setting.
        /// </summary>
        public static readonly string SnapToPixels = "SnapToPixels";                                // (deprecated in v82pre5)
        /// <summary>
        /// false: starts Qiqqa maximized. Old way to set this configuration option.
        /// Obsoleted in Qiqqa Open Source since v82.
        ///
        /// Since v82, Qiqqa tracks window position and maximize settings in the `GUI_RestoreLocationAtStartup` plus
        /// `GUI_RestoreLocationAtStartup_Position` configuration settings.
        /// </summary>
        public static readonly string StartNotMaximized = "StartNotMaximized";                      // (obsoleted in v82pre5)

        /// <summary>
        /// The most important setting of all: this points at the Qiqqa Libraries Base Directory, i.e. the root directory
        /// where-in Qiqqa will store all its Libraries.
        /// </summary>
        public static readonly string BaseDataDirectory = "BaseDataDirectory";

        public static readonly string WriteJson = "WriteJson";                                      // only used in Commercial Qiqqa

        private RegistrySettings() : base("Qiqqa")
        {
        }
    }
}
