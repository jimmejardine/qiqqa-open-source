using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.FolderWatching;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.GUI;
using Utilities.Maintainable;
using Utilities.Misc;
using Utilities.Shutdownable;

namespace Qiqqa.Documents.Common
{
    public class DocumentQueuedStorer
    {
        public static DocumentQueuedStorer Instance = new DocumentQueuedStorer();
        private object documents_to_store_lock = new object();
        private Dictionary<string, PDFDocument> documents_to_store = new Dictionary<string, PDFDocument>();

        protected DocumentQueuedStorer()
        {
            MaintainableManager.Instance.RegisterHeldOffTask(DoMaintenance_FlushDocuments, 30 * 1000);
            // Quit this delayed storing of PDF files when we've hit the end of the execution run:
            // we'll have to save them all to disk in one go then, and quickly too!
            ShutdownableManager.Instance.Register(Shutdown);
        }

        private void DoMaintenance_FlushDocuments(Daemon daemon)
        {
            try
            {
                if (Qiqqa.Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
                {
                    // do run the flush task, but delayed!
                    return;
                }

                // Quit this delayed storing of PDF files when we've hit the end of the execution run:
                // we'll have to save them all to disk in one go then, and quickly too!
                if (!ShutdownableManager.Instance.IsShuttingDown)
                {
                    FlushDocuments(force_flush_no_matter_what: false);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Terminating the Flush-Documents background thread due to an otherwise unhandled exception.");
            }
        }

        private void Shutdown()
        {
            // **forced** flush!
            FlushDocuments(force_flush_no_matter_what: true);
        }

        private object flush_locker = new object();
        private bool _force_flush_requested = false;

        private bool ForcedFlushRequested
        {
            get
            {
                lock (flush_locker)
                {
                    return _force_flush_requested;
                }
            }
            set
            {
                lock (flush_locker)
                {
                    if (value)
                    {
                        _force_flush_requested = true;
                    }
                }
            }
        }

        public void FlushDocuments(bool force_flush_no_matter_what)
        {
            // use a lock to ensure the time-delayed flush doesn't ever collide with the
            // end-of-execution-run flush initiated by ShutdownableManager.
            ForcedFlushRequested = force_flush_no_matter_what;

            if (!force_flush_no_matter_what)
            {
                WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();
            }

            int done_count_for_status = 0;

            Stopwatch breathing_time = Stopwatch.StartNew();

            while (true)
            {
                int count_to_go = PendingQueueCount;
                int todo_count_for_status = done_count_for_status + count_to_go;

                if (0 < count_to_go)
                {
                    StatusManager.Instance.UpdateStatus("DocumentQueuedStorer", String.Format("{0}/{1} documents still to flush", count_to_go, todo_count_for_status), done_count_for_status, todo_count_for_status);
                }
                else
                {
                    StatusManager.Instance.ClearStatus("DocumentQueuedStorer");
                    return;
                }

                if (!ForcedFlushRequested)
                {
                    // No flushing while still adding... unless we're quitting the executable already.
                    if (Library.IsBusyAddingPDFs)
                    {
                        // wait a short while and then check again iff we're still adding PDF documents (bulk import apparently).
                        // If it isn't we'll just have delayed the flush by a few seconds.
                        ShutdownableManager.Sleep(FolderWatcher.MILLISECONDS_TO_RELAX_PER_ITERATION);

                        // reset:
                        breathing_time.Restart();

                        if (Library.IsBusyAddingPDFs)
                        {
                            continue;
                        }
                    }

                    // Relinquish control to the UI thread to make sure responsiveness remains tolerable at 100% CPU load.
                    if (breathing_time.ElapsedMilliseconds > FolderWatcher.MAX_MILLISECONDS_PER_ITERATION)
                    {
                        ShutdownableManager.Sleep(FolderWatcher.MILLISECONDS_TO_RELAX_PER_ITERATION);

                        // reset:
                        breathing_time.Restart();
                    }
                }

                PDFDocument pdf_document_to_flush = null;

                // grab one PDF to save/flush:
                // Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                lock (documents_to_store_lock)
                {
                    // l2_clk.LockPerfTimerStop();
                    foreach (var pair in documents_to_store)
                    {
                        pdf_document_to_flush = pair.Value;
                        documents_to_store.Remove(pair.Key);
                        break;
                    }
                }

                if (null != pdf_document_to_flush)
                {
                    pdf_document_to_flush.SaveToMetaData(ForcedFlushRequested);

                    done_count_for_status++;
                }
            }
        }

        public void Queue(PDFDocument pdf_document)
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (documents_to_store_lock)
            {
                // l1_clk.LockPerfTimerStop();
                documents_to_store[pdf_document.LibraryRef.Id + "." + pdf_document.Fingerprint] = pdf_document;
            }
        }

        public int PendingQueueCount
        {
            get
            {
                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (documents_to_store_lock)
                {
                    // l1_clk.LockPerfTimerStop();
                    return documents_to_store.Count;
                }
            }
        }
    }
}
