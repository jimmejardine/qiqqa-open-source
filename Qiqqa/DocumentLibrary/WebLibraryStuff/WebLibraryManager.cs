using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.BundleLibrary;
using Qiqqa.DocumentLibrary.IntranetLibraryStuff;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Misc;
using File = Alphaleonis.Win32.Filesystem.File;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Qiqqa.DocumentLibrary.WebLibraryStuff
{
    class WebLibraryManager
    {
        private static WebLibraryManager __instance = null;
        public static WebLibraryManager Instance
        {
            get
            {
                if (null == __instance)
                {
                    __instance = new WebLibraryManager();
                }
                return __instance;
            }
        }

        private Dictionary<string, WebLibraryDetail> web_library_details = new Dictionary<string, WebLibraryDetail>();
        private WebLibraryDetail guest_web_library_detail;

        public delegate void WebLibrariesChangedDelegate();
        public event WebLibrariesChangedDelegate WebLibrariesChanged;

        private WebLibraryManager()
        {   
            // Look for any web libraries that we know about
            LoadKnownWebLibraries(KNOWN_WEB_LIBRARIES_FILENAME, false);

            AddLocalGuestLibraryIfMissing();

            // *************************************************************************************************************
            // *** MIGRATION TO OPEN SOURCE CODE ***************************************************************************
            // *************************************************************************************************************
            AddLegacyWebLibrariesThatCanBeFoundOnDisk();
            // *************************************************************************************************************

            FireWebLibrariesChanged();
        }

        private void FireWebLibrariesChanged()
        {
            Logging.Info("+Notifying everyone that web libraries have changed");
            WebLibrariesChanged?.Invoke();
            Logging.Info("-Notifying everyone that web libraries have changed");
        }


        // *************************************************************************************************************
        // *** MIGRATION TO OPEN SOURCE CODE ***************************************************************************
        // *************************************************************************************************************
        private void AddLegacyWebLibrariesThatCanBeFoundOnDisk()
        {
            /**
             * Plan:
             * Iterate through all the folders in the Qiqqa data directory
             * If a folder contains a valid Library record and it is a WEB library, then add it to our list with the word '[LEGACY]' in front of it
             */

            string base_directory_path = UpgradePaths.V037To038.SQLiteUpgrade.BaseDirectoryForQiqqa;
            Logging.Info("Going to scan for web libraries at: {0}", base_directory_path);
            if (Directory.Exists(base_directory_path))
            {
                string[] library_directories = Directory.GetDirectories(base_directory_path);
                foreach (string library_directory in library_directories)
                {
                    Logging.Info("Inspecting directory {0}", library_directory);

                    string databaselist_file = Path.GetFullPath(Path.Combine(library_directory, @"Qiqqa.known_web_libraries"));
                    if (File.Exists(databaselist_file))
                    {
                        LoadKnownWebLibraries(databaselist_file, true);
                    }
                }

                foreach (string library_directory in library_directories)
                {
                    Logging.Info("Inspecting directory {0}", library_directory);

                    string database_file = Path.GetFullPath(Path.Combine(library_directory, @"Qiqqa.library"));
                    if (File.Exists(database_file))
                    {
                        var library_id = Path.GetFileName(library_directory);
                        if (web_library_details.ContainsKey(library_id))
                        {
                            Logging.Info("We already know about this library, so skipping legacy locate: {0}", library_id);
                            continue;
                        }

                        WebLibraryDetail new_web_library_detail = new WebLibraryDetail();

                        new_web_library_detail.Id = library_id;
                        new_web_library_detail.Title = "Legacy Web Library - " + new_web_library_detail.Id.Substring(0, new_web_library_detail.Id.Length);
                        new_web_library_detail.IsReadOnly = false;
                        UpdateKnownWebLibrary(new_web_library_detail);
                    }
                }
            }
        }
        // *************************************************************************************************************

        private void AddLocalGuestLibraryIfMissing()
        {
            // Check if we have an existing Guest library
            foreach (var pair in web_library_details)
            {
                if (pair.Value.IsLocalGuestLibrary)
                {
                    guest_web_library_detail = pair.Value;
                    break;
                }
            }

            // If we did not have a guest library, create one...
            if (null == guest_web_library_detail)
            {
                WebLibraryDetail new_web_library_detail = new WebLibraryDetail();
                new_web_library_detail.Id = "Guest";
                new_web_library_detail.Title = "Local Guest Library";
                new_web_library_detail.Description = "This is the library that comes with your Qiqqa guest account.";
                new_web_library_detail.Deleted = false;
                new_web_library_detail.IsLocalGuestLibrary = true;

                new_web_library_detail.library = new Library(new_web_library_detail);
                web_library_details[new_web_library_detail.Id] = new_web_library_detail;

                // Store this reference to guest
                guest_web_library_detail = new_web_library_detail;
            }

            // Import the Qiqqa manuals in the background, waiting until the library has loaded...
            SafeThreadPool.QueueUserWorkItem(o =>
            {
                while (!guest_web_library_detail.library.LibraryIsLoaded)
                {
                    if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                    {
                        return;
                    }
                    System.Threading.Thread.Sleep(500);
                }
                    
                QiqqaManualTools.AddManualsToLibrary(guest_web_library_detail.library);
            });
        }

        public WebLibraryDetail WebLibraryDetails_Guest
        {
            get
            {
                return guest_web_library_detail;
            }
        }

        public Library Library_Guest
        {
            get
            {
                return guest_web_library_detail.library;
            }
        }

        public bool HaveOnlyLocalGuestLibrary()
        {
            bool have_only_local_guest_library = true;
            foreach (WebLibraryDetail wld in WebLibraryDetails_All_IncludingDeleted)
            {
                if (!wld.IsLocalGuestLibrary) have_only_local_guest_library = false;
            }
            return have_only_local_guest_library;
        }

        public bool HaveOnlyOneWebLibrary()
        {
            return 1 == WebLibraryDetails_WorkingWebLibraries.Count;
        }


        /// <summary>
        /// Returns all working web libraries.  If the user has a web library, guest and deleted libraries are not in this list.
        /// If they have only a guest library, then this list is empty...
        /// </summary>
        public List<WebLibraryDetail> WebLibraryDetails_WorkingWebLibrariesWithoutGuest
        {
            get
            {
                List<WebLibraryDetail> details = new List<WebLibraryDetail>();
                foreach (WebLibraryDetail wld in WebLibraryDetails_All_IncludingDeleted)
                {
                    if (!wld.Deleted && !wld.IsLocalGuestLibrary)
                    {
                        details.Add(wld);
                    }
                }

                return details;
            }
        }
        
        /// <summary>
        /// Returns all working web libraries.  If the user has a web library, guest and deleted libraries are not in this list.
        /// If they have only a guest library, then it is in this list...
        /// </summary>
        public List<WebLibraryDetail> WebLibraryDetails_WorkingWebLibraries
        {
            get
            {
                List<WebLibraryDetail> details = WebLibraryDetails_WorkingWebLibrariesWithoutGuest;

                // If they don't have any real libraries, throw in the guest library
                if (0 == details.Count)
                {
                    details.Add(WebLibraryDetails_Guest);
                }

                return details;
            }
        }

        /// <summary>
        /// Returns all working web libraries, including the guest library.
        /// </summary>
        public List<WebLibraryDetail> WebLibraryDetails_WorkingWebLibraries_All
        {
            get
            {
                List<WebLibraryDetail> details = new List<WebLibraryDetail>();
                foreach (WebLibraryDetail wld in WebLibraryDetails_All_IncludingDeleted)
                {
                    if (!wld.Deleted && !wld.IsLocalGuestLibrary)
                    {
                        details.Add(wld);
                    }
                }

                // Always add the guest library
                details.Add(WebLibraryDetails_Guest);

                return details;
            }
        }

        public List<WebLibraryDetail> WebLibraryDetails_All_IncludingDeleted
        {
            get
            {
                List<WebLibraryDetail> details = new List<WebLibraryDetail>();
                details.AddRange(web_library_details.Values);
                return details;
            }
        }

        public Library GetLibrary(string library_id)
        {
            WebLibraryDetail web_library_detail;
            if (web_library_details.TryGetValue(library_id, out web_library_detail))
            {
                return GetLibrary(web_library_detail);
            }
            else
            {
                return null;
            }
        }

        public Library GetLibrary(WebLibraryDetail web_library_detail)
        {
            return web_library_detail.library;
        }

        public void NotifyOfChangeToWebLibraryDetail()
        {
            SaveKnownWebLibraries();
        }

        public void SortWebLibraryDetailsByLastAccessed(List<WebLibraryDetail> web_library_details)
        {
            string last_open_ordering = ConfigurationManager.Instance.ConfigurationRecord.GUI_LastSelectedLibraryId;

            // Is there nothing to do?
            if (String.IsNullOrEmpty(last_open_ordering))
            {
                return;
            }

            web_library_details.Sort(
                delegate(WebLibraryDetail a, WebLibraryDetail b)
                {
                    if (a == b) return 0;

                    if (b.Deleted) return -1;
                    if (a.Deleted) return +1;

                    if (b.IsLocalGuestLibrary) return -1;
                    if (a.IsLocalGuestLibrary) return +1;

                    int pos_b = last_open_ordering.IndexOf(b.Id);
                    if (-1 == pos_b) return -1;
                    int pos_a = last_open_ordering.IndexOf(a.Id);
                    if (-1 == pos_a) return +1;

                    return Sorting.Compare(pos_a, pos_b);
                }
            );
        }

        #region --- Known web library management -------------------------------------------------------------------------------------------------------------------------

        public static string KNOWN_WEB_LIBRARIES_FILENAME
        {
            get
            {
                return Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.BaseDirectoryForUser, @"Qiqqa.known_web_libraries"));
            }
        }

        void LoadKnownWebLibraries(string filename, bool only_load_those_libraries_which_are_actually_present)
        {
            Logging.Info("+Loading known Web Libraries");
            try
            {
                if (File.Exists(filename))
                {
                    KnownWebLibrariesFile known_web_libraries_file = SerializeFile.ProtoLoad<KnownWebLibrariesFile>(filename);
                    if (null != known_web_libraries_file.web_library_details)
                    {
                        foreach (WebLibraryDetail new_web_library_detail in known_web_libraries_file.web_library_details)
                        {
                            Logging.Info("We have known details for library '{0}' ({1})", new_web_library_detail.Title, new_web_library_detail.Id);

                            if (!new_web_library_detail.IsPurged)
                            {
                                // Intranet libraries have their readonly flag set on the user's current premium status...
                                if (new_web_library_detail.IsIntranetLibrary)
                                {
                                    new_web_library_detail.IsReadOnly = false;
                                }

                                string libdir_path = Library.GetLibraryBasePathForId(new_web_library_detail.Id);
                                string libfile_path = Path.GetFullPath(Path.Combine(libdir_path, @"Qiqqa.library"));

                                if (File.Exists(libfile_path) || !only_load_those_libraries_which_are_actually_present)
                                {
                                    new_web_library_detail.library = new Library(new_web_library_detail);
                                    web_library_details[new_web_library_detail.Id] = new_web_library_detail;
                                }
                                else
                                {
                                    Logging.Info("Not loading library {0} with Id {1} as it does not exist on disk.", new_web_library_detail.Title, new_web_library_detail.Id);
                                }
                            }
                            else
                            {
                                Logging.Info("Not loading purged library {0} with id {1}", new_web_library_detail.Title, new_web_library_detail.Id);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem loading the known Web Libraries from config file {0}", filename);
            }
            Logging.Info("-Loading known Web Libraries");
        }

        void SaveKnownWebLibraries(string filename = null)
        {
            if (null == filename)
            {
                filename = KNOWN_WEB_LIBRARIES_FILENAME;
            }

            Logging.Info("+Saving known Web Libraries to {0}", filename);

            try
            {
                KnownWebLibrariesFile known_web_libraries_file = new KnownWebLibrariesFile();
                known_web_libraries_file.web_library_details = new List<WebLibraryDetail>();
                foreach (WebLibraryDetail web_library_detail in web_library_details.Values)
                {
                    // *************************************************************************************************************
                    // *** MIGRATION TO OPEN SOURCE CODE ***************************************************************************
                    // *************************************************************************************************************
                    // Don't remember the web libraries - let them be discovered by this
                    if ("UNKNOWN" == web_library_detail.LibraryType())
                    {
                        continue;
                    }
                    // *************************************************************************************************************

                    known_web_libraries_file.web_library_details.Add(web_library_detail);
                }
                SerializeFile.ProtoSave<KnownWebLibrariesFile>(filename, known_web_libraries_file);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem saving the known web libraries to file {0}", filename);
            }

            Logging.Info("-Saving known Web Libraries to {0}", filename);
        }

        private void UpdateKnownWebLibrary(WebLibraryDetail new_web_library_detail)
        {
            new_web_library_detail.library = new Library(new_web_library_detail);
            web_library_details[new_web_library_detail.Id] = new_web_library_detail;

            SaveKnownWebLibraries();
            FireWebLibrariesChanged();
        }

        public void UpdateKnownWebLibraryFromIntranet(string intranet_path)
        {
            Logging.Info("+Updating known Intranet Library from {0}", intranet_path);

            IntranetLibraryDetail intranet_library_detail = IntranetLibraryDetail.Read(IntranetLibraryTools.GetLibraryDetailPath(intranet_path));

            WebLibraryDetail new_web_library_detail = new WebLibraryDetail();
            new_web_library_detail.IntranetPath = intranet_path;
            new_web_library_detail.Id = intranet_library_detail.Id;
            new_web_library_detail.Title = intranet_library_detail.Title;
            new_web_library_detail.Description = intranet_library_detail.Description;
            new_web_library_detail.IsReadOnly = false;
            new_web_library_detail.Deleted = false;

            UpdateKnownWebLibrary(new_web_library_detail);

            Logging.Info("-Updating known Intranet Library from {0}", intranet_path);
        }

        public WebLibraryDetail UpdateKnownWebLibraryFromBundleLibraryManifest(BundleLibraryManifest manifest)
        {
            Logging.Info("+Updating known Bundle Library {0} ({1})", manifest.Title, manifest.Id);

            WebLibraryDetail new_web_library_detail = new WebLibraryDetail();
            new_web_library_detail.BundleManifestJSON = manifest.ToJSON();
            new_web_library_detail.Id = manifest.Id;
            new_web_library_detail.Title = manifest.Title;
            new_web_library_detail.Description = manifest.Description;
            new_web_library_detail.IsReadOnly = true;
            new_web_library_detail.Deleted = false;

            UpdateKnownWebLibrary(new_web_library_detail);

            Logging.Info("-Updating known Bundle Library {0} ({1})", manifest.Title, manifest.Id);

            return new_web_library_detail;
        }

        internal void ForgetKnownWebLibraryFromIntranet(WebLibraryDetail web_library_detail)
        {
            Logging.Info("+Forgetting known Intranet Library from {0}", web_library_detail.Title);

            if (MessageBoxes.AskQuestion("Are you sure you want to forget the Intranet Library '{0}'?", web_library_detail.Title))
            {
                web_library_details.Remove(web_library_detail.Id);
                SaveKnownWebLibraries();
                FireWebLibrariesChanged();
            }

            Logging.Info("-Forgetting known Intranet Library from {0}", web_library_detail.Title);
        }

        #endregion
    }
}
