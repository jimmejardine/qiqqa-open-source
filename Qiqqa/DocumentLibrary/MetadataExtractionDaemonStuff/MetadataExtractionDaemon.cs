using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.MetadataSuggestions;
using Utilities;
using Utilities.Collections;
using Utilities.GUI;
using Utilities.Mathematics;
using Utilities.Misc;
using Utilities.Shutdownable;

namespace Qiqqa.DocumentLibrary.MetadataExtractionDaemonStuff
{
    internal class MetadataExtractionDaemon
    {
        private CountingDictionary<string> pdfs_retry_count = new CountingDictionary<string>();

        private class RunningStatistics
        {
            public int totalDocumentCount;
            public int currentdocumentIndex;
            public int documentsProcessedCount;
        }

        public void DoMaintenance(WebLibraryDetail web_library_detail, Action callback_after_some_work_done)
        {
            Stopwatch clk = Stopwatch.StartNew();

            Logging.Debug特("MetadataExtractionDaemon::DoMaintenance START");

            RunningStatistics stats = new RunningStatistics();

            // To recover from a search index fatal failure and re-indexing attempt for very large libraries,
            // we're better off processing a limited number of source files as we'll be able to see
            // *some* results more quickly and we'll have a working, though yet incomplete,
            // index in *reasonable time*.
            //
            // Reconstructing the entire index will take a *long* time. We grow the index and other meta
            // stores a bunch-of-files at a time and then repeat the entire maintenance process until
            // we'll be sure to have run out of files to process for sure...
            const int MAX_NUMBER_OF_PDF_FILES_TO_PROCESS = 30;
            const int MIN_NUMBER_OF_PDF_FILES_TO_PROCESS_PER_ITERATION = 10;
            const int MAX_SECONDS_PER_ITERATION = 10 * 60;
            long clk_bound = clk.ElapsedMilliseconds + MAX_SECONDS_PER_ITERATION * 1000;

            try
            {
                // If this library is busy, skip it for now
                if (Library.IsBusyAddingPDFs || Library.IsBusyRegeneratingTags)
                {
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Not daemon processing any library that is busy with adds...");
                    return;
                }

                if (ShutdownableManager.Instance.IsShuttingDown)
                {
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Breaking out of outer processing loop due to application termination");
                    return;
                }

                if (Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
                {
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Breaking out of outer processing loop due to DisableAllBackgroundTasks");
                    return;
                }

                if (!ConfigurationManager.IsEnabled("SuggestingMetadata"))
                {
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Breaking out of outer processing loop due to SuggestingMetadata=false");
                    return;
                }

                // Check that we have something to do
                List<PDFDocument> pdf_documents = web_library_detail.Xlibrary.PDFDocuments;
                stats.totalDocumentCount = pdf_documents.Count;
                stats.currentdocumentIndex = 0;
                stats.documentsProcessedCount = 0;
                foreach (PDFDocument pdf_document in pdf_documents)
                {
                    int needs_processing = 0;

                    stats.currentdocumentIndex++;

                    // there's nothing to infer from PDF when there's no PDF to process:
                    if (!pdf_document.DocumentExists)
                    {
                        continue;
                    }

                    if (PDFMetadataInferenceFromPDFMetadata.NeedsProcessing(pdf_document))  needs_processing |= 0x01;
                    if (PDFMetadataInferenceFromOCR.NeedsProcessing(pdf_document))          needs_processing |= 0x02;
                    if (PDFMetadataInferenceFromBibTeXSearch.NeedsProcessing(pdf_document)) needs_processing |= 0x04;

                    if (needs_processing != 0)
                    {
                        pdfs_retry_count.TallyOne(pdf_document.Fingerprint);
                        int cnt = pdfs_retry_count.GetCount(pdf_document.Fingerprint);
                        if (!General.IsPowerOfTwo(cnt))
                        {
                            needs_processing = 0;  // skip this time around
                        }
#if true
                        // Reset counter when it has run up to 64 (which means 6 attempts were made up to now).
                        if (cnt > 64)
                        {
                            pdfs_retry_count.ResetTally(pdf_document.Fingerprint);
                        }
#endif
                    }

                    // Previous check calls MAY take some serious time, hence we SHOULD check again whether
                    // the user decided to exit Qiqqa before we go on and do more time consuming work.
                    if (ShutdownableManager.Instance.IsShuttingDown)
                    {
                        Logging.Debug特("Breaking out of MetadataExtractionDaemon PDF fingerprinting loop due to daemon termination");
                        return;
                    }

                    if (needs_processing != 0)
                    {
                        if (DoSomeWork(web_library_detail, pdf_document, stats))
                        {
                            stats.documentsProcessedCount++;
                        }
                    }

                    // Limit the number of source files to process before we go and create/update
                    // a sane (though tiny and incomplete) Lucene search index database so that
                    // we have some up-to-date results ready whenever the user exits the Qiqqa application
                    // while this process is still running.
                    // When the user keeps Qiqqa running, this same approach will help us to 'update'
                    // the search index a bunch of files at a time, so everyone involved will be able
                    // to see progress happening after losing the index due to some fatal crash or
                    // forced re-index request.
                    if ((stats.documentsProcessedCount + 1) % MAX_NUMBER_OF_PDF_FILES_TO_PROCESS == 0)
                    {
                        Logging.Debug特("Interupting the MetadataExtractionDaemon PDF fingerprinting loop due to MAX_NUMBER_OF_PDF_FILES_TO_PROCESS reached");

                        callback_after_some_work_done();
                    }

                    // A timeout should only kick in when we have *some* work done already or
					// we would have introduced a subtle bug for very large libraries: if the timeout
					// is short enough for the library scan to take that long on a slow machine,
					// the timeout would, by itself, cause no work to be done, *ever*.
					// Hence we require a minimum amount of work done before the timeout condition
					// is allowed to fire.
                    if (clk_bound <= clk.ElapsedMilliseconds && stats.documentsProcessedCount >= MIN_NUMBER_OF_PDF_FILES_TO_PROCESS_PER_ITERATION)
                    {
                        Logging.Debug特("Breaking out of MetadataExtractionDaemon PDF fingerprinting loop due to MAX_SECONDS_PER_ITERATION: {0} ms consumed", clk.ElapsedMilliseconds);
                        return;
                    }
                }
            }
            finally
            {
                if (0 < stats.documentsProcessedCount)
                {
                    Logging.Debug特("Got {0} items of metadata extraction work done.", stats.documentsProcessedCount);
                }
                else
                {
                    // nothing to do.
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Breaking out of outer processing loop due to no more files to process right now.");

                    // when there's nothing to do, reset the retry tallying by doing a hard reset:
                    // the idea here being that delaying any retries on pending items is useless when
                    // there's nothing to do otherwise.
                    pdfs_retry_count = new CountingDictionary<string>();   // quickest and cleanest reset is a re-init (+ GarbageCollect of the old dict)
                }

                Logging.Info("{0}ms were spent to extract metadata", clk.ElapsedMilliseconds);
                StatusManager.Instance.ClearStatus("AutoSuggestMetadata");

                callback_after_some_work_done();
            }
        }

        private bool DoSomeWork(WebLibraryDetail web_library_detail, PDFDocument pdf_document, RunningStatistics stats)
        {
            if (ShutdownableManager.Instance.IsShuttingDown)
            {
                Logging.Debug特("Breaking out of MetadataExtractionDaemon PDF processing loop due to daemon termination");
                return false;
            }

            // Start rendering the first page so we can do some extraction
            try
            {
                //if (pdf_document.DocumentExists) -- already tested in collection loop above
                pdf_document.GetOCRText(1);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was an exception while requesting the first page to be OCRed while processing document {0}", pdf_document.Fingerprint);
            }

            StatusManager.Instance.UpdateStatus("AutoSuggestMetadata", "Suggesting metadata", stats.currentdocumentIndex, stats.totalDocumentCount, true);
            if (StatusManager.Instance.IsCancelled("AutoSuggestMetadata"))
            {
                return false;
            }

            // Try get the authors and year with the PDF in-file metadata
            try
            {
                PDFMetadataInferenceFromPDFMetadata.InferFromPDFMetadata(pdf_document);
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem in PDFMetadataInferenceFromPDFMetadata.InferFromPDFMetadata while processing document {0}", pdf_document.Fingerprint);
            }

            // Try looking for the title in the OCR
            try
            {
                PDFMetadataInferenceFromOCR.InferTitleFromOCR(pdf_document);
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem in PDFMetadataInferenceFromOCR.InferTitleFromOCR while processing document {0}", pdf_document.Fingerprint);
            }

            // Try suggesting some bibtex from bibtexsearch.com
            try
            {
                PDFMetadataInferenceFromBibTeXSearch.InferBibTeX(pdf_document, false);
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem in PDFMetadataInferenceFromOCR.InferTitleFromOCR while processing document {0}", pdf_document.Fingerprint);
            }

            return true;
        }
    }
}