using System;
using System.Collections.Generic;
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
        static readonly int LIBRARY_SCAN_PERIOD_SECONDS = 60;
        static readonly int DOCUMENT_INDEX_RETRY_PERIOD_SECONDS = 60;

        private Library library;

        LuceneIndex word_index_manager;

        DateTime time_of_last_library_scan = DateTime.MinValue;
        Dictionary<string, PDFDocumentInLibrary> pdf_documents_in_library;

        public LibraryIndex(Library library)
        {
            this.library = library;

            // Try to load a historical progress file
            if (File.Exists(Filename_DocumentProgressList))
            {
                pdf_documents_in_library = (Dictionary<string, PDFDocumentInLibrary>)SerializeFile.LoadSafely(Filename_DocumentProgressList);
            }

            // If there was no historical progress file, start afresh
            if (null == pdf_documents_in_library)
            {
                Logging.Warn("Cound not find any indexing progress, so starting from scratch.");
                pdf_documents_in_library = new Dictionary<string, PDFDocumentInLibrary>();
            }

            word_index_manager = new LuceneIndex(library.LIBRARY_INDEX_BASE_PATH);
            word_index_manager.WriteMasterList();
        }

        #region --- Disposal ----------------------------------------------------------------------------------------

        ~LibraryIndex()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);            
        } 

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Get rid of managed resources
                this.word_index_manager.Dispose();                
            }

            // Get rid of unmanaged resources 
        }

        #endregion

        public void IncrementalBuildIndex(Daemon daemon)
        {
            if (DateTime.UtcNow.Subtract(time_of_last_library_scan).TotalSeconds > LIBRARY_SCAN_PERIOD_SECONDS)
            {
                RescanLibrary();
                time_of_last_library_scan = DateTime.UtcNow;
            }

            bool did_some_work = IncrementalBuildNextDocuments(daemon);

            // Flush to disk
            if (did_some_work)
            {
                Logging.Info("+Writing the index master list");
                word_index_manager.WriteMasterList();
                lock (pdf_documents_in_library)
                {
                    SerializeFile.SaveSafely(Filename_DocumentProgressList, pdf_documents_in_library);
                }
                Logging.Info("-Wrote the index master list");

                // Report to user
                UpdateStatus();
            }
        }

        public void ReIndexDocument(PDFDocument pdf_document)
        {
            try
            {
                lock (pdf_documents_in_library)
                {                    
                    pdf_documents_in_library.Remove(pdf_document.Fingerprint);
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "How is this exception being thrown?");
                FeatureTrackingManager.Instance.UseFeature(Features.Exception_NullExceptionInReIndexDocument);
            }
        }

        public void UpdateStatus()
        {
            int numerator_documents = 0;
            int denominator_documents = 0;
            int pages_so_far = 0;
            int pages_to_go = 0;

            lock (pdf_documents_in_library)
            {
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

            StatusManager.Instance.UpdateStatus("LibraryIndex", String.Format("{3} page(s) are searchable ({2} still to go)", numerator_documents, denominator_documents, pages_to_go, pages_so_far), 1, pages_to_go);
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


        private void RescanLibrary()
        {
            // We include the deleted ones because we need to reindex their metadata...
            List<PDFDocument> pdf_documents = library.PDFDocuments_IncludingDeleted;

            int total_new_to_be_indexed = 0;

            lock (pdf_documents_in_library)
            {
                foreach (PDFDocument pdf_document in pdf_documents)
                {
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
                        Logging.Error(ex, "There was a problem with a document while rescanning the library for indexing");
                    }
                }
            }

            if (total_new_to_be_indexed > 0)            
            {
                Logging.Info("There are {0} new document(s) to be indexed", total_new_to_be_indexed);
            }
        }

        public void InvalidateIndex()
        {            
            word_index_manager.InvalidateIndex();
        }

        private bool IncrementalBuildNextDocuments(Daemon daemon)
        {
            bool did_some_work = false;

            // If this library is busy, skip it for now
            if (Library.IsBusyAddingPDFs)
            {
                Logging.Info("IncrementalBuildNextDocuments: Not daemon processing a library that is busy with adds...");
                return false;
            }

            lock (pdf_documents_in_library)
            {
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

                    if (daemon != null && !daemon.StillRunning)
                    {
                        Logging.Debug("Breaking out of IncrementalBuildNextDocuments processing loop due to daemon termination");
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

        int NumberOfIndexedPDFDocuments
        {
            get
            {
                lock (pdf_documents_in_library)
                {
                    return pdf_documents_in_library.Count;
                }
            }
        }

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
    }
}
