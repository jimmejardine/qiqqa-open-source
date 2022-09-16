using System;
using System.Collections.Generic;
using System.Text;
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

                    throw new Exception("PDF Export not available in this build!");
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
