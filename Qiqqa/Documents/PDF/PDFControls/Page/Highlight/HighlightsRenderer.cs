using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF.PDFRendering;
using Utilities.Images;
using Image = System.Windows.Controls.Image;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Highlight
{
    [Obfuscation(Feature = "renaming", ApplyToMembers = false)]
    public class HighlightsRenderer : Image
    {
        int page;
        PDFDocument pdf_document;        

        public HighlightsRenderer()
        {
            this.Opacity = ConfigurationManager.Instance.ConfigurationRecord.GUI_HighlightScreenTransparency;
            SizeChanged += HighlightsRenderer_SizeChanged;
            this.Stretch = Stretch.Fill;
        }

        void HighlightsRenderer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RebuildVisual();
        }

        internal void RebuildVisual(PDFDocument pdf_document, int page)
        {
            this.page = page;
            this.pdf_document = pdf_document;

            RebuildVisual();
        }

        
        public void RebuildVisual()
        {
            //Logging.Info("+HighlightsRenderer RebuildVisual() on page {0}", page);

            if (null == pdf_document)
            {
                this.Source = null;
                return;
            }

            if (double.IsNaN(this.Width) || double.IsNaN(this.Height))
            {
                this.Source = null;
                return;
            }

            // We use a smaller image than necessary as we do not need high resolution to represent the highlights
            double scaled_capped_width = Math.Min(this.Width, 300);
            double scaled_capped_height = this.Height * scaled_capped_width / this.Width;
            Bitmap raster_bitmap = PDFOverlayRenderer.RenderHighlights((int)scaled_capped_width, (int)scaled_capped_height, pdf_document, page);
            this.Source = BitmapImageTools.FromBitmap(raster_bitmap);

            //Logging.Info("-HighlightsRenderer RebuildVisual() on page {0}", page);
        }
    }
}
