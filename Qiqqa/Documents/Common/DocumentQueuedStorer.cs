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
            MaintainableManager.Instance.Register(DoMaintenance_FlushDocuments, 30000, ThreadPriority.Normal);
        }

        void DoMaintenance_FlushDocuments(Daemon daemon)
        {
            if (Qiqqa.Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
            {
                // do run the flush task, but delayed!
                daemon.Sleep(5 * 60 * 1000);
            }

            if (period_flush.Expired)
            {
                period_flush.Signal();
                FlushDocuments();
            }
        }

        private void FlushDocuments()
        {
            while (true)
            {
                int count_to_go = 0;

                // No flushing while still adding...
                if (Library.IsBusyAddingPDFs)
                {
                    lock (locker)
                    {
                        count_to_go = documents_to_store.Count;
                    }

                    if (0 < count_to_go)
                    {
                        StatusManager.Instance.UpdateStatusBusy("DocumentQueuedStorer", String.Format("{0} documents still to flush", count_to_go), 1, count_to_go);
                    }
                    else
                    {
                        StatusManager.Instance.ClearStatus("DocumentQueuedStorer");
                    }

                    return;
                }

                PDFDocument pdf_document_to_flush = null;

                lock (locker)
                {
                    foreach (var pair in documents_to_store)
                    {
                        pdf_document_to_flush = pair.Value;
                        documents_to_store.Remove(pair.Key);
                        count_to_go = documents_to_store.Count;
                        break;
                    }
                }

                if (0 < count_to_go)
                {
                    StatusManager.Instance.UpdateStatusBusy("DocumentQueuedStorer", String.Format("{0} documents still to flush", count_to_go), 1, count_to_go);
                }
                else
                {
                    StatusManager.Instance.ClearStatus("DocumentQueuedStorer");
                }

                if (null != pdf_document_to_flush)
                {
                    pdf_document_to_flush.SaveToMetaData();
                }
                else
                {
                    return;
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
