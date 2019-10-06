using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibraryIndex;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.BibTex.Parsing;
using Utilities.Files;
using Utilities.Language;
using Utilities.Language.TextIndexing;
using Utilities.Misc;
using Utilities.OCR;

namespace Qiqqa.DocumentLibrary.DocumentLibraryIndex
{
    public class LibraryIndex : IDisposable
    {
        const int LIBRARY_SCAN_PERIOD_SECONDS = 60;
        const int DOCUMENT_INDEX_RETRY_PERIOD_SECONDS = 60;

        private Library library;

        LuceneIndex word_index_manager;

        DateTime time_of_last_library_scan = DateTime.MinValue;
        Dictionary<string, PDFDocumentInLibrary> pdf_documents_in_library;
        private object pdf_documents_in_library_lock = new object();

        public LibraryIndex(Library library)
        {
            this.library = library;

            Logging.Info("Try to load a historical progress file: {0}", Filename_DocumentProgressList);
            try
            {
                if (File.Exists(Filename_DocumentProgressList))
                {
                    Stopwatch clk = new Stopwatch();
                    clk.Start();
                    Logging.Info("+Loading historical progress file: {0}", Filename_DocumentProgressList);
                    Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                    lock (pdf_documents_in_library_lock)
                    {
                        l1_clk.LockPerfTimerStop();
                        pdf_documents_in_library = (Dictionary<string, PDFDocumentInLibrary>)SerializeFile.LoadSafely(Filename_DocumentProgressList);
                    }
                    Logging.Info("-Loaded historical progress file: {0} (time spent: {1} ms)", Filename_DocumentProgressList, clk.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "FAILED to load historical progress file \"{0}\". Will start indexing afresh.", Filename_DocumentProgressList);
                Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    l2_clk.LockPerfTimerStop();
                    pdf_documents_in_library = null;
                }
            }

            // If there was no historical progress file, start afresh
            Utilities.LockPerfTimer l3_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_in_library_lock)
            {
                l3_clk.LockPerfTimerStop();
                if (null == pdf_documents_in_library)
                {
                    Logging.Warn("Cound not find any indexing progress, so starting from scratch.");
                    pdf_documents_in_library = new Dictionary<string, PDFDocumentInLibrary>();
                }
            }

            word_index_manager = new LuceneIndex(library.LIBRARY_INDEX_BASE_PATH);
            word_index_manager.WriteMasterList();
        }

        #region --- Disposal ----------------------------------------------------------------------------------------

        ~LibraryIndex()
        {
            Logging.Debug("~LibraryIndex()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing LibraryIndex");
            Dispose(true);
            GC.SuppressFinalize(this);            
        }

        private int dispose_count = 0;
        private void Dispose(bool disposing)
        {
            Logging.Debug("LibraryIndex::Dispose({0}) @{1}", disposing ? "true" : "false", ++dispose_count);
            if (disposing)
            {
                // Get rid of managed resources
                this.word_index_manager?.Dispose();

                this.library?.Dispose();
            }

            this.word_index_manager = null;
            this.library = null;
            Utilities.LockPerfTimer l4_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_in_library_lock)
            {
                l4_clk.LockPerfTimerStop();
                this.pdf_documents_in_library = null;
            }

