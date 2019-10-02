using System;
using System.Collections.Generic;
using System.Linq;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Utilities;
using Utilities.Misc;

namespace Qiqqa.Synchronisation.PDFSync
{
    /// <summary>
    /// SyncQueues manages sync of the PDFs themselves.
    /// </summary>
    internal class SyncQueues
    {
        class SyncQueueEntry
        {
            public string fingerprint;
            public Library library;

            public override string ToString()
            {
                return String.Format("{0}/{1}", library.WebLibraryDetail.Id, fingerprint);
            }
        }

        List<SyncQueueEntry> fingerprints_to_put = new List<SyncQueueEntry>();
        List<SyncQueueEntry> fingerprints_to_get = new List<SyncQueueEntry>();

        public static readonly SyncQueues Instance = new SyncQueues();

        private SyncQueues()
        {
        }
        
        #region --- Public queueing --------------------------------------------------------------------------------------------------------

        internal void QueuePut(string fingerprint, Library library)
        {
            //We never send vanilla ref PDFs to s3.
            if (VanillaReferenceCreating.IsVanillaReferenceFingerprint(fingerprint)) return;

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (fingerprints_to_put)
            {
                l1_clk.LockPerfTimerStop();
                fingerprints_to_put.Add(new SyncQueueEntry { fingerprint = fingerprint, library = library });
            }
        }

        internal void QueueGet(string fingerprint, Library library)
        {
            // We never fetch vanilla ref PDFs from S3.
            if (VanillaReferenceCreating.IsVanillaReferenceFingerprint(fingerprint)) return;

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (fingerprints_to_get)
            {
                l1_clk.LockPerfTimerStop();
                fingerprints_to_get.Add(new SyncQueueEntry { fingerprint = fingerprint, library = library });
            }
        }

        internal void DoMaintenance(Daemon daemon)
        {
            bool did_some_transfers = false;

            try
            {
                DaemonEntryPut(daemon, ref did_some_transfers);
                DaemonEntryGet(daemon, ref did_some_transfers);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was an exception while syncing the PDFs");
            }

            if (did_some_transfers)
            {
                StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_DATA, "Finished paper sync.");
            }
        }

        #endregion

        #region --- Put --------------------------------------------------------------------------------------------------------

        private void DaemonEntryPut(Daemon daemon, ref bool did_some_transfers)
        {
            StatusManager.Instance.ClearCancelled(StatusCodes.SYNC_DATA);
            while (daemon.StillRunning)
            {
                // Is there another fingerprint to process?
                int fingerprints_remaining;
                SyncQueueEntry sync_queue_entry = GetNextSyncQueueEntry(fingerprints_to_put, out fingerprints_remaining);
                if (null == sync_queue_entry)
                {
                    break;
                }

                StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_DATA, String.Format("Uploading paper to your Web/Intranet Library ({0} to go)", fingerprints_remaining), 1, fingerprints_remaining, true);

                // Check if the user cancelled this download
                bool user_cancelled_transfer = StatusManager.Instance.IsCancelled(StatusCodes.SYNC_DATA);
                if (user_cancelled_transfer)
                {
                    Logging.Info("User cancelled documents upload");
                    ClearSyncQueue(fingerprints_to_put);
                }

                try
                {
                    // TODO: Replace this with a pretty interface class ------------------------------------------------
                    if (sync_queue_entry.library.WebLibraryDetail.IsIntranetLibrary)
                    {
                        SyncQueues_Intranet.DaemonPut(sync_queue_entry.library, sync_queue_entry.fingerprint);
                    }
                    else
                    {
                        throw new Exception(String.Format("Did not understand how to transfer PDFs for library {0}", sync_queue_entry.library.WebLibraryDetail.Title));
                    }
                    // -----------------------------------------------------------------------------------------------------
                    
                    did_some_transfers = true;
                }
                catch (Exception ex)
                {                    
                    Logging.Error(ex, "Exception while uploading paper {0}", sync_queue_entry);
                }
                StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_DATA, "Uploaded papers to your Web/Intranet Library");
            }
        }

        #endregion

        #region --- Get --------------------------------------------------------------------------------------------------------

        private void DaemonEntryGet(Daemon daemon, ref bool did_some_transfers)        
        {
            StatusManager.Instance.ClearCancelled(StatusCodes.SYNC_DATA);
            while (daemon.StillRunning)
            {
                // Is there another fingerprint to process?
                int fingerprints_remaining;
                SyncQueueEntry sync_queue_entry = GetNextSyncQueueEntry(fingerprints_to_get, out fingerprints_remaining);
                if (null == sync_queue_entry)
                {
                    break;
                }

                StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_DATA, String.Format("Downloading paper from your Web/Intranet Library ({0} to go)", fingerprints_remaining), 1, fingerprints_remaining, true);

                // Check if the user cancelled this download
                bool user_cancelled_transfer = StatusManager.Instance.IsCancelled(StatusCodes.SYNC_DATA);
                if (user_cancelled_transfer)
                {
                    Logging.Info("User cancelled documents download");
                    ClearSyncQueue(fingerprints_to_get);
                }

                try
                {
                    // TODO: Replace this with a pretty interface class ------------------------------------------------
                    if (sync_queue_entry.library.WebLibraryDetail.IsIntranetLibrary)
                    {
                        SyncQueues_Intranet.DaemonGet(sync_queue_entry.library, sync_queue_entry.fingerprint);
                    }
                    else
                    {
                        throw new Exception(String.Format("Did not understand how to transfer PDFs for library {0}", sync_queue_entry.library.WebLibraryDetail.Title));
                    }
                    // -----------------------------------------------------------------------------------------------------
                    
                    did_some_transfers = true;
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Exception while downloading paper {0}", sync_queue_entry);
                }
                StatusManager.Instance.UpdateStatus(StatusCodes.SYNC_DATA, "Downloaded papers from your Web/Intranet Library");
            }
        }

        #endregion

        #region --- Common --------------------------------------------------------------------------------------------------------

        private static SyncQueueEntry GetNextSyncQueueEntry(List<SyncQueueEntry> fingerprints, out int fingerprints_remaining)
        {
            SyncQueueEntry sync_queue_entry = null;

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (fingerprints)
            {
                l1_clk.LockPerfTimerStop();
                fingerprints_remaining = fingerprints.Count;
                if (0 < fingerprints_remaining)
                {
                    sync_queue_entry = fingerprints.First();
                    fingerprints.Remove(sync_queue_entry);
                    --fingerprints_remaining;
                }
            }

            return sync_queue_entry;
        }

        private static void ClearSyncQueue(List<SyncQueueEntry> fingerprints)
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (fingerprints)
            {
                l1_clk.LockPerfTimerStop();
                fingerprints.Clear();
            }
        }

        #endregion
    }
}
