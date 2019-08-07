using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.Documents.PDF;
using Utilities;

namespace Qiqqa.DocumentLibrary.LibraryFilter
{
    /// <summary>
    /// Interaction logic for LibraryFilterControl_Sort.xaml
    /// </summary>
    public partial class LibraryFilterControl_Sort : UserControl
    {
        object sort_hyperlink;
        bool reverse_sort_direction = false;

        public delegate void SortChangedDelegate();
        public event SortChangedDelegate SortChanged;

        public LibraryFilterControl_Sort()
        {
            InitializeComponent();
        }

        void OnHyperLinkClick(object sender, RoutedEventArgs e)
        {
            object new_sort_hyperlink = sender;

            if (sort_hyperlink == new_sort_hyperlink)
            {
                reverse_sort_direction = !reverse_sort_direction;
            }
            else
            {
                reverse_sort_direction = false;
                sort_hyperlink = new_sort_hyperlink;
            }

            SortChanged?.Invoke();
        }

        internal void SetSortToSearchScore()
        {
            reverse_sort_direction = false;
            sort_hyperlink = HyperlinkScore;
        }

        internal void ApplySort(List<PDFDocument> pdf_documents, Dictionary<string, double> search_quick_scores)
        {
            // Now apply the sort
            if (false) { }
            else if (sort_hyperlink == HyperlinkScore)
            {
                if (null != search_quick_scores)
                {
                    pdf_documents.Sort(delegate(PDFDocument p1, PDFDocument p2)
                    {
                        double s1, s2;
                        search_quick_scores.TryGetValue(p1.Fingerprint, out s1);
                        search_quick_scores.TryGetValue(p2.Fingerprint, out s2);
                        return -Sorting.Compare(s1, s2);
                    });
                }
            }
            else if (sort_hyperlink == HyperlinkTitle) pdf_documents.Sort(delegate(PDFDocument p1, PDFDocument p2) { return String.Compare(p1.TitleCombined, p2.TitleCombined); });
            else if (sort_hyperlink == HyperlinkAuthors) pdf_documents.Sort(delegate(PDFDocument p1, PDFDocument p2) { return String.Compare(p1.AuthorsCombined, p2.AuthorsCombined); });
            else if (sort_hyperlink == HyperlinkYear) pdf_documents.Sort(delegate(PDFDocument p1, PDFDocument p2) { return -String.Compare(p1.YearCombined, p2.YearCombined); });
            else if (sort_hyperlink == HyperlinkRecentlyAdded) pdf_documents.Sort(PDFDocumentListSorters.DateAddedToDatabase);
            else if (sort_hyperlink == HyperlinkRecentlyRead) pdf_documents.Sort(PDFDocumentListSorters.DateLastRead);
            else if (sort_hyperlink == HyperlinkRecentlyCited) pdf_documents.Sort(PDFDocumentListSorters.DateLastCited);                
            else if (sort_hyperlink == HyperlinkReadingStage) pdf_documents.Sort(delegate(PDFDocument p1, PDFDocument p2) { return String.Compare(p1.ReadingStage, p2.ReadingStage); });
            else if (sort_hyperlink == HyperlinkRating) pdf_documents.Sort(delegate(PDFDocument p1, PDFDocument p2) { return -String.Compare(p1.Rating, p2.Rating); });
            else if (sort_hyperlink == HyperlinkFavourite) pdf_documents.Sort(delegate(PDFDocument p1, PDFDocument p2) { return Sorting.Compare(p1.IsFavourite, p2.IsFavourite); });
            else if (sort_hyperlink == HyperlinkHasPDF) pdf_documents.Sort(delegate(PDFDocument p1, PDFDocument p2) { return Sorting.Compare(p1.DocumentExists, p2.DocumentExists); });
            else if (sort_hyperlink == HyperlinkHasBibTeX) pdf_documents.Sort(delegate(PDFDocument p1, PDFDocument p2) { return Sorting.CompareLengths(p1.BibTex, p2.BibTex); });
            else if (sort_hyperlink == HyperlinkPageCount) pdf_documents.Sort(delegate(PDFDocument p1, PDFDocument p2) { return Sorting.Compare(p1.SafePageCount, p2.SafePageCount); });

            else
            {
                // If there is nothing else to sort by, do recently added
                pdf_documents.Sort(PDFDocumentListSorters.DateAddedToDatabase);
            }

            // Reverse sort?
            if (reverse_sort_direction)
            {
                pdf_documents.Reverse();
            }
        }
    }
}
