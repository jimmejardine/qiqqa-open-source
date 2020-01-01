using System;
using System.Collections.Generic;
using System.Threading;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Maintainable;
using Utilities.Misc;

namespace Qiqqa.Documents.Common
{
    public class DocumentQueuedStorer
    {
        public static DocumentQueuedStorer Instance = new DocumentQueuedStorer();
        private PeriodTimer period_flush = new PeriodTimer(new TimeSpan(0, 0, 1));
        private object documents_to_store_lock = new object();
        private Dictionary<string, PDFDocument> documents_to_store = new Dictionary<string, PDFDocument>();

        protected DocumentQueuedStorer()
        {
            MaintainableManager.Instance.RegisterHeldOffTask(DoMaintenance_FlushDocuments, 30 * 1000, ThreadPriority.BelowNormal);
            // Quit this delayed storing of PDF files when we've hit the end of the execution run: 
            // we'll have to save them all to disk in one go then, and quickly too!
            Utilities.Shutdownable.ShutdownableManager.Instance.Register(Shutdown);
        }

        private void DoMaintenance_FlushDocuments(Daemon daemon)
        {
            if (Qiqqa.Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
            {
                // do run the flush task, but delayed!
                daemon.Sleep(5 * 60 * 1000);
            }

            // Quit this delayed storing of PDF files when we've hit the end of the excution run: 
            // we'll have to save them all to disk in one go then, and quickly too!
            if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown || period_flush.Expired)
            {
                period_flush.Signal();
                FlushDocuments(false);
            }
        }

        private void Shutdown()
        {
            // **forced** flush!
            FlushDocuments(true);
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

        private void FlushDocuments(bool force_flush_no_matter_what)
        {
            // use a lock to ensure the time-delayed flush doesn't ever collide with the
            // end-of-execution-run flush initiated by ShutdownableManager.
            ForcedFlushRequested = force_flush_no_matter_what;

            while (true)
            {
                int count_to_go = PendingQueueCount;

                if (0 < count_to_go)
                {
                    StatusManager.Instance.UpdateStatus("DocumentQueuedStorer", String.Format("{0} documents still to flush", count_to_go), 1, count_to_go);
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
                        return;
                    }

                    // Relinquish control to the UI thread to make sure responsiveness remains tolerable at 100% CPU load.
                    Utilities.GUI.WPFDoEvents.WaitForUIThreadActivityDone();
                }

                PDFDocument pdf_document_to_flush = null;

                // grab one PDF to save/flush:
                Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                lock (documents_to_store_lock)
                {
                    l2_clk.LockPerfTimerStop();
                    foreach (var pair in documents_to_store)
                    {
                        pdf_document_to_flush = pair.Value;
                        documents_to_store.Remove(pair.Key);
                        break;
                    }
                }

                if (null != pdf_document_to_flush)
                {
                    pdf_document_to_flush.SaveToMetaData();
                }
            }
        }

        public void Queue(PDFDocument pdf_document)
        {
            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (documents_to_store_lock)
            {
                l1_clk.LockPerfTimerStop();
                documents_to_store[pdf_document.Library.WebLibraryDetail.Id + "." + pdf_document.Fingerprint] = pdf_document;
            }
        }

        public int PendingQueueCount
        {
            get
            {
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (documents_to_store_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    return documents_to_store.Count;
                }
            }
        }
    }
}
