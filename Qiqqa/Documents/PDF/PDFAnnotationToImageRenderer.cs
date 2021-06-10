using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Qiqqa.Documents.PDF.PDFRendering;
using Utilities.GUI;
using Utilities.Images;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Documents.PDF
{
    internal class PDFAnnotationToImageRenderer
    {
        public static Image RenderAnnotation(PDFDocument pdf_document, PDFAnnotation pdf_annotation, int dpi)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            using (MemoryStream ms = new MemoryStream(pdf_document.GetPageByHeightAsImage(pdf_annotation.Page, dpi * 12, dpi * 12)))
            {
                Image cropped_image = null;
                Image resized_image = null;
                Image image = Image.FromStream(ms);

                try
                {
#if false
                    // resize image to given dpi
                    int new_width = (int)Math.Round(image.Width * dpi / image.HorizontalResolution);
                    int new_height = (int)Math.Round(image.Height * dpi / image.VerticalResolution);
                    if (Math.Abs(new_width - image.Width) > 2 || Math.Abs(new_height - image.Height) > 2)
                    {
                        resized_image = BitmapImageTools.ResizeImage(image, new_width, new_height);
                        image.Dispose();
                        image = resized_image;
                        resized_image = null;
                    }
#endif

                    PDFOverlayRenderer.RenderHighlights(image, pdf_document, pdf_annotation.Page);
                    PDFOverlayRenderer.RenderInks(image, pdf_document, pdf_annotation.Page);

                    // We rescale in the Drawing.Bitmap world because the WPF world uses so much memory
                    cropped_image = BitmapImageTools.CropImageRegion(image, pdf_annotation.Left, pdf_annotation.Top, pdf_annotation.Width, pdf_annotation.Height);
                }
                finally
                {
                    image.Dispose();
                    resized_image?.Dispose();
                }

                return cropped_image;
            }
        }
    }
}
