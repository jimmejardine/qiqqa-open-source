using System.Threading;
using Qiqqa.DocumentLibrary;
using Qiqqa.Documents.PDF.PDFControls;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF
{
#if TEST
    public class TestHarness
    {
        public static void TestPDFRendererControl()
        {
            Library library = Library.GuestInstance;
            PDFDocument pdf_document = library.PDFDocuments[1];
            PDFRendererControl pdf_renderer_control = new PDFRendererControl(pdf_document, true);
            ControlHostingWindow window = new ControlHostingWindow("PDF Renderer Control", pdf_renderer_control);
            window.Show();
        }
        
        public static void TestPDFReadingControl()
        {
            Library library = Library.GuestInstance;
            Thread.Sleep(1000);

            while (!library.LibraryIsLoaded)
            {
                Logging.Info("Waiting for library");
                Thread.Sleep(100);
            }

            PDFDocument pdf_document = library.PDFDocuments[0];
            PDFReadingControl pdf_reading_control = new PDFReadingControl(pdf_document);            
            ControlHostingWindow window = new ControlHostingWindow("PDF Reading Control", pdf_reading_control);
            window.Show();
        }
    }
#endif
}
