using System;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using icons;
using Qiqqa.AnnotationsReportBuilding;
using Qiqqa.Common;
using Qiqqa.Common.GUI;
using Qiqqa.UtilisationTracking;
using Syncfusion.Pdf;
using Utilities;
using Utilities.Images;
using Utilities.Misc;
using Utilities.OCR;
using Utilities.PDF;
using Brushes = System.Windows.Media.Brushes;

namespace Qiqqa.Documents.PDF.PDFControls.PDFExporting
{
    /// <summary>
    /// Exports to a FlowDocument
    /// </summary>
    internal class ExportToWord
    {
        public static StandardFlowDocument DoExport(PDFDocument pdf_document)
        {
            StandardFlowDocument flow_document = new StandardFlowDocument();
            flow_document.Background = Brushes.White;

            using (AugmentedPdfLoadedDocument doc = new AugmentedPdfLoadedDocument(pdf_document.DocumentPath))
            {
                for (int page = 1; page <= pdf_document.PageCount; ++page)
                {
                    string msg = $"Exporting page {page}/{pdf_document.PageCountAsString}";

                    StatusManager.Instance.UpdateStatus("ExportToWord", msg, page, Math.Max(1, pdf_document.PageCount));

                    try
                    {
                        Logging.Info(msg);

                        // Add the header for the page
                        {
                            Bold bold = new Bold();
                            bold.Inlines.Add(String.Format("--- Page {0} ---", page));
                            flow_document.Blocks.Add(new Paragraph(bold));
                        }

                        StringBuilder sb = new StringBuilder();
                        WordList words = pdf_document.PDFRenderer.GetOCRText(page);
                        if (null != words)
                        {
                            Word last_word = null;

                            foreach (Word word in words)
                            {
                                // Add a newline if we seem to have a linebreak
                                if (
                                        (
                                            (null != last_word && last_word.Left > word.Left) &&
                                            (word.Top > last_word.Top + 2 * last_word.Height) &&
                                            (word.Text.Length > 0 && !Char.IsLower(word.Text[0]))
                                        )
                                    ||
                                        (
                                            (null != last_word && last_word.Left > word.Left) &&
                                            (word.Top > last_word.Top + 2.5 * last_word.Height)
                                        )
                                    )
                                {
                                    sb.Append("\n\n");
                                }

                                sb.Append(word.Text);
                                sb.Append(' ');

                                last_word = word;
                            }
                        }
                        else
                        {
                            sb.AppendLine(String.Format("OCR text is not yet ready for page {0}.  Please try again in a few minutes.", page));
                        }

                        // Add the text from the page
                        {
                            Paragraph paragraph = new Paragraph();
                            paragraph.Inlines.Add(sb.ToString());
                            flow_document.Blocks.Add(paragraph);
                        }

                        // Add the images from the page
                        PdfPageBase pdf_page = doc.Pages[page - 1];
                        Image[] images = pdf_page.ExtractImages();

                        if (null != images)
                        {
                            foreach (Image image in images)
                            {
                                BitmapSource bitmap_source = BitmapImageTools.FromImage(image);

                                System.Windows.Controls.Image image_to_render = new System.Windows.Controls.Image();
                                image_to_render.Source = bitmap_source;
                                BlockUIContainer image_container = new BlockUIContainer(image_to_render);
                                Figure floater = new Figure(image_container);
                                floater.HorizontalAnchor = FigureHorizontalAnchor.PageLeft;
                                floater.WrapDirection = WrapDirection.None;
                                floater.Width = new FigureLength(image.Width);

                                {
                                    Paragraph paragraph = new Paragraph();
                                    paragraph.Inlines.Add(floater);
                                    flow_document.Blocks.Add(paragraph);
                                }
                            }
                        }

                        // Add the annotations from the page
#if false
                        {
                            Logging.Debug("This PDF has {0} annotations", pdf_page.Annotations.Count);
                            Logging.Debug("The size of the page is {0}", pdf_page.Size);
                            foreach (var annotation in pdf_page.Annotations)
                            {
                                PdfLoadedUriAnnotation uri_annotation = annotation as PdfLoadedUriAnnotation;
                                if (null != uri_annotation)
                                {
                                    Logging.Debug("There is a URL to '{0}' with text '{1}' with bounds '{2}'",
                                    uri_annotation.Uri,
                                    uri_annotation.Bounds,
                                    uri_annotation.Text,
                                    null);

                                }
                            }
                        }
#endif
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was a problem exporting page {0}", page);
                    }
                }
            }

            StatusManager.Instance.UpdateStatus("ExportToWord", "Finished exporting");

            return flow_document;
        }

        public static void ExportToTextAndLaunch(PDFDocument pdf_document)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_ExportToText);

            // We have to hack this into an annotation report until we have decided if we want to use the collapsible stuff of annotation report
            StandardFlowDocument flow_document = DoExport(pdf_document);
            AsyncAnnotationReportBuilder.AnnotationReport annotation_report = new AsyncAnnotationReportBuilder.AnnotationReport(flow_document);

            ReportViewerControl report_view_control = new ReportViewerControl(annotation_report);
            string title = String.Format("Text export of PDF titled '{0}'", pdf_document.TitleCombined);
            MainWindowServiceDispatcher.Instance.OpenNewWindow(title, Icons.GetAppIcon(Icons.ExportToText), true, true, report_view_control);
        }
    }
}

