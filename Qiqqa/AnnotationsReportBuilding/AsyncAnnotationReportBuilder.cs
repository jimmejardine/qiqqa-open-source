using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using icons;
using Qiqqa.Common;
using Qiqqa.Common.Common;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.PDFControls.MetadataControls;
using Qiqqa.Documents.PDF.PDFControls.Page.Tools;
using Qiqqa.InCite;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.Images;
using Utilities.Misc;
using Utilities.OCR;
using Utilities.Shutdownable;

namespace Qiqqa.AnnotationsReportBuilding
{
    public static class AsyncAnnotationReportBuilder
    {
        public class AnnotationReport
        {
            public StandardFlowDocument flow_document;

            private class ClickOption
            {
                public Span span;
                public List<Inline> children;

                public ClickOption(Span span)
                {
                    this.span = span;
                    children = null;
                }
            }

            private List<ClickOption> click_options;

            public AnnotationReport()
            {
                flow_document = new StandardFlowDocument();
                flow_document.TextAlignment = TextAlignment.Center;
                flow_document.Background = Brushes.White;

                click_options = new List<ClickOption>();
            }

            public AnnotationReport(StandardFlowDocument flow_document)
            {
                this.flow_document = flow_document;
                click_options = new List<ClickOption>();
            }

            public void AddClickOption(Span span)
            {
                click_options.Add(new ClickOption(span));
            }

            public void CollapseClickOptions()
            {
                foreach (var click_option in click_options)
                {
                    if (0 < click_option.span.Inlines.Count)
                    {
                        click_option.children = new List<Inline>(click_option.span.Inlines);
                        click_option.span.Inlines.Clear();
                    }
                }
            }

            public void ExpandClickOptions()
            {
                foreach (var click_option in click_options)
                {
                    if (0 == click_option.span.Inlines.Count)
                    {
                        click_option.span.Inlines.AddRange(click_option.children);
                    }
                }
            }
        }

        internal delegate void BuildReportCallback(AsyncAnnotationReportBuilder.AnnotationReport annotation_report);

        internal static void BuildReport(WebLibraryDetail web_library_detail, List<PDFDocument> pdf_documents, AnnotationReportOptions annotation_report_options, BuildReportCallback cb)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            // Create a list of all the work we need to do
            List<AnnotationWorkGenerator.AnnotationWork> annotation_works = AnnotationWorkGenerator.GenerateAnnotationWorks(web_library_detail, pdf_documents, annotation_report_options);

            // WARNING: Due to the UI types used next, we need to run that bunch in the UI thread or WPF will barf a hairball later and you'll be sorry!

