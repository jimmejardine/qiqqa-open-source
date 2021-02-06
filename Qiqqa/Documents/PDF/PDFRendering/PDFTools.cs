using System;
using System.IO;
using System.Text.RegularExpressions;
using Utilities;
using Utilities.GUI;
using Utilities.PDF;
using Utilities.PDF.Sorax;

namespace Qiqqa.Documents.PDF.PDFRendering
{
    public static class PDFTools
    {
        public static int CountPDFPages(string filename)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            // Try multiple approaches because each fails in their own special way........
            int pages = 0;

#if false
            if (0 == pages)
                {
                    pages = CountPDFPages_Sorax(filename);
                }
            Logging.Debug("-CountPDFPages #1 = Sorax :: {0} : {1}", filename, pages);
#endif
#if SYNCFUSION_ANTIQUE
            if (0 == pages)
            {
                pages = CountPDFPages_Syncfusion(filename);
            }
            Logging.Debug("-CountPDFPages #2 = Syncfusion :: {0} : {1}", filename, pages);
#endif
            if (0 == pages)
            {
                pages = CountPDFPages_Jimme_MEGA(filename);
            }
            Logging.Debug("-CountPDFPages #3 = Jimme :: {0} : {1}", filename, pages);

            return pages;
        }

#if false
        private static int CountPDFPages_Sorax(string filename)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            try
            {
                Logging.Debug("+CountPDFPages_Sorax: {0}", filename);
                int page_count = SoraxPDFRendererDLLWrapper.GetPageCount(filename, null, null);
                Logging.Debug("-CountPDFPages_Sorax ({0} pages)", page_count);
                return page_count;
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Error while counting pages in CountPDFPages_Sorax for file: {0}", filename);
                return 0;
            }
        }
#endif

        private static int CountPDFPages_Jimme_MEGA(string filename)
        {
            try
            {
                Logging.Debug("+CountPDFPages_Jimme_MEGA");
                string pdf_text = File.ReadAllText(filename);
                Logging.Debug("+CountPDFPages_Jimme_MEGA....read");
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regex.Matches(pdf_text);
                Logging.Debug("-CountPDFPages_Jimme_MEGA ({0} pages)", matches.Count);
                return matches.Count;
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Error while counting pages in CountPDFPages_Jimme_MEGA for file: {0}", filename);
                return 0;
            }
        }

#if SYNCFUSION_ANTIQUE

        private static int CountPDFPages_Syncfusion(string filename)
        {
            try
            {
                Logging.Debug("+CountPDFPages_Syncfusion: {0}", filename);
                using (AugmentedPdfLoadedDocument doc = new AugmentedPdfLoadedDocument(filename))
                {
                    int pages = doc.Pages.Count;
                    Logging.Debug("-CountPDFPages_Syncfusion ({0} pages)", pages);
                    return pages;
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Error while counting pages in CountPDFPages_Syncfusion for file: {0}", filename);
                return 0;
            }
        }

#endif
    
    }
}
