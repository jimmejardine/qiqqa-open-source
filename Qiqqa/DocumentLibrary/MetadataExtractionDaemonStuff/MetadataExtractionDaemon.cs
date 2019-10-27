using System;
using System.Collections.Generic;
using System.Diagnostics;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Collections;
using Utilities.Mathematics;
using Utilities.Misc;
using Utilities.PDF;
using Qiqqa.Documents.PDF.MetadataSuggestions;

namespace Qiqqa.DocumentLibrary.MetadataExtractionDaemonStuff
{
    class MetadataExtractionDaemon
    {
        CountingDictionary<string> pdfs_retry_count = new CountingDictionary<string>();

        public void DoMaintenance(Library library)
        {
            Stopwatch sw_total = new Stopwatch();
            sw_total.Start();

            Logging.Debug特("MetadataExtractionDaemon::DoMaintenance START");

            // To recover from a search index fatal failure and re-indexing attempt for very large libraries,
            // we're better off processing a limited number of source files as we'll be able to see 
            // *some* results more quickly and we'll have a working, though yet incomplete,
            // index in *reasonable time*.
            //
            // To reconstruct the entire index will take a *long* time. We grow the index and other meta
            // stores a bunch-of-files at a time and then repeat the entire maintenance process until
            // we'll be sure to have run out of files to process for sure...
            const int MAX_NUMBER_OF_PDF_FILES_TO_PROCESS = 10;
            const int MAX_SECONDS_PER_ITERATION = 15;
            DateTime index_processing_start_time = DateTime.UtcNow;

            while (true)
            {
                // If this library is busy, skip it for now
                if (Library.IsBusyAddingPDFs)
                {
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Not daemon processing any library that is busy with adds...");
                    break;
                }

                if (DateTime.UtcNow.Subtract(index_processing_start_time).TotalSeconds > MAX_SECONDS_PER_ITERATION)
                {
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Breaking out of outer processing loop due to MAX_SECONDS_PER_ITERATION: {0} seconds consumed", DateTime.UtcNow.Subtract(index_processing_start_time).TotalSeconds);
                    break;
                }

                if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                {
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Breaking out of outer processing loop due to application termination");
                    break;
                }

                if (Common.Configuration.ConfigurationManager.Instance.ConfigurationRecord.DisableAllBackgroundTasks)
                {
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Breaking out of outer processing loop due to DisableAllBackgroundTasks");
                    break;
                }

                // Check that we have something to do
                List<PDFDocument> pdfs_to_process = new List<PDFDocument>();
                {
                    List<PDFDocument> pdf_documents = library.PDFDocuments;
                    foreach (PDFDocument pdf_document in pdf_documents)
                    {
                        bool needs_processing = false;
                        if (PDFMetadataInferenceFromPDFMetadata.NeedsProcessing(pdf_document)) needs_processing = true;
                        if (PDFMetadataInferenceFromOCR.NeedsProcessing(pdf_document)) needs_processing = true;
                        if (PDFMetadataInferenceFromBibTeXSearch.NeedsProcessing(pdf_document)) needs_processing = true;

                        if (needs_processing)
                        {
                            pdfs_retry_count.TallyOne(pdf_document.Fingerprint);
                            if (General.IsPowerOfTwo(pdfs_retry_count.GetCount(pdf_document.Fingerprint)))
                            {
                                pdfs_to_process.Add(pdf_document);
                            }
                        }

                        if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                        {
                            Logging.Debug特("Breaking out of MetadataExtractionDaemon PDF fingerprinting loop due to daemon termination");
                            break;
                        }

                        // Limit the number of source files to process at once or we won't have recreated
                        // a sane (though tiny and incomplete) Lucene search index database by the time 
                        // the user exits the Qiqqa application in a minute or so.
                        // When the user keeps Qiqqa running, this same approach will help us to 'update'
                        // the search index a bunch of files at a time, so everyone involved will be able
                        // to see progress happening after losing the index due to some fatal crash or
                        // forced re-index request.
                        if (pdfs_to_process.Count >= MAX_NUMBER_OF_PDF_FILES_TO_PROCESS)
                        {
                            Logging.Debug特("Breaking out of MetadataExtractionDaemon PDF fingerprinting loop due to MAX_NUMBER_OF_PDF_FILES_TO_PROCESS reached");
                            break;
                        }
                    }

                    if (0 < pdfs_to_process.Count)
                    {
                        Logging.Debug特("Got {0} items of metadata extraction work", pdfs_to_process.Count);
                    }
                }

                // Get each of our guys to start rendering their first pages so we can do some extraction
                foreach (PDFDocument pdf_document in pdfs_to_process)
                {
                    if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                    {
                        Logging.Debug特("Breaking out of MetadataExtractionDaemon PDF processing loop due to daemon termination");
                        break;
                    }

                    try
                    {
                        if (pdf_document.DocumentExists)
                        {
                            pdf_document.PDFRenderer.GetOCRText(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was an exception while requesting the first page to be OCRed");
                    }
                }

                // See if there is any completed OCR to work with
                if (0 < pdfs_to_process.Count)
                {
                    StatusManager.Instance.ClearCancelled("AutoSuggestMetadata");
                }

                for (int i = 0; i < pdfs_to_process.Count; ++i)
                {
                    StatusManager.Instance.UpdateStatusBusy("AutoSuggestMetadata", "Suggesting metadata", i, pdfs_to_process.Count, true);
                    if (StatusManager.Instance.IsCancelled("AutoSuggestMetadata"))
                    {
                        break;
                    }

                    if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                    {
                        Logging.Debug特("Breaking out of MetadataExtractionDaemon metadata suggesting loop due to daemon termination");
                        break;
                    }

                    PDFDocument pdf_document = pdfs_to_process[i];

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
                }

                if (0 < pdfs_to_process.Count)
                {
                    Logging.Info("It took a total of {0}ms to extract metadata", sw_total.ElapsedMilliseconds);
                    StatusManager.Instance.ClearStatus("AutoSuggestMetadata");
                }
                else
                {
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Breaking out of outer processing loop due to no more files to process (count = {0})", pdfs_to_process.Count);
                    break;
                }
            }
        }
    }
}