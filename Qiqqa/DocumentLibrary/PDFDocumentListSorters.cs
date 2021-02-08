using System;
using Qiqqa.Documents.PDF;
using Utilities;

namespace Qiqqa.DocumentLibrary
{
    internal class PDFDocumentListSorters
    {
        public static Comparison<PDFDocument> DateAddedToDatabase => delegate (PDFDocument p1, PDFDocument p2)
                                                                                   {
                                                                                       return -DateTime.Compare(p1.DateAddedToDatabase ?? DateTime.MinValue, p2.DateAddedToDatabase ?? DateTime.MinValue);
                                                                                   };

        public static Comparison<PDFDocument> DateLastRead => delegate (PDFDocument p1, PDFDocument p2)
                                                                            {
                                                                                return -DateTime.Compare(p1.DateLastRead ?? DateTime.MinValue, p2.DateLastRead ?? DateTime.MinValue);
                                                                            };

        public static Comparison<PDFDocument> DateLastCited => delegate (PDFDocument p1, PDFDocument p2)
                                                                             {
                                                                                 return -DateTime.Compare(p1.DateLastCited ?? DateTime.MinValue, p2.DateLastCited ?? DateTime.MinValue);
                                                                             };

        public static Comparison<PDFDocument> Title => delegate (PDFDocument p1, PDFDocument p2)
                                                                     {
                                                                         return String.Compare(p1.TitleCombined, p2.TitleCombined);
                                                                     };

        public static Comparison<PDFDocument> Authors => delegate (PDFDocument p1, PDFDocument p2)
                                                                       {
                                                                           return String.Compare(p1.AuthorsCombined, p2.AuthorsCombined);
                                                                       };

        public static Comparison<PDFDocument> Year => delegate (PDFDocument p1, PDFDocument p2)
                                                                    {
                                                                        return -String.Compare(p1.YearCombined, p2.YearCombined);
                                                                    };

        public static Comparison<PDFDocument> ReadingStage => delegate (PDFDocument p1, PDFDocument p2)
                                                                            {
                                                                                return String.Compare(p1.ReadingStage, p2.ReadingStage);
                                                                            };

        public static Comparison<PDFDocument> Rating => delegate (PDFDocument p1, PDFDocument p2)
                                                                      {
                                                                          return -String.Compare(p1.Rating, p2.Rating);
                                                                      };

        public static Comparison<PDFDocument> Favourite => delegate (PDFDocument p1, PDFDocument p2)
                                                                         {
                                                                             return Sorting.Compare(p1.IsFavourite, p2.IsFavourite);
                                                                         };

        public static Comparison<PDFDocument> HasPDF => delegate (PDFDocument p1, PDFDocument p2)
                                                                      {
                                                                          return Sorting.Compare(p1.DocumentExists, p2.DocumentExists);
                                                                      };

        public static Comparison<PDFDocument> HasBibTeX => delegate (PDFDocument p1, PDFDocument p2)
                                                                         {
                                                                             // TODO: heuristic: number of filled fields trumps length of BibTeX 
                                                                             // TODO: heuristic: length of BibTeX should NOT take comments into account, 
                                                                             // hence we should generate a comment-less BibTeX string for that (while
                                                                             // other parts of Qiqqa require a FULL dump output mode)
                                                                             return Sorting.CompareLengths(p1.BibTex, p2.BibTex);
                                                                         };

        public static Comparison<PDFDocument> PageCount => delegate (PDFDocument p1, PDFDocument p2)
                                                                         {
                                                                             return Sorting.Compare(p1.PageCount, p2.PageCount);
                                                                         };
    }
}
