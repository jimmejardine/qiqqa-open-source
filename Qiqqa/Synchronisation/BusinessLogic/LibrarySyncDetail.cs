using System;
using Qiqqa.DocumentLibrary.WebLibraryStuff;

namespace Qiqqa.Synchronisation.BusinessLogic
{
    /// <summary>
    /// Is contained in a GlobalSyncDetail - one per web library.
    /// Contains all the sync information for a single library.
    /// </summary>
    [Serializable]
    internal class LibrarySyncDetail
    {
        private static WebLibraryDetail WebLibraryDetail_DEFAULT = new WebLibraryDetail();

        public WebLibraryDetail web_library_detail = WebLibraryDetail_DEFAULT;
        public LocalLibrarySyncDetail local_library_sync_detail;
        public SyncDecision sync_decision;

        /// <summary>
        /// Is contained in a LibrarySyncDetail.
        /// Contains the size of all files in the library on local disk.
        /// </summary>
        [Serializable]
        public class LocalLibrarySyncDetail
        {
            public int total_files_in_library = 0;
            public int total_files_in_library_deleted = 0;
            public long total_library_size = 0;
        }

        [Serializable]
        public class SyncDecision
        {
            public bool can_sync;
            public bool can_sync_metadata;

            public bool is_readonly;
        }
    }
}
