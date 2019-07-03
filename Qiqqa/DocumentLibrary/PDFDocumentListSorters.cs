using System;
using Qiqqa.Documents.PDF;

namespace Qiqqa.DocumentLibrary
{
    class PDFDocumentListSorters
    {
        public static Comparison<PDFDocument> DateAddedToDatabase
        {
            get
            {
                return delegate(PDFDocument p1, PDFDocument p2)
                {
                    return -DateTime.Compare(p1.DateAddedToDatabase ?? DateTime.MinValue, p2.DateAddedToDatabase ?? DateTime.MinValue);
                };
            }
        }

        public static Comparison<PDFDocument> DateLastRead
        {
            get
            {
                return delegate(PDFDocument p1, PDFDocument p2)
                {
                    return -DateTime.Compare(p1.DateLastRead ?? DateTime.MinValue, p2.DateLastRead ?? DateTime.MinValue);
                };
            }
        }


        public static Comparison<PDFDocument> DateLastCited
        {
            get
            {
                return delegate(PDFDocument p1, PDFDocument p2)
                {
                    return -DateTime.Compare(p1.DateLastCited ?? DateTime.MinValue, p2.DateLastCited ?? DateTime.MinValue);
                };
            }
        }


        
    }
}
