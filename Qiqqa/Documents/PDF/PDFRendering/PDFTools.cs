using System;
using System.IO;
using System.Text.RegularExpressions;
using Utilities;
using Utilities.PDF;
using Utilities.PDF.Sorax;

namespace Qiqqa.Documents.PDF.PDFRendering
{
    public class PDFTools
    {
        public static int CountPDFPages(string filename)
        {
            // Try multiple approaches because each fails in their own special way........
            int pages = 0;

            try
            {
                if (0 == pages)
                {
                    pages = CountPDFPages_Sorax(filename);
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Fail trying to count pages using Sorax");
            }

            try
            {
                if (0 == pages)
                {
                    pages = CountPDFPages_Syncfusion(filename);
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Fail trying to count pages using Syncfusion");
            }

            try
            {
                if (0 == pages)
                {
                    pages = CountPDFPages_Jimme_MEGA(filename);
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Fail trying to count pages using Jimme");
            }

            return pages;
        }

        private static int CountPDFPages_Sorax(string filename)
        {
            try
            {
                Logging.Debug("+CountPDFPages_Sorax: {0}", filename);
                int page_count = SoraxPDFRendererDLLWrapper.GetPageCount(filename, null, null);
                Logging.Debug("-CountPDFPages_Sorax ({0} pages)", page_count);
                return page_count;
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Error while counting pages in CountPDFPages_Sorax");
                return 0;
            }
        }

        private static int CountPDFPages_Jimme_MEGA(string filename)
        {
            try
            {
                Logging.Debug("+CountPDFPages_Jimme");
                string pdf_text = File.ReadAllText(filename);
                Logging.Debug("+CountPDFPages_Jimme....read");
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regex.Matches(pdf_text);
                Logging.Debug("-CountPDFPages ({0} pages)", matches.Count);
                return matches.Count;
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Error while counting pages in CountPDFPages_Jimme");
                return 0;
            }
        }

        private static int CountPDFPages_Syncfusion(string filename)
        {
            try
            {
                Logging.Debug("+CountPDFPages_Syncfusion");
                using (AugmentedPdfLoadedDocument doc = new AugmentedPdfLoadedDocument(filename))
                {
                    int pages = doc.Pages.Count;
                    Logging.Debug("-CountPDFPages_Syncfusion ({0} pages)", pages);
                    return pages;
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Error while counting pages in CountPDFPages_Syncfusion");
                return 0;
            }
        }
    }
}
