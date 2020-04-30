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

#if DEBUG
        // These 4 constants are live-patched by the Pre-build task (Qiqqa.Build/patch_settings_file.js)
        // vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
        public const string QiqqaDevProjectDir = "W:/Projects/sites/library.visyond.gov/80/lib/tooling/qiqqa/Utilities/";
        public const string QiqqaDevSolutionDir = "W:/Projects/sites/library.visyond.gov/80/lib/tooling/qiqqa/";
        public const string QiqqaDevTargetDir = "W:/Projects/sites/library.visyond.gov/80/lib/tooling/qiqqa/Utilities/bin/Debug/";
        public const string QiqqaDevBuild = "Debug";
        // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        // These 4 constants are live-patched by the Pre-build task (Qiqqa.Build/patch_settings_file.js)
#endif

        /// <summary>
        /// The number of milliseconds we're willing to wait on (background-)tasks 
        /// at application termination/shutdown before forcibly aborting them.
        /// </summary>
        public const int MAX_WAIT_TIME_MS_AT_PROGRAM_SHUTDOWN = 15 * 1000;

        /// <summary>
        /// The number of milliseconds we're willing to wait on QiqqaOCR to finish 
        /// doing what it has been instructed to do.
        /// 
        /// This number is rather large as it must allow for slow machines and 
        /// difficult jobs (complex OCR task of some PDFs).
        /// 
        /// Note that QiqqaOCR internally uses *this* timeout while Qiqqa, which has 
        /// to wait on QiqqaOCR, uses a slightly elevated value to ensure that,
        /// of the two parallel timeouts (one in Qiqqa, one in QiqqaOCR), the timeout
        /// in QiqqaOCR will always win. This is encoded in the 
        /// `EXTRA_TIME_MS_FOR_WAITING_ON_QIQQA_OCR_TASK_TERMINATION` constant.
        /// </summary>
        public const int MAX_WAIT_TIME_MS_FOR_QIQQA_OCR_TASK_TO_TERMINATE = 4 * 60 * 1000;
        public const int EXTRA_TIME_MS_FOR_WAITING_ON_QIQQA_OCR_TASK_TERMINATION = 5 * 1000;
    }
}
