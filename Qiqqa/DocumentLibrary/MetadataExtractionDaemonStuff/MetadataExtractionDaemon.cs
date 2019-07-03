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

        public void DoMaintenance(Library library, Daemon daemon)
        {
            Stopwatch sw_total = new Stopwatch();
            sw_total.Start();            

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
                }

                if (0 < pdfs_to_process.Count)
                {
                    Logging.Debug("Got {0} items of metadata extraction work", pdfs_to_process.Count);
                }
            }

            // Get each of our guys to start rendering their first pages so we can do some extraction
            foreach (PDFDocument pdf_document in pdfs_to_process)
            {
                if (!daemon.StillRunning)
                {
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

                if (!daemon.StillRunning)
                {
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
        }

    }
}