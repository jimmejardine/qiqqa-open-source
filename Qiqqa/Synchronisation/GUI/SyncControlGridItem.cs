using System;
using System.Diagnostics;
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
        public bool SyncLibrary { get; set; }

        public bool CanSyncLibrary => library_sync_detail.sync_decision.can_sync;

        public bool IsReadOnly => library_sync_detail.sync_decision.is_readonly;

        public string LibraryType => library_sync_detail.web_library_detail.LibraryType();

        public string LibraryTitle => library_sync_detail.web_library_detail.Title;

        public string SizeLocalString => String.Format("{0:N1}", SizeLocal / (1024.0 * 1024.0));

        public long SizeLocal => library_sync_detail.local_library_sync_detail.total_library_size;

        public string SyncTarget
        {
            get
            {
                return library_sync_detail.web_library_detail?.IntranetPath ?? "(null)";
            }
            set
            {
                if (library_sync_detail.web_library_detail != null && !String.IsNullOrWhiteSpace(value))
                {
                    // turn this library into an intranet library, if it isn't already:
                    if (library_sync_detail.web_library_detail.IsBundleLibrary)
                    {
                        throw new NotImplementedException("We don't yet support converting a Bundle library to an Intranet Library.");
                    }
                    // are we actually changing the path?
                    if (library_sync_detail.web_library_detail.IntranetPath != value)
                    {
                        library_sync_detail.web_library_detail.IntranetPath = value;
                        Debug.Assert(library_sync_detail.web_library_detail.IsIntranetLibrary);
                    }
#if false
                    SafeThreadPool.QueueUserWorkItem(o =>
                    {
                        bool validation_successful = EnsureIntranetLibraryExists(db_base_path, db_title, db_description);

                        if (validation_successful)
                        {
                            WPFDoEvents.InvokeInUIThread(() =>
                            {
                                Close();
                            });
                        }
                    });
#endif
                }
            }
        }

        public bool MayEditSyncTarget => !library_sync_detail.web_library_detail?.IsBundleLibrary ?? false;

        public string Id => library_sync_detail.web_library_detail?.Id ?? "(null)";
    }
}
