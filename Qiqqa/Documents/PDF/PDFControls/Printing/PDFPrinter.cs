using System;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.Documents.PDF.PDFRendering;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.PDFControls.Printing
{
    public static class PDFPrinter
    {
        public static void Print(PDFDocument pdf_document, PDFRenderer pdf_renderer, string description)
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            FeatureTrackingManager.Instance.UseFeature(Features.Document_Print);

            PrintDialog print_dialog = new PrintDialog();
            print_dialog.UserPageRangeEnabled = true;
            print_dialog.MinPage = 1;
            print_dialog.MaxPage = (uint)pdf_renderer.PageCount;
            if (print_dialog.ShowDialog() == true)
            {
                if (print_dialog.PageRangeSelection == PageRangeSelection.AllPages)
                {
                    PrintThreadEntry(print_dialog, pdf_document, pdf_renderer, description, (int)print_dialog.MinPage, (int)print_dialog.MaxPage);
                }
                else
                {
                    PrintThreadEntry(print_dialog, pdf_document, pdf_renderer, description, print_dialog.PageRange.PageFrom, print_dialog.PageRange.PageTo);
                }
            }
        }

        private static void PrintThreadEntry(PrintDialog print_dialog, PDFDocument pdf_document, PDFRenderer pdf_renderer, string description, int page_from, int page_to)
        {
            PDFPrinterDocumentPaginator paginator = new PDFPrinterDocumentPaginator(pdf_document, pdf_renderer, page_from, page_to, new Size(print_dialog.PrintableAreaWidth, print_dialog.PrintableAreaHeight));
            print_dialog.PrintDocument(paginator, description);
            StatusManager.Instance.UpdateStatus("PDFPrinter", String.Format("Finished printing {0} pages", paginator.TotalPagesPrinted));
        }
    }
}
