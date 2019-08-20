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

        PeriodTimer period_flush = new PeriodTimer(new TimeSpan(0, 0, 1));

        object locker = new object();
        Dictionary<string, PDFDocument> documents_to_store = new Dictionary<string, PDFDocument>();

        protected DocumentQueuedStorer()
        {
            MaintainableManager.Instance.RegisterHeldOffTask(DoMaintenance_FlushDocuments, 30 * 1000, ThreadPriority.BelowNormal);
            // Quit this delayed storing of PDF files when we've hit the end of the execution run: 
            // we'll have to save them all to disk in one go then, and quickly too!
            Utilities.Shutdownable.ShutdownableManager.Instance.Register(Shutdown);
        }

        void DoMaintenance_FlushDocuments(Daemon daemon)
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

        void Shutdown()
        {
            // **forced** flush!
            FlushDocuments(true);
        }

        object flush_locker = new object();

        private void FlushDocuments(bool force_flush_no_matter_what)
        {
            // use a lock to ensure the time-delayed flush doesn't ever collide with the
            // end-of-execution-run flush initiated by ShutdownableManager.
            lock (flush_locker)
            {
                while (true)
                {
                    int count_to_go = PendingQueueCount;

                    if (0 < count_to_go)
                    {
                        StatusManager.Instance.UpdateStatusBusy("DocumentQueuedStorer", String.Format("{0} documents still to flush", count_to_go), 1, count_to_go);
                    }
                    else
                    {
                        StatusManager.Instance.ClearStatus("DocumentQueuedStorer");
                        return;
                    }

                    // No flushing while still adding... unless we're quitting the executable already.
                    if (!force_flush_no_matter_what && Library.IsBusyAddingPDFs)
                    {
                        return;
                    }

                    PDFDocument pdf_document_to_flush = null;

                    // grab one PDF to save/flush:
                    lock (locker)
                    {
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
        }

        public void Queue(PDFDocument pdf_document)
        {
            lock (locker)
            {
                documents_to_store[pdf_document.Library.WebLibraryDetail.Id + "." + pdf_document.Fingerprint] = pdf_document;
            }
        }

        public int PendingQueueCount
        {
            get
            {
                lock (locker)
                {
                    return documents_to_store.Count;
                }
            }
        }
    }
}
