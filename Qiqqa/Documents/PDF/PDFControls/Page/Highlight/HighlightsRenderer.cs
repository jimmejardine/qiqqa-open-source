using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF.PDFRendering;
using Utilities.GUI;
using Utilities.Images;
using Utilities.Misc;
using Image = System.Windows.Controls.Image;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Highlight
{
    public class HighlightsRenderer : Image
    {
        private int page;
        private PDFDocument pdf_document;

        public HighlightsRenderer()
        {
            Opacity = ConfigurationManager.Instance.ConfigurationRecord.GUI_HighlightScreenTransparency;
            SizeChanged += HighlightsRenderer_SizeChanged;
            Stretch = Stretch.Fill;
        }

        private void HighlightsRenderer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                RebuildVisual();
            });
        }

        internal void RebuildVisual(PDFDocument pdf_document, int page)
        {
            this.page = page;
            this.pdf_document = pdf_document;

            RebuildVisual();
        }


        public void RebuildVisual()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            //Logging.Info("+HighlightsRenderer RebuildVisual() on page {0}", page);

            if (null == pdf_document)
            {
                Source = null;
                return;
            }

            if (double.IsNaN(Width) || double.IsNaN(Height))
            {
                Source = null;
                return;
            }

            // We use a smaller image than necessary as we do not need high resolution to represent the highlights
            double scaled_capped_width = Math.Min(Width, 300);
            double scaled_capped_height = Height * scaled_capped_width / Width;

            SafeThreadPool.QueueUserWorkItem(() =>
            {
                BitmapSource bmp;

                using (Bitmap raster_bitmap = PDFOverlayRenderer.RenderHighlights((int)scaled_capped_width, (int)scaled_capped_height, pdf_document, page))
                {
                    bmp = BitmapImageTools.FromBitmap(raster_bitmap);
                }

                WPFDoEvents.InvokeAsyncInUIThread(() =>
                {
                    Source = bmp;
                });

                //Logging.Info("-HighlightsRenderer RebuildVisual() on page {0}", page);
            });
        }
    }
}