            // Get rid of unmanaged resources 
        }

        #endregion

        public void IncrementalBuildIndex()
        {
            if (DateTime.UtcNow.Subtract(time_of_last_library_scan).TotalSeconds > LIBRARY_SCAN_PERIOD_SECONDS)
            {
                if (RescanLibrary())
                {
                    time_of_last_library_scan = DateTime.UtcNow;
                }
            }

            bool did_some_work = IncrementalBuildNextDocuments();

            // Flush to disk
            if (did_some_work)
            {
                Stopwatch clk = new Stopwatch();
                clk.Start();
                Logging.Info("+Writing the index master list");
                word_index_manager.WriteMasterList();
                Utilities.LockPerfTimer l5_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    l5_clk.LockPerfTimerStop();
                    SerializeFile.SaveSafely(Filename_DocumentProgressList, pdf_documents_in_library);
                }
                Logging.Info("-Wrote the index master list (time spent: {0} ms", clk.ElapsedMilliseconds);

                // Report to user
                UpdateStatus();
            }
        }

        public void ReIndexDocument(PDFDocument pdf_document)
        {
            try
            {
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    pdf_documents_in_library.Remove(pdf_document.Fingerprint);
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "How is this exception being thrown?");
                FeatureTrackingManager.Instance.UseFeature(Features.Exception_NullExceptionInReIndexDocument);
            }
        }

        public void GetStatusCounts(out int numerator_documents, out int denominator_documents, out int pages_so_far, out int pages_to_go)
        {
            numerator_documents = 0;
            denominator_documents = 0;
            pages_so_far = 0;
            pages_to_go = 0;

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_in_library_lock)
            {
                l1_clk.LockPerfTimerStop();
                foreach (PDFDocumentInLibrary pdf_document_in_library in pdf_documents_in_library.Values)
                {
                    ++denominator_documents;
                    if (pdf_document_in_library.finished_indexing)
                    {
                        ++numerator_documents;
                        pages_so_far += pdf_document_in_library.total_pages;
                    }
                    else
                    {
                        int finished_pages_count = (pdf_document_in_library.pages_already_indexed != null) ? pdf_document_in_library.pages_already_indexed.Count : 0;
                        pages_so_far += finished_pages_count;
                        pages_to_go += (pdf_document_in_library.total_pages - finished_pages_count);
                    }
                }
            }
        }

        public void UpdateStatus()
        {
            int numerator_documents = 0;
            int denominator_documents = 0;
            int pages_so_far = 0;
            int pages_to_go = 0;

            GetStatusCounts(out numerator_documents, out denominator_documents, out pages_so_far, out pages_to_go);

            StatusManager.Instance.UpdateStatus("LibraryIndex", String.Format("{3} page(s) are searchable ({2} still to go)", numerator_documents, denominator_documents, pages_to_go, pages_so_far), pages_to_go, pages_to_go + pages_so_far);
        }

        public List<IndexResult> GetFingerprintsForQuery(string query)
        {
            return word_index_manager.GetDocumentsWithQuery(query);
        }

        public List<IndexPageResult> GetPagesForQuery(string query)
        {
            return word_index_manager.GetDocumentPagesWithQuery(query);
        }

        [Obsolete("Do not use this attribute, but keep it in the class definition for backwards compatibility of the serialization", true)]
        public HashSet<string> GetFingerprintsForKeyword(string keyword)
        {
            return word_index_manager.GetDocumentsWithWord(keyword);
        }

        public int GetDocumentCountForKeyword(string keyword)
        {
            return word_index_manager.GetDocumentCountForKeyword(keyword);
        }

        string Filename_DocumentProgressList
        {
            get
            {
                return library.LIBRARY_INDEX_BASE_PATH + "DocumentProgressList.dat";
            }
        }

        private bool RescanLibrary()
        {
            // We include the deleted ones because we need to reindex their metadata...
            List<PDFDocument> pdf_documents = library.PDFDocuments_IncludingDeleted;

            int total_new_to_be_indexed = 0;

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_in_library_lock)
            {
                l1_clk.LockPerfTimerStop();
                foreach (PDFDocument pdf_document in pdf_documents)
                {
                    if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                    {
                        Logging.Info("Breaking out of RescanLibrary loop due to application termination");
                        return false;
                    }

                    try
                    {
                        if (!pdf_documents_in_library.ContainsKey(pdf_document.Fingerprint))
                        {
                            ++total_new_to_be_indexed;

                            PDFDocumentInLibrary pdf_document_in_library = new PDFDocumentInLibrary();
                            pdf_document_in_library.fingerprint = pdf_document.Fingerprint;
                            pdf_document_in_library.last_indexed = DateTime.MinValue;
                            pdf_document_in_library.pages_already_indexed = null;

                            if (pdf_document.DocumentExists)
                            {
                                pdf_document_in_library.total_pages = pdf_document.PDFRenderer.PageCount;                                
                                pdf_document_in_library.finished_indexing = false;
                            }
                            else
                            {
                                pdf_document_in_library.total_pages = 0;
                                pdf_document_in_library.finished_indexing = true;
                            }

                            pdf_documents_in_library[pdf_document_in_library.fingerprint] = pdf_document_in_library;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was a problem with a document while rescanning the library for indexing. Document fingerprint: {0}", pdf_document.Fingerprint);
                    }
                }
            }

            if (total_new_to_be_indexed > 0)            
            {
                Logging.Info("There are {0} new document(s) to be indexed", total_new_to_be_indexed);
            }
            return true;
        }

        public void InvalidateIndex()
        {            
            word_index_manager.InvalidateIndex();
        }

        private bool IncrementalBuildNextDocuments()
        {
            bool did_some_work = false;

            // If this library is busy, skip it for now
            if (Library.IsBusyAddingPDFs)
            {
                Logging.Info("IncrementalBuildNextDocuments: Not daemon processing a library that is busy with adds...");
                return false;
            }

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_in_library_lock)
            {
                l1_clk.LockPerfTimerStop();
                // We will only attempt to process documents that have not been looked at for a while - what is that time
                DateTime most_recent_eligible_time_for_processing = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(DOCUMENT_INDEX_RETRY_PERIOD_SECONDS));

                // Get all documents that are not been finished with their indexing
                var pdf_documents_in_library_to_process =
                    from pdf_document_in_library in pdf_documents_in_library.Values
                    orderby pdf_document_in_library.last_indexed ascending
                    where (pdf_document_in_library.finished_indexing == false || pdf_document_in_library.metadata_already_indexed == false)
                    && pdf_document_in_library.last_indexed < most_recent_eligible_time_for_processing
                    select pdf_document_in_library;

                // Process each one
                const int MAX_SECONDS_PER_ITERATION = 15;
                DateTime index_processing_start_time = DateTime.UtcNow;
                foreach (PDFDocumentInLibrary pdf_document_in_library in pdf_documents_in_library_to_process)
                {
                    if (DateTime.UtcNow.Subtract(index_processing_start_time).TotalSeconds > MAX_SECONDS_PER_ITERATION)
                    {
                        Logging.Info("IncrementalBuildNextDocuments: Breaking out of processing loop due to MAX_SECONDS_PER_ITERATION: {0} seconds consumed", DateTime.UtcNow.Subtract(index_processing_start_time).TotalSeconds);
                        break;
                    }

                    if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                    {
                        Logging.Info("Breaking out of IncrementalBuildNextDocuments processing loop due to application termination");
                        break;
                    }

                    // Don't try to reprocess the document queue too frequently
                    if (DateTime.UtcNow.Subtract(pdf_document_in_library.last_indexed).TotalSeconds < DOCUMENT_INDEX_RETRY_PERIOD_SECONDS)
                    {
                        continue;
                    }

                    try
                    {
                        Logging.Info("Indexing document {0}", pdf_document_in_library.fingerprint);

                        PDFDocument pdf_document = library.GetDocumentByFingerprint(pdf_document_in_library.fingerprint);

                        bool all_pages_processed_so_far = true;

                        if (null != pdf_document)
                        {
                            // Do we need to index the metadata?
                            if (!pdf_document_in_library.metadata_already_indexed)
                            {
                                did_some_work = true;

                                StringBuilder sb_annotations = new StringBuilder();
                                foreach (var annotation in pdf_document.Annotations)
                                {
                                    sb_annotations.AppendLine(annotation.Text);
                                    sb_annotations.AppendLine(annotation.Tags);
                                }

                                StringBuilder sb_tags = new StringBuilder();
                                foreach (string tag in TagTools.ConvertTagBundleToTags(pdf_document.Tags))
                                {
                                    sb_tags.AppendLine(tag);
                                }

                                word_index_manager.AddDocumentMetadata(pdf_document.Deleted, pdf_document.Fingerprint, pdf_document.TitleCombined, pdf_document.AuthorsCombined, pdf_document.YearCombined, pdf_document.Comments, sb_tags.ToString(), sb_annotations.ToString(), pdf_document.BibTex, pdf_document.BibTexItem);
    
                                pdf_document_in_library.metadata_already_indexed = true;
                            }

                            // If the document is deleted, we are done...
                            if (pdf_document.Deleted)
                            {
                                pdf_document_in_library.finished_indexing = true;
                            }

                            if (!pdf_document_in_library.finished_indexing)
                            {
                                if (pdf_document.DocumentExists)
                                {
                                    for (int page = 1; page <= pdf_document.PDFRenderer.PageCount; ++page)
                                    {
                                        // Don't reprocess any pages that have already been processed
                                        if (null != pdf_document_in_library.pages_already_indexed && pdf_document_in_library.pages_already_indexed.Contains(page))
                                        {
                                            continue;
                                        }

                                        // Process each word of the document
                                        WordList word_list = pdf_document.PDFRenderer.GetOCRText(page);
                                        if (null != word_list)
                                        {
                                            did_some_work = true;

                                            // Create the text string
                                            StringBuilder sb = new StringBuilder();
                                            foreach (Word word in word_list)
                                            {
                                                string reasonable_word = ReasonableWord.MakeReasonableWord(word.Text);
                                                if (!String.IsNullOrEmpty(reasonable_word))
                                                {
                                                    sb.Append(reasonable_word);
                                                    sb.Append(' ');
                                                }
                                            }

                                            // Index it
                                            word_index_manager.AddDocumentPage(pdf_document.Deleted, pdf_document_in_library.fingerprint, page, sb.ToString());

                                            // Indicate that we have managed to index this page
                                            if (null == pdf_document_in_library.pages_already_indexed)
                                            {
                                                pdf_document_in_library.pages_already_indexed = new HashSet<int>();
                                            }
                                            pdf_document_in_library.pages_already_indexed.Add(page);
                                        }
                                        else
                                        {
                                            all_pages_processed_so_far = false;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Logging.Warn("It appears that document {0} is no longer in library {1} so will be removed from indexing", pdf_document_in_library.fingerprint, library.WebLibraryDetail.Id);
                        }

                        if (all_pages_processed_so_far)
                        {
                            Logging.Info("Indexing is complete for {0}", pdf_document_in_library.fingerprint);
                            pdf_document_in_library.finished_indexing = true;
                            pdf_document_in_library.pages_already_indexed = null;                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was a problem while indexing document {0}", pdf_document_in_library.fingerprint);
                    }

                    pdf_document_in_library.last_indexed = DateTime.UtcNow;                    
                }
            }

            return did_some_work;
        }

        public int NumberOfIndexedPDFDocuments
        {
            get
            {
                Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    l1_clk.LockPerfTimerStop();
                    return pdf_documents_in_library.Count;
                }
            }
        }

        public int PagesPending
        {
            get
            {
                int numerator_documents = 0;
                int denominator_documents = 0;
                int pages_so_far = 0;
                int pages_to_go = 0;

                GetStatusCounts(out numerator_documents, out denominator_documents, out pages_so_far, out pages_to_go);

                return pages_to_go;
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestHarness()
        {
            while (true)
            {
                Library library = Library.GuestInstance;
                library.LibraryIndex.IncrementalBuildIndex(null);

                Logging.Info("Number of indexed PDF Documents is {0}", library.LibraryIndex.NumberOfIndexedPDFDocuments);
                Thread.Sleep(1000);
            }
        }
#endif

        #endregion
    }
}
