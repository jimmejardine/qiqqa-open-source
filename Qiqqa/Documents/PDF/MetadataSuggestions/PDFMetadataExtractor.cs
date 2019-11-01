using System;
using Utilities.PDF;

namespace Qiqqa.Documents.PDF.MetadataSuggestions
{
    public static class PDFMetadataExtractor
    {
        private static readonly char[] SPLITS = new char[] { ',', ';', ' ', '\'', '"' };

        public static void ExtractKeywordsAsTags(PDFDocument pdf_document)
        {
            if (pdf_document.DocumentExists)
            {
                using (AugmentedPdfLoadedDocument doc = new AugmentedPdfLoadedDocument(pdf_document.DocumentPath))
                {
                    string keywords_raw = doc.DocumentInformation.Keywords;
                    string[] keywords = keywords_raw.Split(SPLITS, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string keyword in keywords)
                    {
                        pdf_document.AddTag(keyword);
                    }
                }
            }
        }
    }
}