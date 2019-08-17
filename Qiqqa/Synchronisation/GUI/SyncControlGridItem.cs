using System;
using System.Reflection;
using Qiqqa.Synchronisation.BusinessLogic;

namespace Qiqqa.Synchronisation.GUI
{
    public class SyncControlGridItem
    {
        internal LibrarySyncDetail library_sync_detail;

        internal SyncControlGridItem(LibrarySyncDetail library_sync_detail)
        {
            this.library_sync_detail = library_sync_detail;
        }

        public bool UserRequestedSync { get; set; }
        public bool SyncMetadata { get; set; }
        public bool SyncDocuments { get; set; }

        public bool CanSyncMetadata
        {
            get
            {
                return library_sync_detail.sync_decision.can_sync_metadata;
            }
        }

        public bool CanSyncDocuments
        {
            get
            {
                return library_sync_detail.sync_decision.can_sync;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return library_sync_detail.sync_decision.is_readonly;
            }
        }

        public string LibraryType
        {
            get
            {
                return library_sync_detail.web_library_detail.LibraryType();
            }
        }
        
        public string LibraryTitle
        {
            get
            {
                return library_sync_detail.web_library_detail.Title;
            }
        }

        public string SizeLocalString
        {
            get
            {
                return String.Format("{0:N1}", SizeLocal / 1024.0 / 1024.0);
            }
        }

        public long SizeLocal
        {
            get
            {
                return library_sync_detail.local_library_sync_detail.total_library_size;
            }
        }
    }
}
