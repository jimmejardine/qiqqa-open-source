using System;
using System.Collections.Generic;
using System.Windows;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Synchronisation.BusinessLogic;
using Utilities;

namespace Qiqqa.DocumentLibrary.AutoSyncStuff
{
    class AutoSyncManager
    {
        public static AutoSyncManager Instance = new AutoSyncManager();

        private AutoSyncManager()
        {
        }

        internal void DoMaintenance()
        {
            List<string> library_identifiers = new List<string>();
            List<WebLibraryDetail> web_library_details = WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibrariesWithoutGuest;

            List<Library> libraries_to_sync = new List<Library>();
            foreach (WebLibraryDetail web_library_detail in web_library_details)
            {
                // TODO - this needs to be written for intranet libraries
                //
                // The best way to do it might be, at the end of a sync, to write a timestamp 
                // to the shared folder at the end of a sync, and write that same timestamp to 
                // library.WebLibraryDetail.LastServerSyncNotificationDate
                //
                // That way, if the timestamp in the shared folder has been changed 
                // to what our client last saw, we know we need to sync.
                //
                if (web_library_detail.AutoSync)
                {
                    Library library = WebLibraryManager.Instance.GetLibrary(web_library_detail.Id);
#if TODO
                    if (_____TIMESTAMP_IN_SHARED_FOLDER_____ != library.WebLibraryDetail.LastServerSyncNotificationDate)
                    {
                        Logging.Info("Library {0} is in need of a sync - and has been flagged for autosync", library.WebLibraryDetail.Title);
                        libraries_to_sync.Add(library);
                    }
#endif
                }
            }

            if (0 < libraries_to_sync.Count)
            {
                Logging.Info("At least one library needs to autosync");
                WebLibraryManager.Instance.NotifyOfChangeToWebLibraryDetail();

                LibrarySyncManager.SyncRequest sync_request = new LibrarySyncManager.SyncRequest(false, libraries_to_sync, true, true, true);
                Application.Current.Dispatcher.Invoke(((Action)(() =>
                    LibrarySyncManager.Instance.RequestSync(sync_request)
                )));
            }
        }
    }
}