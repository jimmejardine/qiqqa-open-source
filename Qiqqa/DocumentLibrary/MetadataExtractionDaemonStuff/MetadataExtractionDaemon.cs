using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.MetadataSuggestions;
using Utilities;
using Utilities.Collections;
using Utilities.GUI;
using Utilities.Mathematics;
using Utilities.Misc;

namespace Qiqqa.DocumentLibrary.MetadataExtractionDaemonStuff
{
    internal class MetadataExtractionDaemon
    {
        private CountingDictionary<string> pdfs_retry_count = new CountingDictionary<string>();

        public void DoMaintenance(Library library)
        {
            Stopwatch clk = Stopwatch.StartNew();

            Logging.Debug特("MetadataExtractionDaemon::DoMaintenance START");

            // To recover from a search index fatal failure and re-indexing attempt for very large libraries,
            // we're better off processing a limited number of source files as we'll be able to see 
            // *some* results more quickly and we'll have a working, though yet incomplete,
            // index in *reasonable time*.
            //
            // To reconstruct the entire index will take a *long* time. We grow the index and other meta
            // stores a bunch-of-files at a time and then repeat the entire maintenance process until
            // we'll be sure to have run out of files to process for sure...
            const int MAX_NUMBER_OF_PDF_FILES_TO_PROCESS = 100;
            const int MAX_SECONDS_PER_ITERATION = 60;
            long clk_bound = clk.ElapsedMilliseconds + MAX_SECONDS_PER_ITERATION * 1000;

            while (true)
            {
                // If this library is busy, skip it for now
                if (Library.IsBusyAddingPDFs || Library.IsBusyRegeneratingTags)
                {
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Not daemon processing any library that is busy with adds...");
                    break;
                }

                if (clk_bound <= clk.ElapsedMilliseconds)
                {
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Breaking out of outer processing loop due to MAX_SECONDS_PER_ITERATION: {0} ms consumed", clk.ElapsedMilliseconds);
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

                    List<PDFDocument> pdf_documents = library.PDFDocuments;
                    foreach (PDFDocument pdf_document in pdf_documents)
                    {
                        bool needs_processing = false;

                    // there's nothing to infer from PDF when there's no PDF to process:
                    if (!pdf_document.DocumentExists)
                    {
                        continue;
                    }

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
                        if (clk_bound <= clk.ElapsedMilliseconds)
                        {
                            Logging.Debug特("Breaking out of MetadataExtractionDaemon PDF fingerprinting loop due to MAX_SECONDS_PER_ITERATION: {0} ms consumed", clk.ElapsedMilliseconds);
                            break;
                        }
                    }

                    if (0 < pdfs_to_process.Count)
                    {
                        Logging.Debug特("Got {0} items of metadata extraction work", pdfs_to_process.Count);
                    }
                else
                {
                    // nothing to do due to timeout
                    Logging.Debug特("MetadataExtractionDaemon::DoMaintenance: Breaking out of outer processing loop due to no more files to process right now.");
                    break;
                }

                // Get each of our guys to start rendering their first pages so we can do some extraction
                for (int i = 0; i < pdfs_to_process.Count; ++i)
                {
                    PDFDocument pdf_document = pdfs_to_process[i];
                
                    if (Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                    {
                        Logging.Debug特("Breaking out of MetadataExtractionDaemon PDF processing loop due to daemon termination");
                        break;
                    }

                    try
                    {
                        //if (pdf_document.DocumentExists) -- already tested in collection loop above
                            pdf_document.PDFRenderer.GetOCRText(1);
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was an exception while requesting the first page to be OCRed");
                    }
                
                    StatusManager.Instance.UpdateStatus("AutoSuggestMetadata", "Suggesting metadata", i, pdfs_to_process.Count, true);
                    if (StatusManager.Instance.IsCancelled("AutoSuggestMetadata"))
                    {
                        break;
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
                }

                // nap a short while between mini-runs:
                WPFDoEvents.WaitForUIThreadActivityDone();
            }

            Logging.Info("{0}ms were spent to extract metadata", clk.ElapsedMilliseconds);
            StatusManager.Instance.ClearStatus("AutoSuggestMetadata");
        }
    }
}