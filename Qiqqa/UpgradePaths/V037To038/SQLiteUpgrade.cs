using System;
using System.Collections.Generic;
using System.IO;
using Qiqqa.Main.SplashScreenStuff;
using Utilities;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.UpgradePaths.V037To038
{
    internal class SQLiteUpgrade
    {
        private static List<string> EXTENSIONS = new List<string>(
            new string[] {
                "metadata",
                "annotations",
                "highlights",
                "inks",
                "citations",
            }
            );

        internal static void RunUpgrade(SplashScreenWindow splashscreen_window)
        {
            Logging.Info("Upgrading from 037 to 038");

            string base_directory_path = BaseDirectoryForQiqqa;
            if (Directory.Exists(base_directory_path))
            {
                int info_library_count, info_item_count;

                string[] library_directories = Directory.GetDirectories(base_directory_path);
                info_library_count = 0;
                foreach (string library_directory in library_directories)
                {
                    ++info_library_count;
                    Logging.Info("Inspecting directory {0}", library_directory);

                    string documents_directory = Path.GetFullPath(Path.Combine(library_directory, @"documents"));
                    string database_file = Path.GetFullPath(Path.Combine(library_directory, @"Qiqqa.library"));

                    if (!File.Exists(database_file) && Directory.Exists(documents_directory))
                    {
                        Logging.Warn("We have to upgrade {0}", library_directory);

                        SQLiteUpgrade_LibraryDB library_db = new SQLiteUpgrade_LibraryDB(library_directory);

                        using (var connection = library_db.GetConnection())
                        {
                            connection.Open();
                            using (var transaction = connection.BeginTransaction())
                            {
                                // Get a list of ALL the files in the documents directory...
                                string[] full_filenames = Directory.GetFiles(documents_directory, "*.*", SearchOption.AllDirectories);
                                info_item_count = 0;
                                foreach (string full_filename in full_filenames)
                                {
                                    ++info_item_count;
                                    splashscreen_window.UpdateMessage("Upgrading library {0}/{1}: {2:P0}", info_library_count, library_directories.Length, info_item_count / (double)full_filenames.Length);

                                    string fingerprint = Path.GetFileNameWithoutExtension(full_filename);
                                    string extension = Path.GetExtension(full_filename).Trim('.');

                                    if (EXTENSIONS.Contains(extension))
                                    {
                                        Logging.Info("Upgrading {0}--{1}", fingerprint, extension);
                                        byte[] data = File.ReadAllBytes(full_filename);
                                        library_db.PutBlob(connection, transaction, fingerprint, extension, data);
                                    }
                                    else
                                    {
                                        Logging.Info("NOT upgrading {0}--{1}", fingerprint, extension);
                                    }
                                }

                                transaction.Commit();
                            }
                        }
                    }
                }
            }

            splashscreen_window.UpdateMessage("Finished migrating libraries.");
        }

        public static string BaseDirectoryForQiqqa
        {
            get
            {
                string base_directory_for_qiqqa = null;

                if (null == base_directory_for_qiqqa)
                {
                    string override_path = LOCALRegistrySettings.Instance.Read(LOCALRegistrySettings.BaseDataDirectory);
                    if (!String.IsNullOrEmpty(override_path))
                    {
                        override_path = override_path.Trim();
                        if (!String.IsNullOrEmpty(override_path))
                        {
                            base_directory_for_qiqqa = Path.GetFullPath(override_path);

                            // Check that the path is reasonable
                            try
                            {
                                Directory.CreateDirectory(base_directory_for_qiqqa);
                            }
                            catch (Exception ex)
                            {
                                Logging.Error(ex, "There was a problem creating the user-overridden base directory, so reverting to default");
                                base_directory_for_qiqqa = null;
                            }
                        }
                    }

                    // If we get here, use the default path
                    if (null == base_directory_for_qiqqa)
                    {
                        base_directory_for_qiqqa = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Quantisle/Qiqqa"));
                    }
                }

                return base_directory_for_qiqqa;
            }
        }


        private class LOCALRegistrySettings : QuantisleUserRegistry
        {
            public static LOCALRegistrySettings Instance = new LOCALRegistrySettings();
            public static readonly string BaseDataDirectory = "BaseDataDirectory";
            private LOCALRegistrySettings()
                : base("Qiqqa")
            {
            }
        }

    }
}
