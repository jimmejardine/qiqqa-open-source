using System;

namespace Utilities
{
    public class Constants
    {
        public static readonly DateTime DATETIME_MIN = DateTime.MinValue;

        public const string TITLE_UNKNOWN = "(unknown title)";
        public const string UNKNOWN_AUTHORS = "(unknown authors)";
        public const string UNKNOWN_YEAR = "(unknown year)";

        public const string VanillaReferenceFileType = "VANILLA_REFERENCE";

        // These 4 constants are live-patched by the Pre-build task (Qiqqa.Build/patch_settings_file.js)
        // vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
        public const string QiqqaDevProjectDir = "W:/Users/Ger/Projects/sites/library.visyond.gov/80/lib/tooling/qiqqa/Utilities/";
        public const string QiqqaDevSolutionDir = "W:/Users/Ger/Projects/sites/library.visyond.gov/80/lib/tooling/qiqqa/";
        public const string QiqqaDevTargetDir = "W:/Users/Ger/Projects/sites/library.visyond.gov/80/lib/tooling/qiqqa/Utilities/bin/Debug/";
        public const string QiqqaDevBuild = "Debug";
        // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        // These 4 constants are live-patched by the Pre-build task (Qiqqa.Build/patch_settings_file.js)

        /// <summary>
        /// The number of milliseconds we're willing to wait on (background-)tasks 
        /// at application termination/shutdown before forcibly aborting them.
        /// </summary>
        public const int MAX_WAIT_TIME_MS_AT_PROGRAM_SHUTDOWN = 15 * 1000;
    }
}
