using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Utilities;
using Utilities.GUI;
using Utilities.PDF;
using Utilities.PDF.MuPDF;

namespace Qiqqa.Documents.PDF.PDFRendering
{
    public static class PDFTools
    {
        public static int CountPDFPages(string filename, string password)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            try
            {
                Logging.Debug("+CountPDFPages_MuPDF: {0}", filename);
                int page_count;
                PDFDocumentMuPDFMetaInfo metadata = null;
                if (!File.Exists(filename))
                {
                    page_count = PDFErrors.DOCUMENT_DOES_NOT_EXIST;
                }
                else
                {
                    metadata = MuPDFRenderer.GetDocumentMetaInfo(filename, password, ProcessPriorityClass.Normal);
                    page_count = metadata?.PageCount ?? PDFErrors.PAGECOUNT_GENERAL_FAILURE;

                    if (page_count <= 0)
                    {
                        if (metadata?.DocumentIsCorrupted ?? false)
                        {
                            if (metadata.DocumentErrorCode < 0)
                            {
                                page_count = metadata.DocumentErrorCode;
                                Debug.Assert(page_count != PDFErrors.PAGECOUNT_PENDING);
                                //Debug.Assert(false);
                            }
                            else
                            {
                                page_count = PDFErrors.DOCUMENT_IS_CORRUPTED;
                            }
                        }
                    }
                }
                string error_report = metadata != null ? "\n" + String.Join("\n", metadata.errors.ToArray()) : "";
                Logging.Debug($"-CountPDFPages_MuPDF '{ filename }' -> ({ page_count } pages){0}", error_report);
                return page_count;
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Error while counting pages in CountPDFPages_MuPDF for file: {0}", filename);
                return PDFErrors.PAGECOUNT_GENERAL_FAILURE;
            }
        }
    }
}
