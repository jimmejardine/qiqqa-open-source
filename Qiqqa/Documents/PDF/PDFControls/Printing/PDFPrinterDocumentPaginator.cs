using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qiqqa.Documents.PDF.PDFRendering;
using Utilities;
using Utilities.GUI;
using Utilities.Images;
using Utilities.Misc;
using Size = System.Windows.Size;

namespace Qiqqa.Documents.PDF.PDFControls.Printing
{
    internal class PDFPrinterDocumentPaginator : DocumentPaginator, IDisposable
    {
        private PDFDocument pdf_document;
        private int page_from;
        private int page_to;
        private Size page_size;
        private int total_pages_printed = 0;

        public PDFPrinterDocumentPaginator(PDFDocument pdf_document, int page_from, int page_to, Size page_size)
        {
            this.pdf_document = pdf_document;
            this.page_from = page_from;
            this.page_to = page_to;
            this.page_size = page_size;

            StatusManager.Instance.ClearCancelled("PDFPrinter");
        }

        private DocumentPage last_document_page = null;
        public override DocumentPage GetPage(int page_zero_based)
        {
            last_document_page?.Dispose();
            last_document_page = null;

            int page = page_from + page_zero_based;

            StatusManager.Instance.UpdateStatus("PDFPrinter", String.Format("Printing page {0} of {1}", page_zero_based + 1, PageCount), page_zero_based + 1, PageCount, true);

            // Render a page at 300 DPI...
            using (MemoryStream ms = new MemoryStream(pdf_document.GetPageByDPIAsImage(page, 300)))
            {
                using (Image image = Image.FromStream(ms))
                {
                    PDFOverlayRenderer.RenderAnnotations(image, pdf_document, page, null);
                    PDFOverlayRenderer.RenderHighlights(image, pdf_document, page);
                    PDFOverlayRenderer.RenderInks(image, pdf_document, page);
                    BitmapSource image_page = BitmapImageTools.CreateBitmapSourceFromImage(image);
                    ASSERT.Test(image_page.IsFrozen);

                    DrawingVisual dv = new DrawingVisual();
                    using (DrawingContext dc = dv.RenderOpen())
                    {
                        // Rotate the image if its orientation does not match the printer
                        if (
                            page_size.Width < page_size.Height && image_page.Width > image_page.Height ||
                            page_size.Width > page_size.Height && image_page.Width < image_page.Height
                            )
                        {
                            image_page = new TransformedBitmap(image_page, new RotateTransform(90));
                            image_page.Freeze();
                        }

                        dc.DrawImage(image_page, new Rect(0, 0, page_size.Width, page_size.Height));
                    }

                    ++total_pages_printed;

                    last_document_page = new DocumentPage(dv);
                    return last_document_page;
                }
            }
        }

        public int TotalPagesPrinted => total_pages_printed;

        public override bool IsPageCountValid => true;

        public override int PageCount
        {
            get
            {
                // Check if we have been cancelled
                if (StatusManager.Instance.IsCancelled("PDFPrinter"))
                {
                    return total_pages_printed;
                }
                else
                {
                    return page_to - page_from + 1;
                }
            }
        }

        public override Size PageSize
        {
            get => page_size;
            set => page_size = value;
        }

        public override IDocumentPaginatorSource Source => null;

        #region --- IDisposable ------------------------------------------------------------------------

        ~PDFPrinterDocumentPaginator()
        {
            Logging.Debug("~PDFPrinterDocumentPaginator()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing PDFPrinterDocumentPaginator");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("PDFPrinterDocumentPaginator::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    // Get rid of managed resources / get rid of cyclic references:
                    pdf_document = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    last_document_page?.Dispose();
                    last_document_page = null;
                });

                ++dispose_count;
            });
        }

        #endregion

    }
}
