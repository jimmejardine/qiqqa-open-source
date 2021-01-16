#if SYNCFUSION_ANTIQUE

using System;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.PDFControls.MetadataControls;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Utilities;
using Utilities.PDF;

namespace Qiqqa.AnnotationsReportBuilding.LegacyAnnotationConvertorStuff
{
    public static class LegacyAnnotationConvertor
    {
        public static void ForgetLegacyAnnotations(PDFDocument pdf_document)
        {
            foreach (PDFAnnotation pdf_annotation in pdf_document.GetAnnotations())
            {
                if (pdf_annotation.Legacy && !pdf_annotation.Deleted)
                {
                    pdf_annotation.Deleted = true;
                    pdf_annotation.Bindable.NotifyPropertyChanged();
                }
            }
        }


        public static int ImportLegacyAnnotations(PDFDocument pdf_document)
        {
            int imported_count = 0;

            if (!pdf_document.DocumentExists)
            {
                Logging.Info("Not importing legacy annotations for {0} because it has no PDF.", pdf_document.Fingerprint);
                return imported_count;
            }


            Logging.Info("+Importing legacy annotations from {0}", pdf_document.Fingerprint);
            using (AugmentedPdfLoadedDocument raw_pdf_document = new AugmentedPdfLoadedDocument(pdf_document.PDFRenderer.PDFFilename))
            {
                for (int page = 0; page < raw_pdf_document.Pages.Count; ++page)
                {
                    PdfLoadedPage raw_pdf_page = (PdfLoadedPage)raw_pdf_document.Pages[page];
                    if (null != raw_pdf_page.Annotations)
                    {
                        foreach (PdfAnnotation raw_pdf_annotation in raw_pdf_page.Annotations)
                        {
                            PDFAnnotation pdf_annotation = ConvertLegacyAnnotationToPDFAnnotation(pdf_document, page, raw_pdf_page, raw_pdf_annotation);
                            if (null != pdf_annotation)
                            {
                                // Check if we already have this annotation
                                PDFAnnotation matching_existing_pdf_annotation = null;
                                foreach (PDFAnnotation existing_pdf_annotation in pdf_document.GetAnnotations())
                                {
                                    if (true
                                        && existing_pdf_annotation.Page == pdf_annotation.Page
                                        && existing_pdf_annotation.Left == pdf_annotation.Left
                                        && existing_pdf_annotation.Top == pdf_annotation.Top
                                        && existing_pdf_annotation.Text == pdf_annotation.Text
                                        )
                                    {
                                        matching_existing_pdf_annotation = existing_pdf_annotation;
                                    }
                                }

                                if (null == matching_existing_pdf_annotation)
                                {
                                    // Add it to the PDFDocument
                                    pdf_document.GetAnnotations().AddUpdatedAnnotation(pdf_annotation);
                                    imported_count++;
                                }
                                else
                                {
                                    if (matching_existing_pdf_annotation.Deleted)
                                    {
                                        Logging.Info("Undeleting an identical legacy annotation.");
                                        matching_existing_pdf_annotation.Deleted = false;
                                        matching_existing_pdf_annotation.Bindable.NotifyPropertyChanged();
                                        imported_count++;
                                    }
                                    else
                                    {
                                        Logging.Info("Not importing an identical legacy annotation.");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Logging.Info("-Importing legacy annotations from {0}", pdf_document.Fingerprint);

            return imported_count;
        }

        private static PDFAnnotation ConvertLegacyAnnotationToPDFAnnotation(PDFDocument pdf_document, int page, PdfLoadedPage raw_pdf_page, PdfAnnotation raw_pdf_annotation)
        {
            // Try a popup
            {
                PdfLoadedPopupAnnotation raw_popup_annotation = raw_pdf_annotation as PdfLoadedPopupAnnotation;
                if (null != raw_popup_annotation)
                {
                    Logging.Debug特("  - page       = {0}", raw_pdf_page.Size);
                    Logging.Debug特("  - annotation = {0}", raw_popup_annotation.Bounds);
                    Logging.Debug特("  - text       = {0}", raw_popup_annotation.Text);

                    // Work out the relative coordinates
                    double BUFFER_HORIZ = 0.05;
                    double BUFFER_VERT = 0.1;
                    double left = BUFFER_HORIZ;
                    double width = 1 - 2 * BUFFER_HORIZ;
                    double top = 1.0 - (raw_popup_annotation.Bounds.Top / raw_pdf_page.Size.Height) - BUFFER_VERT / 2 + (raw_popup_annotation.Bounds.Height / raw_pdf_page.Size.Height) / 2;
                    double height = BUFFER_VERT;

                    //Bound it
                    top = Math.Max(0, top);

                    string text = raw_popup_annotation.Icon + ": " + raw_popup_annotation.Text;

                    // Create the annotation
                    PDFAnnotation pdf_annotation = new PDFAnnotation(pdf_document.PDFRenderer.DocumentFingerprint, page + 1, PDFAnnotationEditorControl.LastAnnotationColor, null);
                    pdf_annotation.Legacy = true;
                    pdf_annotation.Left = left;
                    pdf_annotation.Top = top;
                    pdf_annotation.Width = width;
                    pdf_annotation.Height = height;
                    pdf_annotation.Text = text;
                    return pdf_annotation;
                }
            }

            // Try a highlight
            {
                PdfLoadedTextMarkupAnnotation raw_markup_annotation = raw_pdf_annotation as PdfLoadedTextMarkupAnnotation;
                if (null != raw_markup_annotation)
                {
                    Logging.Debug特("  - page       = {0}", raw_pdf_page.Size);
                    Logging.Debug特("  - annotation = {0}", raw_markup_annotation.Bounds);

                    // Work out the relative coordinates
                    double left = raw_markup_annotation.Bounds.Left / raw_pdf_page.Size.Width;
                    double width = raw_markup_annotation.Bounds.Width / raw_pdf_page.Size.Width;
                    double top = 1 - (raw_markup_annotation.Bounds.Top / raw_pdf_page.Size.Height);
                    double height = raw_markup_annotation.Bounds.Height / raw_pdf_page.Size.Height;

                    string text = raw_markup_annotation.TextMarkupAnnotationType.ToString();

                    // Create the annotation
                    PDFAnnotation pdf_annotation = new PDFAnnotation(pdf_document.PDFRenderer.DocumentFingerprint, page + 1, PDFAnnotationEditorControl.LastAnnotationColor, null);
                    pdf_annotation.Legacy = true;
                    pdf_annotation.Left = left;
                    pdf_annotation.Top = top;
                    pdf_annotation.Width = width;
                    pdf_annotation.Height = height;
                    pdf_annotation.Text = text;
                    return pdf_annotation;
                }
            }

            // We don't understand this annotation
            {
                return null;
            }
        }

        public static void GetAnnotations(string pdf_filename)
        {
            Logging.Debug特("+Getting legacy annotations from {0}", pdf_filename);

            using (AugmentedPdfLoadedDocument pdf_doc = new AugmentedPdfLoadedDocument(pdf_filename))
            {
                foreach (PdfLoadedPage pdf_page in pdf_doc.Pages)
                {
                    foreach (PdfAnnotation pdf_annotation in pdf_page.Annotations)
                    {
                        {
                            PdfLoadedPopupAnnotation popup_annotation = pdf_annotation as PdfLoadedPopupAnnotation;
                            if (null != popup_annotation)
                            {
                                Logging.Debug特("Popup is {0}", popup_annotation.ToString());
                                Logging.Debug特("  - text     = {0}", popup_annotation.Text);
                                Logging.Debug特("  - icon     = {0}", popup_annotation.Icon);
                                Logging.Debug特("  - size     = {0}", popup_annotation.Size);
                                Logging.Debug特("  - location = {0}", popup_annotation.Location);
                                Logging.Debug特("  - open     = {0}", popup_annotation.Open);
                                Logging.Debug特("  - bounds   = {0}", popup_annotation.Bounds);
                                Logging.Debug特("  - color    = {0}", popup_annotation.Color);
                                continue;
                            }

                        }

                        {
                            PdfLoadedTextMarkupAnnotation markup_annotation = pdf_annotation as PdfLoadedTextMarkupAnnotation;
                            if (null != markup_annotation)
                            {
                                Logging.Debug特("Markup is {0}", markup_annotation.ToString());
                                Logging.Debug特("  - size     = {0}", markup_annotation.Size);
                                Logging.Debug特("  - location = {0}", markup_annotation.Location);
                                Logging.Debug特("  - type     = {0}", markup_annotation.TextMarkupAnnotationType);
                                Logging.Debug特("  - color    = {0}", markup_annotation.TextMarkupColor);
                                continue;
                            }
                        }

                        // We don't know what it is
                        {
                            Logging.Info("Unknown annotation {0}", pdf_annotation);
                            Logging.Debug特("  - text     = {0}", pdf_annotation.Text);
                            Logging.Debug特("  - rect     = {0}", pdf_annotation.Bounds);
                            Logging.Debug特("  - location = {0}", pdf_annotation.Location);
                            Logging.Debug特("  - flags    = {0}", pdf_annotation.AnnotationFlags);
                            continue;
                        }
                    }
                }
            }

            Logging.Debug特("-Getting legacy annotations from {0}", pdf_filename);
        }

#region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            Library library = Library.GuestInstance;
            while (!library.LibraryIsLoaded)
            {
                Thread.Sleep(100);
            }
            PDFDocument pdf_document = library.PDFDocuments[0];

            GetAnnotations(pdf_document.PDFRenderer.PDFFilename);
        }
#endif

#endregion
    }
}

#endif
