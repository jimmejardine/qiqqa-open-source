using System;
using Utilities;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.UpgradePaths.V003To004
{
    internal class Upgrade
    {
        internal static void RunUpgrade()
        {
            Logging.Info("Upgrading from 003 to 004");
            //MoveHomeLibraryToGuest();
        }

        private static void MoveHomeLibraryToGuest()
        {
            try
            {
                string OLD_BASE_PATH = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Quantisle/Qiqqa"));
                if (Directory.Exists(OLD_BASE_PATH))
                {
                    string NEW_BASE_PATH = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Quantisle/Qiqqa/Guest"));

                    // Check that we have the Guest directory
                    if (!Directory.Exists(NEW_BASE_PATH))
                    {
                        Logging.Info("Guest directory does not exist, so creating it");
                        Directory.CreateDirectory(NEW_BASE_PATH);
                    }

                    // Move the documents if we have them
                    string old_documents_path = Path.GetFullPath(Path.Combine(OLD_BASE_PATH, @"documents"));
                    if (Directory.Exists(old_documents_path))
                    {
                        Logging.Info("Old style documents path exists, so moving it");
                        string new_documents_path = Path.GetFullPath(Path.Combine(NEW_BASE_PATH, @"documents"));
                        Directory.Move(old_documents_path, new_documents_path);
                    }

                    // Move the application files if we have them
                    string[] application_filenames = new string[]
                {
                    "Qiqqa.configuration", "Qiqqa.most_recently_read", "Qiqqa.utilisation"
                };

                    foreach (string application_filename in application_filenames)
                    {
                        string old_filename = OLD_BASE_PATH + application_filename;
                        if (File.Exists(old_filename))
                        {
                            Logging.Info("Old style filename exists, so moving it ({0})", old_filename);
                            string new_filename = Path.GetFullPath(Path.Combine(NEW_BASE_PATH, application_filename));
                            File.Move(old_filename, new_filename);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Upgrade V003-to-V004 failure");
            }
        }
    }
}
