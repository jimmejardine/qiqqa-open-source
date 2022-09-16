using System;
using System.IO;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Common
{
    internal class DropboxChecker
    {
        private static readonly string WARNING = "Using cloud syncing tools like Dropbox, GoogleDrive, SkyDrive, etc. can corrupt your Qiqqa database.  Only use them if you know what you are doing...";
        private static readonly string PREAMBLE_FILENAME = "DropboxDetection";

        internal static void DoCheck()
        {
            try
            {
                Logging.Info("Checking for Dropbox conflicts in {0} for machine {1}", ConfigurationManager.Instance.BaseDirectoryForQiqqa, Environment.MachineName);

                // Write our version
                string FULL_FILENAME = Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.BaseDirectoryForQiqqa, PREAMBLE_FILENAME + @"." + Environment.MachineName + @".txt"));
                File.WriteAllText(FULL_FILENAME, WARNING);

                // Check for other's versions
                string[] matching_files = Directory.GetFiles(ConfigurationManager.Instance.BaseDirectoryForQiqqa, PREAMBLE_FILENAME + @"*", SearchOption.TopDirectoryOnly);
                if (1 < matching_files.Length)
                {
                    // We have a problem, Houston...

                    // Analytics it
                    FeatureTrackingManager.Instance.UseFeature(Features.Diagnostics_DropBox);

                    // Report it to user
                    NotificationManager.Instance.AddPendingNotification(
                        new NotificationManager.Notification(
                            WARNING,
                            "Danger using 3rd party cloud tools!",
                            NotificationManager.NotificationType.Warning,
                            Icons.No,
                            "I understand!",
                            IUnderstand
                        )
                    );
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem checking for Dropbox.");
            }
        }

        private static void IUnderstand(object obj)
        {
            try
            {
                string[] matching_files = Directory.GetFiles(ConfigurationManager.Instance.BaseDirectoryForQiqqa, PREAMBLE_FILENAME + @"*", SearchOption.TopDirectoryOnly);
                foreach (string filename in matching_files)
                {
                    File.Delete(filename);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem tidying up checking for Dropbox.");
            }
        }
    }
}
