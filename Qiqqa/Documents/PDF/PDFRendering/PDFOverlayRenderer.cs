using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qiqqa.Common.Common;
using Qiqqa.Common.Configuration;
using Utilities.GUI;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;

namespace Qiqqa.Documents.PDF.PDFRendering
{
    class PDFOverlayRenderer
    {   
        public static Bitmap RenderHighlights(int width, int height, PDFDocument pdf_document, int page)
        {
            // Render onto a scratch image in solid
            Bitmap bitmap = new Bitmap(width, height);     // <--- must b Dispose()d by caller

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                double last_right = Double.NegativeInfinity;
                double last_top = Double.NegativeInfinity;
                double last_bottom = Double.NegativeInfinity;
                PointF[] adjoinment_points = new PointF[4];

                foreach (PDFHighlight highlight in pdf_document.Highlights.GetHighlightsForPage(page))
                {
                    using (Brush highlight_pen = new SolidBrush(StandardHighlightColours.GetColor_Drawing(highlight.Color)))
                    {
                        graphics.FillRectangle(highlight_pen, (float)(highlight.Left * width), (float)(highlight.Top * height), (float)(highlight.Width * width), (float)(highlight.Height * height));

                        // Do some adjoining
                        if (Math.Abs(last_right - highlight.Left) < highlight.Height * 0.75 && Math.Abs(last_top - highlight.Top) < highlight.Height * 0.75 && Math.Abs(last_bottom - highlight.Bottom) < highlight.Height * 0.75)
                        {
                            // 0 -- 1
                            // |    |
                            // 3 -- 2

                            adjoinment_points[0].X = (float)(last_right * width);
                            adjoinment_points[0].Y = (float)(last_top * height);

                            adjoinment_points[1].X = (float)(highlight.Left * width);
                            adjoinment_points[1].Y = (float)(highlight.Top * height);

                            adjoinment_points[2].X = (float)(highlight.Left * width);
                            adjoinment_points[2].Y = (float)(highlight.Bottom * height);

                            adjoinment_points[3].X = (float)(last_right * width);
                            adjoinment_points[3].Y = (float)(last_bottom * height);

                            graphics.FillPolygon(highlight_pen, adjoinment_points);
                        }

                        // Remember the last position for future potential adjoining
                        last_right = highlight.Right;
                        last_top = highlight.Top;
                        last_bottom = highlight.Bottom;
                    }
                }
            }

            return bitmap;
        }

        public static void RenderHighlights(Image image, PDFDocument pdf_document, int page)
        {
            using (Bitmap scratch = RenderHighlights(image.Width, image.Height, pdf_document, page))
            {
                // Then render scratch onto target in transparent            
                var color_matrix = new ColorMatrix();
                color_matrix.Matrix33 = (float)ConfigurationManager.Instance.ConfigurationRecord.GUI_AnnotationPrintTransparency;
                using (var image_attributes = new ImageAttributes())
                {
                    image_attributes.SetColorMatrix(color_matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    using (Graphics graphics = Graphics.FromImage(image))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.DrawImage(
                            scratch,
                            new Rectangle(0, 0, scratch.Width, scratch.Height),
                            0,
                            0,
                            scratch.Width,
                            scratch.Height,
                            GraphicsUnit.Pixel,
                            image_attributes
                        );
                    }
                }
            }
        }

        public static void RenderAnnotations(Image image, PDFDocument pdf_document, int page, PDFAnnotation specific_pdf_annotation)
        {
            int TRANSPARENCY = (int)Math.Round(ConfigurationManager.Instance.ConfigurationRecord.GUI_AnnotationPrintTransparency * 255);

            using (Graphics graphics = Graphics.FromImage(image))
            {
                foreach (PDFAnnotation pdf_annotation in pdf_document.GetAnnotations())
                {
                    if (pdf_annotation.Deleted)
                    {
                        continue;
                    }

                    // Must be same page
                    if (pdf_annotation.Page != page)
                    {
                        continue;
                    }

                    // If they want specifics, must be same annotation
                    if (null != specific_pdf_annotation && pdf_annotation != specific_pdf_annotation)
                    {
                        continue;
                    }

                    // If we get here, do it!
                    using (Brush highlight_pen = new SolidBrush(Color.FromArgb(TRANSPARENCY, ColorTools.ConvertWindowsToDrawingColor(pdf_annotation.Color))))
                    { 
                        graphics.FillRectangle(highlight_pen, (float)(pdf_annotation.Left * image.Width), (float)(pdf_annotation.Top * image.Height), (float)(pdf_annotation.Width * image.Width), (float)(pdf_annotation.Height * image.Height));
                    }
                }
            }
        }

        public static void RenderInks(Image image, PDFDocument pdf_document, int page)
        {
            StrokeCollection stroke_collection = pdf_document.Inks.GetInkStrokeCollection(page);
            if (null != stroke_collection)
            {
                InkCanvas ic = new InkCanvas();
                ic.Width = 1000;
                ic.Height = 1000;
                ic.Strokes = stroke_collection;

                RenderTargetBitmap ink_image = new RenderTargetBitmap(1000, 1000, 96, 96, PixelFormats.Default);
                ink_image.Render(ic);
                using (MemoryStream ms = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(ink_image));
                    encoder.Save(ms);

                    using (Bitmap bitmap = new Bitmap(ms))
                    {
                        using (Graphics graphics = Graphics.FromImage(image))
                        {
                            graphics.DrawImage(bitmap, 0, 0, image.Width, image.Height);
                        }
                    }
                }
            }
        }
    }
}
