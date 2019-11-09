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
        private object sort_hyperlink;
        private bool reverse_sort_direction = false;

        public delegate void SortChangedDelegate();
        public event SortChangedDelegate SortChanged;

        public LibraryFilterControl_Sort()
        {
            InitializeComponent();
        }

        private void OnHyperLinkClick(object sender, RoutedEventArgs e)
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
            if (sort_hyperlink == HyperlinkScore)
            {
                if (null != search_quick_scores)
                {
                    pdf_documents.Sort(delegate (PDFDocument p1, PDFDocument p2)
                    {
                        double s1, s2;
                        search_quick_scores.TryGetValue(p1.Fingerprint, out s1);
                        search_quick_scores.TryGetValue(p2.Fingerprint, out s2);
                        return -Sorting.Compare(s1, s2);
                    });
                }
            }
            else if (sort_hyperlink == HyperlinkTitle) pdf_documents.Sort(PDFDocumentListSorters.Title);
            else if (sort_hyperlink == HyperlinkAuthors) pdf_documents.Sort(PDFDocumentListSorters.Authors);
            else if (sort_hyperlink == HyperlinkYear) pdf_documents.Sort(PDFDocumentListSorters.Year);
            else if (sort_hyperlink == HyperlinkRecentlyAdded) pdf_documents.Sort(PDFDocumentListSorters.DateAddedToDatabase);
            else if (sort_hyperlink == HyperlinkRecentlyRead) pdf_documents.Sort(PDFDocumentListSorters.DateLastRead);
            else if (sort_hyperlink == HyperlinkRecentlyCited) pdf_documents.Sort(PDFDocumentListSorters.DateLastCited);
            else if (sort_hyperlink == HyperlinkReadingStage) pdf_documents.Sort(PDFDocumentListSorters.ReadingStage);
            else if (sort_hyperlink == HyperlinkRating) pdf_documents.Sort(PDFDocumentListSorters.Rating);
            else if (sort_hyperlink == HyperlinkFavourite) pdf_documents.Sort(PDFDocumentListSorters.Favourite);
            else if (sort_hyperlink == HyperlinkHasPDF) pdf_documents.Sort(PDFDocumentListSorters.HasPDF);
            else if (sort_hyperlink == HyperlinkHasBibTeX) pdf_documents.Sort(PDFDocumentListSorters.HasBibTeX);
            else if (sort_hyperlink == HyperlinkPageCount) pdf_documents.Sort(PDFDocumentListSorters.PageCount);
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