            WPFDoEvents.InvokeInUIThread(() =>
            {
                AnnotationReport annotation_report = new AnnotationReport();
                StandardFlowDocument flow_document = annotation_report.flow_document;

                // Now build the report
                PDFDocument last_pdf_document = null;
                for (int j = 0; j < annotation_works.Count; ++j)
                {
                    StatusManager.Instance.UpdateStatus("AnnotationReport", "Building annotation report", j, annotation_works.Count);
                    AnnotationWorkGenerator.AnnotationWork annotation_work = annotation_works[j];
                    PDFDocument pdf_document = annotation_work.pdf_document;
                    PDFAnnotation pdf_annotation = annotation_work.pdf_annotation;

                    // If this is a new PDFDocument, print out the header
                    if (last_pdf_document != pdf_document)
                    {
                        last_pdf_document = pdf_document;
                        Logging.Info("Processing {0}", pdf_document.Fingerprint);

                        if (!annotation_report_options.SuppressPDFDocumentHeader)
                        {
                            Span bold_title = new Span();
                            bold_title.FontSize = 30;
                            bold_title.FontFamily = ThemeTextStyles.FontFamily_Header;
                            bold_title.Inlines.Add(new LineBreak());

                            if (!String.IsNullOrEmpty(pdf_document.TitleCombined))
                            {
                                bold_title.Inlines.Add(pdf_document.TitleCombined);
                                bold_title.Inlines.Add(new LineBreak());
                            }
                            Span bold = new Span();
                            bold.FontSize = 16;
                            if (!String.IsNullOrEmpty(pdf_document.YearCombined))
                            {
                                bold.Inlines.Add(pdf_document.YearCombined);
                                bold.Inlines.Add(" · ");
                            }
                            if (!String.IsNullOrEmpty(pdf_document.AuthorsCombined))
                            {
                                bold.Inlines.Add(pdf_document.AuthorsCombined);
                            }
                            if (!String.IsNullOrEmpty(pdf_document.Publication))
                            {
                                Italic italic = new Italic();
                                italic.Inlines.Add(pdf_document.Publication);
                                bold.Inlines.Add(new LineBreak());
                                bold.Inlines.Add(italic);
                            }
                            if (!String.IsNullOrEmpty(pdf_document.Tags))
                            {
                                bold.Inlines.Add(new LineBreak());

                                Run run = new Run();
                                run.Text = "[" + pdf_document.Tags + "]";
                                bold.Inlines.Add(run);
                            }
                            bold.Inlines.Add(new LineBreak());

                            {
                                bold.Inlines.Add(new LineBreak());

                                Span click_options = new Span();
                                {
                                    {
                                        Run run = new Run(" Open ");
                                        run.Background = ThemeColours.Background_Brush_Blue_VeryVeryDark;
                                        run.Foreground = Brushes.White;
                                        run.Cursor = Cursors.Hand;
                                        run.Tag = pdf_document;
                                        run.MouseDown += run_Open_MouseDown;
                                        click_options.Inlines.Add(run);
                                    }
                                    click_options.Inlines.Add(new Run(" "));
                                    {
                                        Run run = new Run(" Cite (Author, Date) ");
                                        run.Background = ThemeColours.Background_Brush_Blue_VeryVeryDark;
                                        run.Foreground = Brushes.White;
                                        run.Cursor = Cursors.Hand;
                                        run.Tag = pdf_document;
                                        run.MouseDown += run_Cite_MouseDown_Together;
                                        click_options.Inlines.Add(run);
                                    }
                                    click_options.Inlines.Add(new Run(" "));
                                    {
                                        Run run = new Run(" [Cite Author (Date)] ");
                                        run.Background = ThemeColours.Background_Brush_Blue_VeryVeryDark;
                                        run.Foreground = Brushes.White;
                                        run.Cursor = Cursors.Hand;
                                        run.Tag = pdf_document;
                                        run.MouseDown += run_Cite_MouseDown_Separate;
                                        click_options.Inlines.Add(run);
                                    }
                                    click_options.Inlines.Add(new LineBreak());

                                    bold.Inlines.Add(click_options);
                                    annotation_report.AddClickOption(click_options);
                                }
                            }

                            Paragraph paragraph_header = new Paragraph();
                            paragraph_header.Inlines.Add(bold_title);
                            paragraph_header.Inlines.Add(bold);

                            Section section_header = new Section();
                            section_header.Background = ThemeColours.Background_Brush_Blue_VeryVeryDarkToWhite;
                            if (Colors.Transparent != pdf_document.Color)
                            {
                                section_header.Background = new SolidColorBrush(pdf_document.Color);
                            }
                            section_header.Blocks.Add(paragraph_header);

                            flow_document.Blocks.Add(new Paragraph(new LineBreak()));
                            flow_document.Blocks.Add(section_header);
                            //flow_document.Blocks.Add(new Paragraph(new LineBreak()));

                            bool have_document_details = false;

                            // Add the paper comment if we need to
                            if (annotation_report_options.IncludeComments)
                            {
                                string comment_text = pdf_document.Comments;
                                if (!String.IsNullOrEmpty(comment_text))
                                {
                                    have_document_details = true;

                                    flow_document.Blocks.Add(new Paragraph(new Run("●")));
                                    {
                                        Paragraph paragraph = new Paragraph();
                                        {
                                            Bold header = new Bold();
                                            header.Inlines.Add("Comments: ");
                                            paragraph.Inlines.Add(header);
                                        }
                                        {
                                            Italic italic = new Italic();
                                            italic.Inlines.Add(comment_text);
                                            paragraph.Inlines.Add(italic);
                                        }
                                        flow_document.Blocks.Add(paragraph);
                                    }
                                }
                            }
                            if (annotation_report_options.IncludeAbstract)
                            {
                                string abstract_text = pdf_document.Abstract;
                                if (PDFAbstractExtraction.CANT_LOCATE != abstract_text)
                                {
                                    have_document_details = true;

                                    flow_document.Blocks.Add(new Paragraph(new Run("●")));
                                    {
                                        Paragraph paragraph = new Paragraph();
                                        {
                                            Bold header = new Bold();
                                            header.Inlines.Add("Abstract: ");
                                            paragraph.Inlines.Add(header);
                                        }
                                        {
                                            Italic italic = new Italic();
                                            italic.Inlines.Add(abstract_text);
                                            paragraph.Inlines.Add(italic);
                                        }
                                        flow_document.Blocks.Add(paragraph);
                                    }
                                }
                            }

                            if (have_document_details)
                            {
                                flow_document.Blocks.Add(new Paragraph(new Run("●")));
                            }
                        }
                    }

                    // Print out the annotation
                    if (null != pdf_annotation)
                    {
                        // First the header
                        {
                            //flow_document.Blocks.Add(new Paragraph(new Run(" ● ")));

                            Paragraph paragraph = new Paragraph();
                            paragraph.Inlines.Add(new LineBreak());

                            {
                                Bold coloured_blob = new Bold();
                                coloured_blob.Foreground = new SolidColorBrush(pdf_annotation.Color);
                                coloured_blob.Inlines.Add(" ■ ");
                                paragraph.Inlines.Add(coloured_blob);
                            }

                            {
                                Span italic = new Span();
                                italic.FontSize = 16;
                                string annotation_header = String.Format("Page {0}", pdf_annotation.Page);
                                italic.Inlines.Add(annotation_header);
                                //Underline underline = new Underline(italic);
                                paragraph.Inlines.Add(italic);
                            }

                            {
                                Bold coloured_blob = new Bold();
                                coloured_blob.Foreground = new SolidColorBrush(pdf_annotation.Color);
                                coloured_blob.Inlines.Add(" ■ ");
                                paragraph.Inlines.Add(coloured_blob);
                            }

                            paragraph.Inlines.Add(new LineBreak());

                            // List the tags for this annotation
                            if (!annotation_report_options.SuppressPDFAnnotationTags)
                            {
                                if (!String.IsNullOrEmpty(pdf_annotation.Tags))
                                {
                                    paragraph.Inlines.Add(" [" + pdf_annotation.Tags.Replace(";", "; ") + "] ");

                                    paragraph.Inlines.Add(new LineBreak());
                                }
                            }

                            bool is_not_synthetic_annotation = (null == pdf_annotation.Tags || (!pdf_annotation.Tags.Contains(HighlightToAnnotationGenerator.HIGHLIGHTS_TAG) && !pdf_annotation.Tags.Contains(InkToAnnotationGenerator.INKS_TAG)));

                            {
                                paragraph.Inlines.Add(new LineBreak());

                                Span click_options = new Span();
                                {
                                    Run run = new Run(" Open ");
                                    run.Background = ThemeColours.Background_Brush_Blue_VeryDark;
                                    run.Foreground = Brushes.White;
                                    run.Cursor = Cursors.Hand;
                                    run.Tag = annotation_work;
                                    run.MouseDown += run_AnnotationOpen_MouseDown;
                                    click_options.Inlines.Add(run);
                                }

                                if (is_not_synthetic_annotation)
                                {
                                    click_options.Inlines.Add(new Run(" "));
                                    Run run = new Run(" Edit ");
                                    run.Background = ThemeColours.Background_Brush_Blue_VeryDark;
                                    run.Foreground = Brushes.White;
                                    run.Cursor = Cursors.Hand;
                                    run.Tag = pdf_annotation;
                                    run.MouseDown += run_AnnotationEdit_MouseDown;
                                    click_options.Inlines.Add(run);
                                }

                                {
                                    click_options.Inlines.Add(new Run(" "));
                                    Run run = new Run(" Cite (Author, Date) ");
                                    run.Background = ThemeColours.Background_Brush_Blue_VeryDark;
                                    run.Foreground = Brushes.White;
                                    run.Cursor = Cursors.Hand;
                                    run.Tag = pdf_document;
                                    run.MouseDown += run_Cite_MouseDown_Together;
                                    click_options.Inlines.Add(run);
                                }

                                {
                                    click_options.Inlines.Add(new Run(" "));
                                    Run run = new Run(" Cite Author (Date) ");
                                    run.Background = ThemeColours.Background_Brush_Blue_VeryDark;
                                    run.Foreground = Brushes.White;
                                    run.Cursor = Cursors.Hand;
                                    run.Tag = pdf_document;
                                    run.MouseDown += run_Cite_MouseDown_Separate;
                                    click_options.Inlines.Add(run);
                                }

                                click_options.Inlines.Add(new LineBreak());

                                paragraph.Inlines.Add(click_options);
                                annotation_report.AddClickOption(click_options);
                            }

                            {
                                Run run = new Run("Waiting for processing...");
                                run.Foreground = Brushes.Red;
                                paragraph.Inlines.Add(run);
                                annotation_work.processing_error = run;
                            }

                            paragraph.Background = ThemeColours.Background_Brush_Blue_DarkToWhite;
                            flow_document.Blocks.Add(paragraph);
                        }

                        if (!String.IsNullOrEmpty(pdf_annotation.Text))
                        {
                            Paragraph paragraph = new Paragraph();
                            paragraph.Inlines.Add(pdf_annotation.Text);
                            flow_document.Blocks.Add(paragraph);
                        }

                        {
                            // Prepare for some annotation image
                            if ((!annotation_report_options.ObeySuppressedImages || !pdf_annotation.AnnotationReportSuppressImage) && !annotation_report_options.SuppressAllImages)
                            {
                                Image image = new Image();
                                MouseWheelDisabler.DisableMouseWheelForControl(image);
                                image.Source = Icons.GetAppIcon(Icons.AnnotationReportImageWaiting);
                                BlockUIContainer image_container = new BlockUIContainer(image);
                                Figure floater = new Figure(image_container);
                                floater.HorizontalAnchor = FigureHorizontalAnchor.PageCenter;
                                floater.WrapDirection = WrapDirection.None;
                                floater.Width = new FigureLength(64);

                                floater.Cursor = Cursors.Hand;
                                floater.Tag = annotation_work;
                                floater.MouseDown += Floater_MouseDown;

                                Paragraph paragraph = new Paragraph();
                                paragraph.Inlines.Add(floater);

                                annotation_work.report_image = image;
                                annotation_work.report_floater = floater;

                                flow_document.Blocks.Add(paragraph);
                            }

                            // Prepare for some annotation text
                            if ((!annotation_report_options.ObeySuppressedText || !pdf_annotation.AnnotationReportSuppressText) && !annotation_report_options.SuppressAllText)
                            {
                                Paragraph paragraph = new Paragraph();
                                annotation_work.annotation_paragraph = paragraph;
                                flow_document.Blocks.Add(paragraph);
                            }
                        }
                    }

                    // Add another paragraph to separate nicely
                    {
                        Paragraph paragraph = new Paragraph();
                        flow_document.Blocks.Add(paragraph);
                    }
                }

                // Render the images in the background
                BackgroundRenderImages(flow_document, annotation_works, annotation_report_options);

                // Finito!
                StatusManager.Instance.ClearStatus("AnnotationReport");

                cb(annotation_report);
            });
        }

