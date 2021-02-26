using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Utilities;
using Utilities.GUI;
using Utilities.PDF;
using Utilities.PDF.MuPDF;
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
                var metadata = MuPDFRenderer.GetDocumentMetaInfo(filename, password, ProcessPriorityClass.Normal);
                int page_count = metadata?.PageCount ?? (metadata.DocumentIsCorrupted ? -3 : -1);
                Logging.Debug("-CountPDFPages_MuPDF '{1}' -> ({0} pages)", page_count, filename);
                return page_count;
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Error while counting pages in CountPDFPages_MuPDF for file: {0}", filename);
                return -1;
            }
        }
    }
}
