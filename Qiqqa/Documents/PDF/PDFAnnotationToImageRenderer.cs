using System.Drawing;
using System.IO;
using Qiqqa.Documents.PDF.PDFRendering;
using Utilities.GUI;
using Utilities.Images;

namespace Qiqqa.Documents.PDF
{
    internal class PDFAnnotationToImageRenderer
    {
        public static Image RenderAnnotation(PDFDocument pdf_document, PDFAnnotation pdf_annotation, int dpi)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            using (MemoryStream ms = new MemoryStream(pdf_document.PDFRenderer.GetPageByDPIAsImage(pdf_annotation.Page, dpi)))
            {
                Image image = Image.FromStream(ms);
                PDFOverlayRenderer.RenderHighlights(image, pdf_document, pdf_annotation.Page);
                PDFOverlayRenderer.RenderInks(image, pdf_document, pdf_annotation.Page);

                // We rescale in the Drawing.Bitmap world because the WPF world uses so much memory
                Image cropped_image = BitmapImageTools.CropImageRegion(image, pdf_annotation.Left, pdf_annotation.Top, pdf_annotation.Width, pdf_annotation.Height);
                return cropped_image;
            }
        }
    }
}
