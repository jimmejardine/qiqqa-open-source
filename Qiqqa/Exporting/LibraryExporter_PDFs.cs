using System;
using System.Collections.Generic;
using System.Text;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Annotations;
using PdfSharp.Pdf.IO;
using Qiqqa.Common.Common;
using Qiqqa.Common.TagManagement;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Misc;

namespace Qiqqa.Exporting
{
    internal class LibraryExporter_PDFs
    {
        internal static void Export(WebLibraryDetail web_library_detail, string base_path, Dictionary<string, PDFDocumentExportItem> pdf_document_export_items)
        {
            List<PDFDocumentExportItem> pdf_document_export_items_values = new List<PDFDocumentExportItem>(pdf_document_export_items.Values);
            for (int i = 0; i < pdf_document_export_items_values.Count; ++i)
            {
                var item = pdf_document_export_items_values[i];
                try
                {
                    StatusManager.Instance.UpdateStatus("ContentExport", String.Format("Exporting entry {0} of {1}", i, pdf_document_export_items_values.Count), i, pdf_document_export_items_values.Count);

                    using (PdfDocument document = PdfReader.Open(item.filename, PdfDocumentOpenMode.Modify))
                    {
                        // First the properties
                        document.Info.Title = item.pdf_document.TitleCombined;
                        document.Info.Author = item.pdf_document.AuthorsCombined;
                        document.Info.ModificationDate = DateTime.Now;
                        document.Info.Creator = "Qiqqa.com library export v1";
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (string tag in TagTools.ConvertTagBundleToTags(item.pdf_document.Tags))
                            {
                                sb.Append(tag);
                                sb.Append(";");
                            }
                            document.Info.Keywords = sb.ToString();
                        }

                        // Then the main comments (if any)
                        {
                            string comments = item.pdf_document.Comments;
                            if (!String.IsNullOrEmpty(comments))
                            {
                                PdfPage page = document.Pages[0];

                                using (XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend))
                                {

                                    XRect rect = new XRect(5, 5, 100, 100);
                                    PdfTextAnnotation new_annotation = new PdfTextAnnotation();
                                    new_annotation.Contents = comments;
                                    new_annotation.Icon = PdfTextAnnotationIcon.Note;
                                    new_annotation.Rectangle = new PdfRectangle(gfx.Transformer.WorldToDefaultPage(rect));
                                    page.Annotations.Add(new_annotation);
                                }
                            }
                        }

                        // Then the highlights
                        foreach (PDFHighlight highlight in item.pdf_document.Highlights.GetAllHighlights())
                        {
                            PdfPage page = document.Pages[highlight.Page - 1];
                            using (XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend))
                            {
                                XColor color = XColor.FromArgb(StandardHighlightColours.GetColor(highlight.Color));
                                XPen pen = new XPen(color);
                                XBrush brush = new XSolidBrush(color);
                                XRect rect = new XRect(highlight.Left * gfx.PageSize.Width, highlight.Top * gfx.PageSize.Height, highlight.Width * gfx.PageSize.Width, highlight.Height * gfx.PageSize.Height);
                                gfx.DrawRectangle(brush, rect);
                            }

                        }

                        // Then the annotations
                        foreach (PDFAnnotation annotation in item.pdf_document.GetAnnotations())
                        {
                            if (annotation.Deleted)
                            {
                                continue;
                            }

                            PdfPage page = document.Pages[annotation.Page - 1];
                            using (XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend))
                            {
                                XColor color = XColor.FromArgb(annotation.Color);
                                XPen pen = new XPen(color);
                                XBrush brush = new XSolidBrush(color);
                                XRect rect = new XRect(annotation.Left * gfx.PageSize.Width, annotation.Top * gfx.PageSize.Height, annotation.Width * gfx.PageSize.Width, annotation.Height * gfx.PageSize.Height);
                                gfx.DrawRectangle(brush, rect);

                                PdfTextAnnotation new_annotation = new PdfTextAnnotation();
                                new_annotation.Contents = String.Format("Tags:{0}\n{1}", annotation.Tags, annotation.Text);
                                new_annotation.Icon = PdfTextAnnotationIcon.Note;
                                new_annotation.Color = color;
                                new_annotation.Opacity = 0.5;
                                new_annotation.Open = false;
                                new_annotation.Rectangle = new PdfRectangle(gfx.Transformer.WorldToDefaultPage(rect));
                                page.Annotations.Add(new_annotation);

                                //foreach (var pair in new_annotation.Elements)
                                //{
                                //    Logging.Debug("   {0}={1}", pair.Key, pair.Value);
                                //}
                            }
                        }

                        // Save it
                        Logging.Info("Saving {0}", item.filename);
                        document.Save(item.filename);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error updating PDF for " + item.filename);
                }

                StatusManager.Instance.UpdateStatus("ContentExport", String.Format("Exported your PDF content"));
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            PdfDocument document = PdfReader.Open(@"C:\temp\pdfwithhighlights.pdf", PdfDocumentOpenMode.ReadOnly);
            PdfPage page = document.Pages[0];
            foreach (PdfAnnotation annotation in page.Annotations)
            {
                foreach (var pair in annotation.Elements)
                {
                    Logging.Debug("   {0}={1}", pair.Key, pair.Value);
                }
            }
        }
#endif

        #endregion
    }
}
