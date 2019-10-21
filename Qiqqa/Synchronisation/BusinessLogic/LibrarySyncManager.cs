using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.MessageBoxControls;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Main.LoginStuff;
using Qiqqa.Synchronisation.GUI;
using Qiqqa.Synchronisation.MetadataSync;
using Qiqqa.Synchronisation.PDFSync;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Synchronisation.BusinessLogic
{
    public class LibrarySyncManager
    {
        public class SyncRequest
        {
            public bool suppress_already_in_progress_notification;
            public bool wants_user_intervention;
            public List<Library> libraries_to_sync;
            public bool sync_metadata;
            public bool sync_pdfs;

            /**
             * This will prompt the user what to sync
             */
            public SyncRequest(bool wants_user_intervention, bool sync_metadata, bool sync_pdfs, bool suppress_already_in_progress_notification) :
                this(wants_user_intervention, new List<Library>(), sync_metadata, sync_pdfs, suppress_already_in_progress_notification)
            {
            }

            /**
             * This will autotick just the specified library.
             */
            public SyncRequest(bool wants_user_intervention, Library library, bool sync_metadata, bool sync_pdfs, bool suppress_already_in_progress_notification) : 
                this(wants_user_intervention, new List<Library>(new Library[] {library}), sync_metadata, sync_pdfs, suppress_already_in_progress_notification)
            {
            }

            /**
             * This will autotick just the specified libraries.
             */
            public SyncRequest(bool wants_user_intervention, List<Library> libraries_to_sync, bool sync_metadata, bool sync_pdfs, bool suppress_already_in_progress_notification)
            {
                this.wants_user_intervention = wants_user_intervention;
                this.libraries_to_sync = libraries_to_sync;
                this.sync_metadata = sync_metadata;
                this.sync_pdfs = sync_pdfs;
                this.suppress_already_in_progress_notification = suppress_already_in_progress_notification;
            }
        }

        public static readonly LibrarySyncManager Instance = new LibrarySyncManager();

        private LibrarySyncManager()
        {
        }

        internal void RefreshSyncControl(SyncControlGridItemSet scgis_previous, SyncControl sync_control)
        {
            GlobalSyncDetail global_sync_detail = GenerateGlobalSyncDetail();
            SyncControlGridItemSet scgis = new SyncControlGridItemSet(scgis_previous.sync_request, global_sync_detail);
            scgis.AutoTick();
            sync_control.SetSyncParameters(scgis);
        }

        public void RequestSync(SyncRequest sync_request)
        {
            bool user_wants_intervention = KeyboardTools.IsCTRLDown() || !ConfigurationManager.Instance.ConfigurationRecord.SyncTermsAccepted;

            GlobalSyncDetail global_sync_detail = GenerateGlobalSyncDetail();
            SyncControlGridItemSet scgis = new SyncControlGridItemSet(sync_request, global_sync_detail);
            scgis.AutoTick();

            if (scgis.CanRunWithoutIntervention() && !user_wants_intervention)
            {
                Sync(scgis);
            }
            else
            {
                SyncControl sync_control = new SyncControl();
                sync_control.SetSyncParameters(scgis);
                sync_control.Show();
            }
        }

        internal GlobalSyncDetail GenerateGlobalSyncDetail()
        {
            try
            {
                GlobalSyncDetail global_sync_detail = new GlobalSyncDetail();

                // Build the initial knowledge of our libraries
                StatusManager.Instance.UpdateStatusBusy(StatusCodes.SYNC_META_GLOBAL, "Getting your library details", 0, 1);
                List<WebLibraryDetail> web_library_details = WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries_All;
                foreach (WebLibraryDetail web_library_detail in web_library_details)
                {
                    // Create the new LibrarySyncDetail
                    LibrarySyncDetail library_sync_detail = new LibrarySyncDetail();
                    global_sync_detail.library_sync_details.Add(library_sync_detail);

                    // Store our knowledge about the web library detail
                    library_sync_detail.web_library_detail = web_library_detail;
                }

                // Now get the local details of each library
                {
                    StatusManager.Instance.UpdateStatusBusy(StatusCodes.SYNC_META_GLOBAL, String.Format("Gather local library details"));
                    for (int i = 0; i < global_sync_detail.library_sync_details.Count; ++i)
                    {
                        LibrarySyncDetail library_sync_detail = global_sync_detail.library_sync_details[i];
                        StatusManager.Instance.UpdateStatusBusy(StatusCodes.SYNC_META_GLOBAL, String.Format("Getting the local library details for {0}", library_sync_detail.web_library_detail.Title), i, global_sync_detail.library_sync_details.Count);
                        library_sync_detail.local_library_sync_detail = GetLocalLibrarySyncDetail(library_sync_detail.web_library_detail.library);
                    }
                }
                
                // Work out if they are allowed to sync this record
                StatusManager.Instance.UpdateStatusBusy(StatusCodes.SYNC_META_GLOBAL, String.Format("Determining sync allowances"));
                foreach (LibrarySyncDetail library_sync_detail in global_sync_detail.library_sync_details)
                {
                    //  Eagerly sync metadata
                    library_sync_detail.sync_decision = new LibrarySyncDetail.SyncDecision();
                    library_sync_detail.sync_decision.can_sync_metadata = true;

                    // Else initiator
                    if (false) { }

                    // Never guests
                    else if (library_sync_detail.web_library_detail.IsLocalGuestLibrary)
                    {
                        library_sync_detail.sync_decision.can_sync = false;
                        library_sync_detail.sync_decision.can_sync_metadata = false;
                        library_sync_detail.sync_decision.is_readonly = library_sync_detail.web_library_detail.IsReadOnly;
                    }

                    // Intranets are dependent on premium
                    else if (library_sync_detail.web_library_detail.IsIntranetLibrary)
                    {
                        library_sync_detail.sync_decision.can_sync = true;
                        library_sync_detail.sync_decision.can_sync_metadata = true;
                        library_sync_detail.sync_decision.is_readonly = library_sync_detail.web_library_detail.IsReadOnly;
                    }

                    // Bundles can never sync
                    else if (library_sync_detail.web_library_detail.IsBundleLibrary)
                    {
                        library_sync_detail.sync_decision.can_sync = false;
                        library_sync_detail.sync_decision.can_sync_metadata = false;
                        library_sync_detail.sync_decision.is_readonly = library_sync_detail.web_library_detail.IsReadOnly;
                    }

                    // Unknown library types
                    else
                    {
                        library_sync_detail.sync_decision.can_sync = false;
                        library_sync_detail.sync_decision.can_sync_metadata = false;
                        library_sync_detail.sync_decision.is_readonly = true;
                    }
                }

                return global_sync_detail;
            }
            finally
            {
                // Clear the status bar
                StatusManager.Instance.ClearStatus(StatusCodes.SYNC_META_GLOBAL);
            }
        }


        private static LibrarySyncDetail.LocalLibrarySyncDetail GetLocalLibrarySyncDetail(Library library)
        {
            LibrarySyncDetail.LocalLibrarySyncDetail local_library_sync_detail = new LibrarySyncDetail.LocalLibrarySyncDetail();

            List<PDFDocument> pdf_documents = library.PDFDocuments_IncludingDeleted;

            foreach (PDFDocument pdf_document in pdf_documents)
            {
                try
                {
                    ++local_library_sync_detail.total_files_in_library;

                    // Don't tally the deleted documents
                    bool deleted = pdf_document.Deleted;
                    if (deleted)
                    {
                        ++local_library_sync_detail.total_files_in_library_deleted;
                        continue;
                    }

                    // We can only really tally up the documents that exist locally
                    if (pdf_document.DocumentExists)
                    {
                        long document_size = (new FileInfo(pdf_document.DocumentPath)).Length;
                        local_library_sync_detail.total_library_size += document_size;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was an error tallying up the local library sync detail for '{0}'.", pdf_document.Fingerprint);
                }
            }

            return local_library_sync_detail;
        }


        // -------------------------------------------------------------------------------------------------------------------------------------------------------

        internal void Sync(SyncControlGridItemSet sync_control_grid_item_set)
        {
            Logging.Info("Syncing");

            foreach (SyncControlGridItem sync_control_grid_item_temp in sync_control_grid_item_set.grid_items)
            {
                // Needed for passing to background thread...
                SyncControlGridItem sync_control_grid_item = sync_control_grid_item_temp;

                if (sync_control_grid_item.library_sync_detail.web_library_detail.library.sync_in_progress)
                {
                    if (!sync_control_grid_item_set.sync_request.suppress_already_in_progress_notification)
                    {
                        MessageBoxes.Info("A sync operation is already in progress for library {0}.  Please wait for it to finish before trying to sync again.", sync_control_grid_item.library_sync_detail.web_library_detail.library.WebLibraryDetail.Title);
                    }
                    else
                    {
                        Logging.Info("A sync operation is already in progress for library {0}.  This has been suppressed from the GUI.", sync_control_grid_item.library_sync_detail.web_library_detail.library.WebLibraryDetail.Title);
                    }
                }
                else
                {
                    if (sync_control_grid_item.SyncMetadata || sync_control_grid_item.SyncDocuments)
                    {
                        sync_control_grid_item.library_sync_detail.web_library_detail.library.sync_in_progress = true;
                        SafeThreadPool.QueueUserWorkItem(o => Sync_BACKGROUND(sync_control_grid_item));
                    }
                }
            }
        }

        private void Sync_BACKGROUND(SyncControlGridItem sync_control_grid_item)
        {
            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(sync_control_grid_item.library_sync_detail.web_library_detail.library), String.Format("Starting sync of {0}", sync_control_grid_item.LibraryTitle));

            try
            {
                if (sync_control_grid_item.SyncMetadata)
                {
                    if (sync_control_grid_item.library_sync_detail.sync_decision.can_sync)
                    {
                        Logging.Info("Syncing metadata for {0}", sync_control_grid_item.library_sync_detail.web_library_detail.Title);
                        SynchronizeMetadata_INTERNAL_BACKGROUND(sync_control_grid_item.library_sync_detail.web_library_detail.library, false, sync_control_grid_item.IsReadOnly);
                        SynchronizeDocuments_Upload_INTERNAL_BACKGROUND(sync_control_grid_item.library_sync_detail.web_library_detail.library, sync_control_grid_item.library_sync_detail.web_library_detail.library.PDFDocuments, sync_control_grid_item.IsReadOnly);
                    }
                    else
                    {
                        Logging.Info("Partial syncing metadata for {0}", sync_control_grid_item.library_sync_detail.web_library_detail.Title);
                        SynchronizeMetadata_INTERNAL_BACKGROUND(sync_control_grid_item.library_sync_detail.web_library_detail.library, true, sync_control_grid_item.IsReadOnly);

                        Logging.Info("Not uploading documents for {0}", sync_control_grid_item.library_sync_detail.web_library_detail.Title);
                    }
                }

                if (sync_control_grid_item.SyncDocuments)
                {
                    if (sync_control_grid_item.library_sync_detail.sync_decision.can_sync)
                    {
                        Logging.Info("Downloading documents for {0}", sync_control_grid_item.library_sync_detail.web_library_detail.Title);
                        SynchronizeDocuments_Download_INTERNAL_BACKGROUND(sync_control_grid_item.library_sync_detail.web_library_detail.library, sync_control_grid_item.library_sync_detail.web_library_detail.library.PDFDocuments, sync_control_grid_item.IsReadOnly);
                    }
                    else
                    {
                        Logging.Info("Not downloading documents for {0}", sync_control_grid_item.library_sync_detail.web_library_detail.Title);
                    }
                }
            }
            finally
            {
                sync_control_grid_item.library_sync_detail.web_library_detail.library.sync_in_progress = false;

                // Indicate that we have synced
                sync_control_grid_item.library_sync_detail.web_library_detail.library.WebLibraryDetail.LastSynced = DateTime.UtcNow;
                WebLibraryManager.Instance.NotifyOfChangeToWebLibraryDetail();
            }

            StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_META(sync_control_grid_item.library_sync_detail.web_library_detail.library), String.Format("Finished sync of {0}", sync_control_grid_item.LibraryTitle));
        }

        private void SynchronizeMetadata_INTERNAL_BACKGROUND(Library library, bool restricted_metadata_sync, bool is_readonly)
        {
            Dictionary<string, string> historical_sync_file = HistoricalSyncFile.GetHistoricalSyncFile(library);
            try
            {
                SynchronisationStates ss = SynchronisationStateBuilder.Build(library, historical_sync_file);
                SynchronisationAction sa = SynchronisationActionBuilder.Build(library, ss);
                SynchronisationExecutor.Sync(library, restricted_metadata_sync, is_readonly, historical_sync_file, sa);
            }
            catch (Exception ex)
            {
                UnhandledExceptionMessageBox.DisplayException(ex);
            }

            HistoricalSyncFile.PutHistoricalSyncFile(library, historical_sync_file);
        }

        private void SynchronizeDocuments_Download_INTERNAL_BACKGROUND(Library library, List<PDFDocument> pdf_documents, bool is_readonly)
        {
            int total_downloads_requested = 0;

            foreach (PDFDocument pdf_document in pdf_documents)
            {
                if (pdf_document.Deleted) continue;
                if (pdf_document.DocumentExists) continue;
                if (pdf_document.IsVanillaReference) continue;

                ++total_downloads_requested;
                SyncQueues.Instance.QueueGet(pdf_document.Fingerprint, library);
            }

            Logging.Info("Queueing {0} PDF download requests.", total_downloads_requested);
        }
        
        private void SynchronizeDocuments_Upload_INTERNAL_BACKGROUND(Library library, List<PDFDocument> pdf_documents, bool is_readonly)
        {
            // TODO: Replace this with a pretty interface class ------------------------------------------------
            if (is_readonly)
            {
                // Do nothing...
                Logging.Info("Not queueing upload of PDFs for readonly library.");
            }
            else if (library.WebLibraryDetail.IsIntranetLibrary)
            {
                SyncQueues_Intranet.QueueUploadOfMissingPDFs(library, pdf_documents);
            }
            else
            {
                throw new Exception(String.Format("Did not understand how to queue upload PDFs for library {0}", library.WebLibraryDetail.Title));
            }
            // -----------------------------------------------------------------------------------------------------
        }

        public void QueuePut(Library library, string[] fingerprints)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Sync_SyncUploadSinglePDF);

            foreach (string fingerprint in fingerprints)
            {
                SyncQueues.Instance.QueuePut(fingerprint, library);
            }
        }

        public void QueueGet(Library library, string[] fingerprints)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Sync_SyncDownloadSinglePDF);

            foreach (string fingerprint in fingerprints)
            {
                SyncQueues.Instance.QueueGet(fingerprint, library);
            }
        }
    }
}
