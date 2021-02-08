using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Qiqqa.UtilisationTracking;
using Utilities.Files;
using Utilities.OCR;

namespace Qiqqa.Documents.PDF.PDFControls.PDFExporting
{
    /// <summary>
    /// Exports to a text string
    /// </summary>
    internal class ExportToText
    {
        public static string DoExport(PDFDocument pdf_document)
        {
            StringBuilder sb = new StringBuilder();

            for (int page = 1; page <= pdf_document.PageCount; ++page)
            {
                sb.AppendLine();
                sb.AppendLine(String.Format("--- Page {0} ---", page));
                sb.AppendLine();

                WordList words = pdf_document.PDFRenderer.GetOCRText(page);
                if (null != words)
                {
                    foreach (Word word in words)
                    {
                        sb.Append(word.Text);
                        sb.Append(' ');
                    }
                }
                else
                {
                    sb.AppendLine(String.Format("OCR text is not yet ready for page {0}.  Please try again in a few minutes.", page));
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static void ExportToTextAndLaunch(PDFDocument pdf_document)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_ExportToText);

            string exported_text = DoExport(pdf_document);

            string filename = TempFile.GenerateTempFilename("txt");
            File.WriteAllText(filename, exported_text);
            Process.Start(filename);
        }
    }
}
