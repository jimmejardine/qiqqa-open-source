using System.Collections.Generic;
using Qiqqa.Documents.PDF;

namespace Qiqqa.DocumentLibrary
{
    public class LibraryStats
    {
        public static List<PDFDocument> GetNextReads(Library library)
        {
            List<PDFDocument> pdf_documents = new List<PDFDocument>();
            foreach (PDFDocument pdf_document in library.PDFDocuments)
            {
            }

            return null;
        }
    }
}
