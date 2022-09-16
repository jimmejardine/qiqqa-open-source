using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;
using Utilities.Files;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.IntranetLibraryStuff
{
    internal class IntranetLibraryDetail
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        internal static IntranetLibraryDetail Read(string library_detail_path)
        {
            string json = File.ReadAllText(library_detail_path);
            return JsonConvert.DeserializeObject<IntranetLibraryDetail>(json);
        }

        internal static void Write(string library_detail_path, IntranetLibraryDetail library_detail)
        {
            string json = JsonConvert.SerializeObject(library_detail);
            File.WriteAllText(library_detail_path, json);
        }

        internal static string GetRandomId()
        {
            return "INTRANET_" + Guid.NewGuid().ToString().ToUpper();
        }


        // ---------------------------------------------------------------------------------------

        public static void EnsureIntranetLibraryExists(WebLibraryDetail library_detail)
        {
            EnsureIntranetLibraryExists(library_detail.IntranetPath, library_detail.Title, library_detail.Description, library_detail.Id);
        }

        public static void EnsureIntranetLibraryExists(string db_base_path, string db_title, string db_description, string id = null)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            try
            {
                string base_path = db_base_path;

                if (!Directory.Exists(base_path))
                {
                    DirectoryTools.CreateDirectory(base_path);
                }

                EnsureWarningFilesArePresent(base_path);

                // If the file exists, check that we don't need to update its details
                string library_detail_path = IntranetLibraryTools.GetLibraryDetailPath(base_path);
                if (File.Exists(library_detail_path))
                {
                    try
                    {
                        IntranetLibraryDetail library_detail = IntranetLibraryDetail.Read(library_detail_path);
                        if (library_detail.Title != db_title || library_detail.Description != db_description)
                        {
                            library_detail.Title = db_title;
                            library_detail.Description = db_description;
                            IntranetLibraryDetail.Write(library_detail_path, library_detail);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was an error while updating an Intranet Library path, so will try to delete and recreate... (path: {0})", base_path);
                        FileTools.Delete(library_detail_path);
                    }
                }

                // If the file does not exist, create it from scratch
                if (!File.Exists(library_detail_path))
                {
                    IntranetLibraryDetail library_detail = new IntranetLibraryDetail();
                    library_detail.Id = String.IsNullOrEmpty(id) ? IntranetLibraryDetail.GetRandomId() : id;
                    library_detail.Title = db_title;
                    library_detail.Description = db_description;
                    IntranetLibraryDetail.Write(library_detail_path, library_detail);
                }

                // If the sync database does not exist, put one in place.
                IntranetLibraryDB db = new IntranetLibraryDB(base_path);

                // Notify the WebLibraryManager
                WebLibraryManager.Instance.UpdateKnownWebLibraryFromIntranet(base_path, suppress_flush_to_disk: false, extra_info_message_on_skip: String.Format("as specified in file {0}", library_detail_path));

                // make sure the PDF/documents database is loaded into memory:
                WebLibraryManager.Instance.InitAllLoadedLibraries();
            }
            catch (Exception ex)
            {
                Logging.Error(ex, $"Problem accessing Intranet Library for the first time. (Id: {id}, path: '{db_base_path}', DB title: '{db_title}', Description: '{db_description}'");

                throw;
            }
        }

        private static void EnsureWarningFilesArePresent(string base_path)
        {
            IEnumerable<string> warning_files = Directory.EnumerateFiles(base_path, "---*");
            if (7 != warning_files.Count())
            {
                EnsureWarningFilesArePresent_TOUCH(base_path, "---0--- --------------------------------------------------------------------------");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---1--- THIS IS A QIQQA INTRANET LIBRARY SYNC FOLDER");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---2--- --------------------------------------------------------------------------");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---3--- PLEASE TREAT IT AS A BLACK BOX DATABASE");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---4--- DO NOT MANUALLY ADD-DELETE-ALTER ANY FILES");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---5--- ALL MODIFICATIONS TO THIS FOLDER SHOULD BE DONE THROUGH QIQQA");
                EnsureWarningFilesArePresent_TOUCH(base_path, "---9--- --------------------------------------------------------------------------");
            }
        }

        private static void EnsureWarningFilesArePresent_TOUCH(string base_path, string filename)
        {
            string path = Path.GetFullPath(Path.Combine(base_path, filename));

            try
            {
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, "");
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem writing Intranet Library mount point warnings (path: {0})", path);
            }
        }
    }
}
