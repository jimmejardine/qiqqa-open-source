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
        public const int PAGECOUNT_PENDING = -4;
        public const int PAGECOUNT_DOCUMENT_IS_CORRUPTED = -3;
        public const int PAGECOUNT_DOCUMENT_DOES_NOT_EXIST = -2;
        public const int PAGECOUNT_GENERAL_FAILURE = -1;

        public static int CountPDFPages(string filename, string password)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            try
            {
                Logging.Debug("+CountPDFPages_MuPDF: {0}", filename);
                var metadata = MuPDFRenderer.GetDocumentMetaInfo(filename, password, ProcessPriorityClass.Normal);
                int page_count = metadata?.PageCount ?? PAGECOUNT_GENERAL_FAILURE;
                if (page_count <= 0)
                {
                    if (!File.Exists(filename))
                    {
                        page_count = PAGECOUNT_DOCUMENT_DOES_NOT_EXIST;
                    }
                    else if (metadata.DocumentIsCorrupted)
                    {
                        page_count = PAGECOUNT_DOCUMENT_IS_CORRUPTED;
                    }
                }
                Logging.Debug("-CountPDFPages_MuPDF '{1}' -> ({0} pages)", page_count, filename);
                return page_count;
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Error while counting pages in CountPDFPages_MuPDF for file: {0}", filename);
                return PAGECOUNT_GENERAL_FAILURE;
            }
        }
    }
}
