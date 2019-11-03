using System;
using Syncfusion.Pdf;
using Utilities;
using Utilities.PDF;

namespace Qiqqa.Documents.PDF.MetadataSuggestions
{
    public static class PDFMetadataInferenceFromPDFMetadata
    {
        internal static bool NeedsProcessing(PDFDocument pdf_document)
        {
            if (!pdf_document.DocumentExists) return false;
            if (pdf_document.AutoSuggested_PDFMetadata) return false;

            Logging.Info("{0} requires PDFMetadataInferenceFromPDFMetadata", pdf_document.Fingerprint);
            return true;
        }

        internal static bool InferFromPDFMetadata(PDFDocument pdf_document)
        {
            if (!pdf_document.DocumentExists) return false;
            if (pdf_document.AutoSuggested_PDFMetadata) return false;

            pdf_document.AutoSuggested_PDFMetadata = true;
            pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.AutoSuggested_PDFMetadata);

            using (AugmentedPdfLoadedDocument doc = new AugmentedPdfLoadedDocument(pdf_document.DocumentPath))
            {
                if (String.IsNullOrEmpty(pdf_document.TitleSuggested))
                {
                    string title = doc.DocumentInformation.Title;
                    if (PDFMetadataInferenceFromOCR.IsReasonableTitleOrAuthor(title))
                    {
                        Logging.Info("Auto-found in PDF metadata title '{0}'", title);
                        pdf_document.TitleSuggested = title;
                        pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.TitleSuggested);
                    }
                }

                if (String.IsNullOrEmpty(pdf_document.AuthorsSuggested))
                {
                    string authors = doc.DocumentInformation.Author;
                    if (PDFMetadataInferenceFromOCR.IsReasonableTitleOrAuthor(authors))
                    {
                        Logging.Info("Auto-found in PDF metadata authors '{0}'", authors);
                        pdf_document.AuthorsSuggested = authors;
                        pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.AuthorsSuggested);
                    }
                }

                if (String.IsNullOrEmpty(pdf_document.YearSuggested))
                {
                    int year = GetSafeDocumentYear(doc.DocumentInformation).Year;
                    if (PDFMetadataInferenceFromOCR.IsReasonableYear(year))
                    {
                        Logging.Info("Auto-found in PDF metadata year '{0}'", year);
                        pdf_document.YearSuggested = Convert.ToString(year);
                        pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.YearSuggested);
                    }
                }
            }

            return true;
        }

        private static DateTime GetSafeDocumentYear(PdfDocumentInformation doc_info)
        {
            try
            {
                return doc_info.CreationDate;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }
}
