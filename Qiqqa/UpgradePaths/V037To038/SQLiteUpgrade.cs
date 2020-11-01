using System;
using System.Collections.Generic;
using System.IO;
using Utilities;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;
using LibraryDB = Qiqqa.DocumentLibrary.LibraryDB;
using Qiqqa.DocumentLibrary.IntranetLibraryStuff;
using Qiqqa.Common.Configuration;

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

        internal static void RunUpgrade()
        {
            Logging.Info("Upgrading from 037 to 038");

            string base_directory_path = ConfigurationManager.Instance.BaseDirectoryForQiqqa;
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
                    string database_file = LibraryDB.GetLibraryDBPath(library_directory);
                    string database_syncref_file = IntranetLibraryTools.GetLibraryMetadataPath(library_directory);

                    // make sure we skip S3DB internet DB sync directories and only go through the upgrade process
                    // when this looks like a viable (local) Qiqqa library:
                    if (!File.Exists(database_file) && Directory.Exists(documents_directory) && !File.Exists(database_syncref_file))
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
                                    StatusManager.Instance.UpdateStatus("DBUpgrade", String.Format("Upgrading library {0}/{1}", info_library_count, library_directories.Length), info_item_count, full_filenames.Length);

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

            StatusManager.Instance.UpdateStatus("DBUpgrade", "Finished migrating libraries.");
        }
    }
}
