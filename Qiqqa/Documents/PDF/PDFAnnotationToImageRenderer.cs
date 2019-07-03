using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Qiqqa.Documents.PDF.PDFRendering;
using Utilities.Images;

namespace Qiqqa.Documents.PDF
{
    class PDFAnnotationToImageRenderer
    {
        public static Image RenderAnnotation(PDFDocument pdf_document, PDFAnnotation pdf_annotation, float dpi)
        {
            Image image = Image.FromStream(new MemoryStream(pdf_document.PDFRenderer.GetPageByDPIAsImage(pdf_annotation.Page, dpi)));
            PDFOverlayRenderer.RenderHighlights(image, pdf_document, pdf_annotation.Page);
            PDFOverlayRenderer.RenderInks(image, pdf_document, pdf_annotation.Page);

            // We rescale in the Drawing.Bitmap world because the WPF world uses so much memory
            Image cropped_image = BitmapImageTools.CropImageRegion(image, pdf_annotation.Left, pdf_annotation.Top, pdf_annotation.Width, pdf_annotation.Height);
            return cropped_image;
        }
    }
}
