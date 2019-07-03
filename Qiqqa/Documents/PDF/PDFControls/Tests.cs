using System.Collections.Generic;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF.PDFRendering;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls
{
    public class Tests
    {
        public static void TestPDFRendererControl()
        {
            Library library = Library.GuestInstance;
            List <PDFDocument> pdf_documents = library.PDFDocuments;

            if (0 < pdf_documents.Count)
            {
                PDFDocument pdf_document = pdf_documents[0];
                PDFRenderer pdf_renderer = new PDFRenderer(pdf_document.DocumentPath, null, null);
                PDFAnnotationList pdf_annotation_list = pdf_document.Annotations;

                PDFRendererControl control = new PDFRendererControl(pdf_document, true);
                ControlHostingWindow window = new ControlHostingWindow("PDFRenderer", control);
                window.Show();
            }
        }
    }
}
