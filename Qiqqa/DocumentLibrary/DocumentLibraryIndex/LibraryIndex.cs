using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibraryIndex;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Language;
using Utilities.Language.TextIndexing;
using Utilities.Misc;
using Utilities.OCR;
using Utilities.Strings;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.DocumentLibraryIndex
{
    public class LibraryIndex : IDisposable
    {
        private const int LIBRARY_SCAN_PERIOD_SECONDS = 60;
        private const int DOCUMENT_INDEX_RETRY_PERIOD_SECONDS = 60;
        private const int MAX_MILLISECONDS_PER_ITERATION = 15 * 1000;

        private TypedWeakReference<Library> library;
        public Library Library => library?.TypedTarget;

        private LuceneIndex word_index_manager = null;
        private object word_index_manager_lock = new object();
        private Stopwatch time_of_last_library_scan = Stopwatch.StartNew();
        private Dictionary<string, PDFDocumentInLibrary> pdf_documents_in_library = null;
        private object pdf_documents_in_library_lock = new object();

        private bool libraryIndex_is_loaded = false;
        private object libraryIndex_is_loaded_lock = new object();        // fast lock
        private object libraryIndexInit_is_pending_lock = new object();   // slow lock; wraps the entire Init phase to ensure no two threads run it in parallel

        public bool LibraryIndexIsLoaded
        {
            get
            {
                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (libraryIndex_is_loaded_lock)
                {
                    // l1_clk.LockPerfTimerStop();
                    return libraryIndex_is_loaded;
                }
            }
            private set
            {
                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (libraryIndex_is_loaded_lock)
                {
                    // l1_clk.LockPerfTimerStop();
                    libraryIndex_is_loaded = value;
                }
            }
        }

        public LibraryIndex(Library library)
        {
            this.library = new TypedWeakReference<Library>(library);

            // postpone INIT phase...
        }

        private void Init()
        {
            // have we been here before?
            if (LibraryIndexIsLoaded)
            {
                return;
            }

            // Utilities.LockPerfTimer l5_clk = Utilities.LockPerfChecker.Start();
            lock (libraryIndexInit_is_pending_lock)
            {
                // l5_clk.LockPerfTimerStop();

                //Utilities.LockPerfTimer l4_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    lock (word_index_manager_lock)
                    {
                        //l4_clk.LockPerfTimerStop();
                        if (null != pdf_documents_in_library && null != word_index_manager)
                        {
                            Logging.Warn("LibraryIndex has already been initialized.");
                            return;
                        }
                    }
                }

                Logging.Info("Try to load a historical progress file: {0}", Filename_DocumentProgressList);
                try
                {
                    if (File.Exists(Filename_DocumentProgressList))
                    {
                        Stopwatch clk = Stopwatch.StartNew();
                        Logging.Info("+Loading historical progress file: {0}", Filename_DocumentProgressList);
                        //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                        lock (pdf_documents_in_library_lock)
                        {
                            //l1_clk.LockPerfTimerStop();
                            pdf_documents_in_library = (Dictionary<string, PDFDocumentInLibrary>)SerializeFile.LoadSafely(Filename_DocumentProgressList);
                        }
                        Logging.Info("-Loaded historical progress file: {0} (time spent: {1} ms)", Filename_DocumentProgressList, clk.ElapsedMilliseconds);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "FAILED to load historical progress file \"{0}\". Will start indexing afresh.", Filename_DocumentProgressList);
                    //Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                    lock (pdf_documents_in_library_lock)
                    {
                        //l2_clk.LockPerfTimerStop();
                        pdf_documents_in_library = null;
                    }
                }

                // If there was no historical progress file, start afresh
                //Utilities.LockPerfTimer l3_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    //l3_clk.LockPerfTimerStop();
                    if (null == pdf_documents_in_library)
                    {
                        Logging.Warn("Cound not find any indexing progress, so starting from scratch.");
                        pdf_documents_in_library = new Dictionary<string, PDFDocumentInLibrary>();
                    }
                }

                // Utilities.LockPerfTimer l6_clk = Utilities.LockPerfChecker.Start();
                lock (word_index_manager_lock)
                {
                    // l6_clk.LockPerfTimerStop();
                    word_index_manager = new LuceneIndex(Library.LIBRARY_INDEX_BASE_PATH);
                    word_index_manager.WriteMasterList();
                }

                LibraryIndexIsLoaded = true;
            }
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
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("LibraryIndex::Dispose({0}) @{1}", disposing, dispose_count);

            try
            {
                if (dispose_count == 0)
                {
                    // Get rid of managed resources
                    // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                    lock (word_index_manager_lock)
                    {
                        // l1_clk.LockPerfTimerStop();

                        word_index_manager?.Dispose();
                        word_index_manager = null;
                    }

                    //this.library?.Dispose();
                }

                //this.word_index_manager = null;
                library = null;

                //Utilities.LockPerfTimer l4_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    //l4_clk.LockPerfTimerStop();

                    pdf_documents_in_library?.Clear();
                    pdf_documents_in_library = null;
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }

            ++dispose_count;
        }

        #endregion

        public void IncrementalBuildIndex()
        {
            if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
            {
                Logging.Debug特("LibraryIndex::IncrementalBuildIndex: Breaking out due to application termination");
                return;
            }

            if (Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
            {
                Logging.Debug特("LibraryIndex::IncrementalBuildIndex: Breaking due to DisableAllBackgroundTasks");
                return;
            }

            Init();

            if (time_of_last_library_scan.ElapsedMilliseconds >= LIBRARY_SCAN_PERIOD_SECONDS * 1000)
            {
                if (RescanLibrary())
                {
                    time_of_last_library_scan.Restart();
                }
            }

            bool did_some_work = IncrementalBuildNextDocuments();

            // Flush to disk
            if (did_some_work)
            {
                Stopwatch clk = Stopwatch.StartNew();

                Logging.Info("+Writing the index master list");

                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (word_index_manager_lock)
                {
                    // l1_clk.LockPerfTimerStop();

                    word_index_manager.WriteMasterList();
                }

                //Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    //l2_clk.LockPerfTimerStop();
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
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    pdf_documents_in_library?.Remove(pdf_document.Fingerprint);
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "How is this exception being thrown? Are we in the process of starting the application and 'inside' the RestoreDesktop process?");
                FeatureTrackingManager.Instance.UseFeature(Features.Exception_NullExceptionInReIndexDocument);
            }
        }

        private void GetStatusCounts(out int numerator_documents, out int denominator_documents, out int pages_so_far, out int pages_to_go)
        {
            numerator_documents = 0;
            denominator_documents = 0;
            pages_so_far = 0;
            pages_to_go = 0;

            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_in_library_lock)
            {
                //l1_clk.LockPerfTimerStop();
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

        private void UpdateStatus()
        {
            int numerator_documents = 0;
            int denominator_documents = 0;
            int pages_so_far = 0;
            int pages_to_go = 0;

            GetStatusCounts(out numerator_documents, out denominator_documents, out pages_so_far, out pages_to_go);

            StatusManager.Instance.UpdateStatus("LibraryIndex", String.Format("{1} page(s) are searchable ({0} still to go)", pages_to_go, pages_so_far), pages_so_far, pages_to_go + pages_so_far);
        }

        public List<IndexResult> GetFingerprintsForQuery(string query)
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (word_index_manager_lock)
            {
                // l1_clk.LockPerfTimerStop();

                return word_index_manager?.GetDocumentsWithQuery(query) ?? new List<IndexResult>();
            }
        }

        public List<IndexPageResult> GetPagesForQuery(string query)
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (word_index_manager_lock)
            {
                // l1_clk.LockPerfTimerStop();

                return word_index_manager?.GetDocumentPagesWithQuery(query) ?? new List<IndexPageResult>();
            }
        }

        [Obsolete("Do not use this attribute", true)]
        public HashSet<string> GetFingerprintsForKeyword(string keyword)
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (word_index_manager_lock)
            {
                // l1_clk.LockPerfTimerStop();

                return word_index_manager?.GetDocumentsWithWord(keyword) ?? new HashSet<string>();
            }
        }

        public int GetDocumentCountForKeyword(string keyword)
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (word_index_manager_lock)
            {
                // l1_clk.LockPerfTimerStop();

                return word_index_manager?.GetDocumentCountForKeyword(keyword) ?? 0;
            }
        }

        private string Filename_DocumentProgressList => Path.GetFullPath(Path.Combine(Library.LIBRARY_INDEX_BASE_PATH, @"DocumentProgressList.dat"));

        private bool RescanLibrary()
        {
            // We include the deleted ones because we need to reindex their metadata...
            List<PDFDocument> pdf_documents = Library.PDFDocuments_IncludingDeleted;

            int total_new_to_be_indexed = 0;

            Stopwatch clk = Stopwatch.StartNew();

            foreach (PDFDocument pdf_document in pdf_documents)
            {
                if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                {
                    Logging.Info("Breaking out of RescanLibrary loop due to application termination");
                    return false;
                }

                if (Library.LibraryIsKilled)
                {
                    Logging.Info("Breaking out of RescanLibrary loop due to forced ABORT/Dispose of library instance.");
                    return false;
                }

                // Do not index *deleted* documents:
                if (pdf_document.Deleted)
                {
                    continue;
                }

                try
                {
                    bool go;

                    //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                    lock (pdf_documents_in_library_lock)
                    {
                        //l1_clk.LockPerfTimerStop();

                        go = !pdf_documents_in_library.ContainsKey(pdf_document.Fingerprint);
                    }

                    if (go)
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

                        //Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                        lock (pdf_documents_in_library_lock)
                        {
                            //l2_clk.LockPerfTimerStop();
                            pdf_documents_in_library[pdf_document_in_library.fingerprint] = pdf_document_in_library;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem with a document while rescanning the library for indexing. Document fingerprint: {0}", pdf_document.Fingerprint);
                }
            }

            long clk_duration = clk.ElapsedMilliseconds;
            Logging.Debug特("Rescan of library {0} for indexing took {1}ms for {2} documents.", Library.ToString(), clk_duration, pdf_documents.Count);

            if (total_new_to_be_indexed > 0)
            {
                Logging.Info("There are {0} new document(s) to be indexed", total_new_to_be_indexed);
            }
            return true;
        }

        public void InvalidateIndex()
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (word_index_manager_lock)
            {
                // l1_clk.LockPerfTimerStop();

                word_index_manager?.InvalidateIndex();
            }
        }

        private bool IncrementalBuildNextDocuments()
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            bool did_some_work = false;

            // If this library is busy, skip it for now
            if (Library.IsBusyAddingPDFs)
            {
                Logging.Debug特("IncrementalBuildNextDocuments: Not daemon processing any library that is busy with adds...");
                return false;
            }

            Stopwatch clk = Stopwatch.StartNew();
            DateTime index_processing_start_time = DateTime.UtcNow;

            // We will only attempt to process documents that have not been looked at for a while - what is that time
            DateTime most_recent_eligible_time_for_processing = index_processing_start_time.Subtract(TimeSpan.FromSeconds(DOCUMENT_INDEX_RETRY_PERIOD_SECONDS));

            //
            // IMPORTANT THREAD SAFETY NOTE:
            //
            // We can use minimal locking (i.e. only critical section-ing the list-fetch qeury code below, instead of the entire work loop further below)
            // as this is the only place where the content of the individual records is edited and accessed (apart from the non-critical function
            // `GetStatusCounts()` which only serves to update the UI status reports) and the rest of the Qiqqa code ensures that this method 
            // `IncrementalBuildNextDocuments()` is only invoked from a single (background) thread.
            //
            // All the other places where the `pdf_documents_in_library` data is accessed are (critical section-ed) member functions of this class which
            // only add or remove *entire records* at once; as those add/remove actions happen *inside* those critical sections, we're safe to minimize the
            // critical section below to only the LINQ query code.
            //
            // This also permits us to place 'yield' calls inside the work loop further below, iff we ever feel the need to in order to reduce the CPU load
            // of this piece of code in relation to other Qiqqa activities.
            //

            List<PDFDocumentInLibrary> pdf_documents_in_library_to_process;

            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (pdf_documents_in_library_lock)
            {
                //l1_clk.LockPerfTimerStop();

                // Get all documents that are not been finished with their indexing
                pdf_documents_in_library_to_process = new List<PDFDocumentInLibrary>(
                    from pdf_document_in_library in pdf_documents_in_library.Values
                    orderby pdf_document_in_library.last_indexed ascending
                    where (pdf_document_in_library.finished_indexing == false || pdf_document_in_library.metadata_already_indexed == false)
                    // Don't try to reprocess the document queue too frequently
                    && pdf_document_in_library.last_indexed < most_recent_eligible_time_for_processing
                    select pdf_document_in_library
                );
            }

            // Process each one
            foreach (PDFDocumentInLibrary pdf_document_in_library in pdf_documents_in_library_to_process)
            {
                if (clk.ElapsedMilliseconds > MAX_MILLISECONDS_PER_ITERATION)
                {
                    Logging.Info("IncrementalBuildNextDocuments: Breaking out of processing loop due to MAX_SECONDS_PER_ITERATION: {0}ms consumed", clk.ElapsedMilliseconds);
                    break;
                }

                if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                {
                    Logging.Info("Breaking out of IncrementalBuildNextDocuments processing loop due to application termination");
                    break;
                }

                if (Library.LibraryIsKilled)
                {
                    Logging.Info("Breaking out of IncrementalBuildNextDocuments loop due to forced ABORT/Dispose of library instance.");
                    break;
                }

                try
                {
                    Logging.Info("Indexing document {0}", pdf_document_in_library.fingerprint);

                    PDFDocument pdf_document = Library.GetDocumentByFingerprint(pdf_document_in_library.fingerprint);

                    bool all_pages_processed_so_far = true;

                    if (null != pdf_document)
                    {
                        // Do we need to index the metadata?
                        if (!pdf_document_in_library.metadata_already_indexed)
                        {
                            did_some_work = true;

                            StringBuilder sb_annotations = new StringBuilder();
                            foreach (var annotation in pdf_document.GetAnnotations())
                            {
                                sb_annotations.AppendLine(annotation.Text);
                                sb_annotations.AppendLine(annotation.Tags);
                            }

                            StringBuilder sb_tags = new StringBuilder();
                            foreach (string tag in TagTools.ConvertTagBundleToTags(pdf_document.Tags))
                            {
                                sb_tags.AppendLine(tag);
                            }

                            // Utilities.LockPerfTimer l6_clk = Utilities.LockPerfChecker.Start();
                            lock (word_index_manager_lock)
                            {
                                // l6_clk.LockPerfTimerStop();

                                word_index_manager.AddDocumentMetadata(pdf_document.Deleted, pdf_document.Fingerprint, pdf_document.TitleCombined, pdf_document.AuthorsCombined, pdf_document.YearCombined, pdf_document.Comments, sb_tags.ToString(), sb_annotations.ToString(), pdf_document.BibTex, pdf_document.BibTexItem);
                            }

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
                                    WordList word_list = null;

                                    // Don't reprocess any pages that have already been processed
                                    if (pdf_document_in_library.pages_already_indexed?.Contains(page) ?? false)
                                    {
                                        continue;
                                    }

                                    word_list = pdf_document.PDFRenderer.GetOCRText(page);

                                    // Process each word of the document
                                    if (null == word_list)
                                    {
                                        // Report the missing pages as this is *probably* an OCR issue with this PDF/document
                                        //
                                        // First check if the OCR actions have delivered already:
                                        if (null != pdf_document_in_library.pages_already_indexed && pdf_document_in_library.pages_already_indexed.Count > 0)
                                        {
                                            Logging.Warn("LibraryIndex::IncrementalBuildNextDocuments: PDF document {0}: page {1} has no text (while pages {2} DO have text!) and will (re)trigger a PDF OCR action.{3}", pdf_document.Fingerprint, page, StringTools.PagesSetAsString(pdf_document_in_library.pages_already_indexed), (page < pdf_document_in_library.pages_already_indexed.Last() ? " This is probably a document which could not be OCRed properly (for reasons unknown at this time)." : ""));
                                        }

                                        all_pages_processed_so_far = false;
                                    }
                                    else
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

                                        // Utilities.LockPerfTimer l7_clk = Utilities.LockPerfChecker.Start();
                                        lock (word_index_manager_lock)
                                        {
                                            // l7_clk.LockPerfTimerStop();

                                            // Index it
                                            word_index_manager.AddDocumentPage(pdf_document.Deleted, pdf_document_in_library.fingerprint, page, sb.ToString());
                                        }

                                        // Indicate that we have managed to index this page
                                        if (null == pdf_document_in_library.pages_already_indexed)
                                        {
                                            pdf_document_in_library.pages_already_indexed = new HashSet<int>();
                                        }
                                        pdf_document_in_library.pages_already_indexed.Add(page);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Logging.Warn("It appears that document {0} is no longer in library {1} so will be removed from indexing", pdf_document_in_library.fingerprint, Library.WebLibraryDetail.Id);
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

                pdf_document_in_library.last_indexed = index_processing_start_time;
            }

            long clk_duration = clk.ElapsedMilliseconds;
            Logging.Debug特("Incremental building of the library index for library {0} took {1}ms.", Library.ToString(), clk_duration);

            return did_some_work;
        }

#if false
        public int NumberOfIndexedPDFDocuments
        {
            get
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (pdf_documents_in_library_lock)
                {
                    //l1_clk.LockPerfTimerStop();
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
#endif

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestHarness()
        {
            while (true)
            {
                Library library = Library.GuestInstance;
                library.LibraryIndex.IncrementalBuildIndex(null);

                Logging.Info("Number of indexed PDF Documents is {0}", Library.LibraryIndex.NumberOfIndexedPDFDocuments);
                Thread.Sleep(1000);
            }
        }
#endif

        #endregion
    }
}
