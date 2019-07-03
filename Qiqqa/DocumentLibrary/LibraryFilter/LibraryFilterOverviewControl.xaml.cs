using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Qiqqa.Documents.PDF;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.LibraryFilter
{
    /// <summary>
    /// Interaction logic for LibraryFilterOverviewControl.xaml
    /// </summary>
    public partial class LibraryFilterOverviewControl : UserControl
    {
        LibraryFilterControl library_filter_control = null;

        public LibraryFilterOverviewControl()
        {
            InitializeComponent();

            ObjLibraryFilterDescriptiveText.Background = ThemeColours.Background_Brush_Blue_VeryDarkToDark;
        }

        internal void OnFilterChanged(LibraryFilterControl library_filter_control, List<PDFDocument> pdf_documents, Span descriptive_span, string filter_terms, Dictionary<string, double> search_scores, PDFDocument pdf_document_to_focus_on)
        {
            this.library_filter_control = library_filter_control;

            if (null != descriptive_span)
            {
                int match_count = 0;
                foreach (var pdf_document in pdf_documents)
                {
                    if (!pdf_document.Deleted) ++match_count;
                }

                ObjLibraryFilterDescriptiveTextBorder.Visibility = Visibility.Visible;

                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(LibraryFilterHelpers.GetClearImageInline("Clear all filters.", hyperlink_clear_all_OnClick));
                paragraph.Inlines.Add(" ");
                paragraph.Inlines.Add("Library currently filtered by: ");
                paragraph.Inlines.Add(descriptive_span);
                paragraph.Inlines.Add("(" + match_count + " matching documents)");
                

                // Add to our viewer
                ObjLibraryFilterDescriptiveText.Blocks.Clear();
                ObjLibraryFilterDescriptiveText.Blocks.Add(paragraph);
            }
            else
            {
                ObjLibraryFilterDescriptiveTextBorder.Visibility = Visibility.Collapsed;
                ObjLibraryFilterDescriptiveText.Blocks.Clear();
            }
        }

        void hyperlink_clear_all_OnClick(object sender, MouseButtonEventArgs e)
        {
            library_filter_control.ResetFilters();
        }
    }
}