        private static void run_AnnotationOpen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Run run = sender as Run;
            AnnotationWorkGenerator.AnnotationWork annotation_work = run.Tag as AnnotationWorkGenerator.AnnotationWork;
            OpenAnnotationWork(annotation_work);
            e.Handled = true;
        }

        private static void run_AnnotationEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Run run = (Run)sender;
            PDFAnnotation pdf_annotation = (PDFAnnotation)run.Tag;
            PDFAnnotationEditorControl pdf_annotation_editor_control = new PDFAnnotationEditorControl();
            pdf_annotation_editor_control.SetAnnotation(pdf_annotation);

            AugmentedToolWindow pdf_annotation_editor_control_popup = new AugmentedToolWindow(pdf_annotation_editor_control, "Edit Annotation");
            pdf_annotation_editor_control_popup.IsOpen = true;

            e.Handled = true;
        }

        private static void run_Open_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.AnnotationReport_Open);

            Run run = (Run)sender;
            PDFDocument pdf_document = (PDFDocument)run.Tag;
            MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document);
            e.Handled = true;

        }

        private static void run_Cite_MouseDown_Together(object sender, MouseButtonEventArgs e)
        {
            run_Cite_MouseDown(sender, e, false);
        }

        private static void run_Cite_MouseDown_Separate(object sender, MouseButtonEventArgs e)
        {
            run_Cite_MouseDown(sender, e, true);
        }

        private static void run_Cite_MouseDown(object sender, MouseButtonEventArgs e, bool separate_author_and_date)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.AnnotationReport_Cite);
            FeatureTrackingManager.Instance.UseFeature(Features.InCite_AddNewCitation_FromAnnotationReport);

            Run run = (Run)sender;
            PDFDocument pdf_document = (PDFDocument)run.Tag;
            PDFDocumentCitingTools.CitePDFDocument(pdf_document, separate_author_and_date);
            e.Handled = true;
        }

        private static void BackgroundRenderImages(FlowDocument flow_document, List<AnnotationWorkGenerator.AnnotationWork> annotation_works, AnnotationReportOptions annotation_report_options)
        {
            // Render the images in the background
            SafeThreadPool.QueueUserWorkItem(o => BackgroundRenderImages_BACKGROUND(flow_document, annotation_works, annotation_report_options));
        }

        private static void BackgroundRenderImages_BACKGROUND(FlowDocument flow_document, List<AnnotationWorkGenerator.AnnotationWork> annotation_works, AnnotationReportOptions annotation_report_options)
        {
            ShutdownableManager.Sleep(annotation_report_options.InitialRenderDelayMilliseconds);

            PDFDocument last_pdf_document = null;
            StatusManager.Instance.ClearCancelled("AnnotationReportBackground");
            for (int j = 0; j < annotation_works.Count; ++j)
            {
                if (ShutdownableManager.Instance.IsShuttingDown)
                {
                    Logging.Error("Canceling creation of Annotation Report due to signaled application shutdown");
                    StatusManager.Instance.SetCancelled("AnnotationReportBackground");
                }

                StatusManager.Instance.UpdateStatus("AnnotationReportBackground", "Building annotation report image", j, annotation_works.Count, true);
                if (StatusManager.Instance.IsCancelled("AnnotationReportBackground"))
                {
                    Logging.Warn("User canceled annotation report generation");
                    break;
                }

                AnnotationWorkGenerator.AnnotationWork annotation_work = annotation_works[j];
                PDFDocument pdf_document = annotation_work.pdf_document;

                // Clear down our previously caches pages
                if (null != last_pdf_document && last_pdf_document != pdf_document)
                {
                    if (last_pdf_document.DocumentExists)
                    {
                        last_pdf_document.PDFRenderer.FlushCachedPageRenderings();
                    }
                }

                // Remember this PDF document so we can flush it if necessary
                last_pdf_document = pdf_document;

                // Now render each image
                PDFAnnotation pdf_annotation = annotation_work.pdf_annotation;
                if (null != pdf_annotation)
                {
                    try
                    {
                        // Clear the waiting for processing text
                        annotation_work.processing_error.Dispatcher.Invoke(new Action(() =>
                        {
                            annotation_work.processing_error.Text = "";
                        }
                        ), DispatcherPriority.Background);


                        if (pdf_document.DocumentExists)
                        {
                            // Fill in the paragraph text
                            if (null != annotation_work.annotation_paragraph)
                            {
                                annotation_work.processing_error.Dispatcher.Invoke(
                                    new Action(() => BuildAnnotationWork_FillAnnotationText(pdf_document, pdf_annotation, annotation_work)),
                                    DispatcherPriority.Background);
                            }

                            if (null != annotation_work.report_floater)
                            {
                                annotation_work.processing_error.Dispatcher.Invoke(new Action(() =>
                                    {
                                        try
                                        {
                                            System.Drawing.Image annotation_image = PDFAnnotationToImageRenderer.RenderAnnotation(pdf_document, pdf_annotation, 80);
                                            BitmapSource cropped_image_page = BitmapImageTools.FromImage(annotation_image);
                                            annotation_work.report_image.Source = cropped_image_page;
                                            annotation_work.report_floater.Width = new FigureLength(cropped_image_page.PixelWidth / 1);
                                        }
                                        catch (Exception ex)
                                        {
                                            Logging.Warn(ex, "There was a problem while rendering an annotation.");
                                            annotation_work.report_image.Source = Icons.GetAppIcon(Icons.AnnotationReportImageError);
                                            annotation_work.processing_error.Text = "There was a problem while rendering this annotation.";
                                        }
                                    }
                                ), DispatcherPriority.Background);
                            }
                        }
                        else
                        {
                            annotation_work.processing_error.Dispatcher.Invoke(new Action(() =>
                            {
                                if (null != annotation_work.report_image)
                                {
                                    annotation_work.report_image.Source = Icons.GetAppIcon(Icons.AnnotationReportImageError);
                                }

                                annotation_work.processing_error.Text = "Can't show image: The PDF does not exist locally.";
                            }
                            ), DispatcherPriority.Background);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "There was an error while rendering page {0} for document {1} for the annotation report", pdf_annotation.Page, pdf_annotation.DocumentFingerprint);

                        annotation_work.processing_error.Dispatcher.Invoke(new Action(() =>
                        {
                            if (null != annotation_work.report_image)
                            {
                                annotation_work.report_image.Source = Icons.GetAppIcon(Icons.AnnotationReportImageError);
                            }

                            annotation_work.processing_error.Text = "Can't show image: There was an error rendering the metadata image.";
                        }
                        ), DispatcherPriority.Background);
                    }
                }
            }

            // And flush the rendering cache of the last document
            if (null != last_pdf_document)
            {
                if (last_pdf_document.DocumentExists)
                {
                    last_pdf_document.PDFRenderer.FlushCachedPageRenderings();
                }
            }

            StatusManager.Instance.ClearStatus("AnnotationReportBackground");
        }

        private static void BuildAnnotationWork_FillAnnotationText(PDFDocument pdf_document, PDFAnnotation pdf_annotation, AnnotationWorkGenerator.AnnotationWork annotation_work)
        {
            int current_color = -1;
            Run current_run = new Run();
            annotation_work.annotation_paragraph.Inlines.Add(current_run);

            try
            {
                // Get the text for this annotation
                WordList word_list = pdf_document.PDFRenderer.GetOCRText(pdf_annotation.Page);

                if (null != word_list)
                {
                    StringBuilder current_sb = new StringBuilder();
                    foreach (var word in word_list)
                    {
                        if (word.Contains(pdf_annotation.Left, pdf_annotation.Top, pdf_annotation.Width, pdf_annotation.Height))
                        {
                            // Find out what colour this word is...
                            int new_color = -1;
                            foreach (PDFHighlight highlight in pdf_document.Highlights.GetHighlightsForPage(pdf_annotation.Page))
                            {
                                if (highlight.Page == pdf_annotation.Page && word.ContainsMajority(highlight.Left, highlight.Top, highlight.Width, highlight.Height))
                                {
                                    new_color = highlight.Color;
                                    break;
                                }
                            }

                            // If the colour has change
                            if (new_color != current_color)
                            {
                                // Emit the existing span
                                current_run.Text = current_sb.ToString();

                                // Create the new span
                                current_color = new_color;
                                current_run = new Run();
                                current_sb = new StringBuilder();
                                annotation_work.annotation_paragraph.Inlines.Add(current_run);
                                if (-1 != new_color)
                                {
                                    current_run.Background = new SolidColorBrush(StandardHighlightColours.GetColor(new_color));
                                }
                            }

                            // Tidy up dashes on line-ends
                            string current_text = word.Text;
                            string current_spacer = " ";
                            if (current_text.EndsWith("-"))
                            {
                                current_text = current_text.TrimEnd('-');
                                current_spacer = "";
                            }

                            // Append the new text
                            current_sb.Append(current_text);
                            current_sb.Append(current_spacer);
                        }
                    }

                    // Emit the final span
                    current_run.Text = current_sb.ToString();
                }
                else
                {
                    Run run = new Run();
                    run.Background = Brushes.Orange;
                    run.Text = String.Format("OCR is not complete for page {0}", pdf_annotation.Page);
                    annotation_work.annotation_paragraph.Inlines.Add(run);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem while trying to add annotation text for document {0}", pdf_document.Fingerprint);
                Run run = new Run();
                run.Background = Brushes.Red;
                run.Text = String.Format("Processing error: {0}", ex.Message);
                annotation_work.annotation_paragraph.Inlines.Add(run);
            }
        }

        private static void Floater_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Figure floater = sender as Figure;
            AnnotationWorkGenerator.AnnotationWork annotation_work = floater.Tag as AnnotationWorkGenerator.AnnotationWork;
            OpenAnnotationWork(annotation_work);
            e.Handled = true;
        }

        private static void OpenAnnotationWork(AnnotationWorkGenerator.AnnotationWork annotation_work)
        {
            string fingerprint = annotation_work.pdf_annotation.DocumentFingerprint;
            PDFDocument pdf_document = annotation_work.web_library_detail.Xlibrary.GetDocumentByFingerprint(fingerprint);
            if (null == pdf_document)
            {
                Logging.Error("AsyncAnnotationReportBuilder: Cannot find document anymore for fingerprint {0}", fingerprint);
            }
            else
            {
                MainWindowServiceDispatcher.Instance.OpenDocument(pdf_document, page: annotation_work.pdf_annotation.Page);
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            Library library = Library.GuestInstance;
            List<PDFDocument> pdf_documents = library.PDFDocuments;

            {
                var annotation_report = BuildReport(library, pdf_documents, null);
                FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
                viewer.Document = annotation_report.flow_document;
                ControlHostingWindow window = new ControlHostingWindow("Annotations", viewer);
                window.Show();

                Thread.Sleep(1000);
            }
        }

        public static void TestTagFilter()
        {
            Library library = Library.GuestInstance;
            List<PDFDocument> pdf_documents = library.PDFDocuments;

            AnnotationReportOptionsWindow arow = new AnnotationReportOptionsWindow();
            arow.ShowTagOptions(library, pdf_documents, OnShowTagOptionsComplete);
        }

        private static void OnShowTagOptionsComplete(WebLibraryDetail web_library_detail, List<PDFDocument> pdf_documents, AnnotationReportOptions annotation_report_options)
        {
            var annotation_report = BuildReport(library, pdf_documents, annotation_report_options);
            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            viewer.Document = annotation_report.flow_document;
            ControlHostingWindow window = new ControlHostingWindow("Annotations", viewer);
            window.Show();
        }
#endif

        #endregion

    }
}
