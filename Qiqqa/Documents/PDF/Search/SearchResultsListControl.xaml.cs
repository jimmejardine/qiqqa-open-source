using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.Search
{
    /// <summary>
    /// Interaction logic for SearchResultsListControl.xaml
    /// </summary>
    public partial class SearchResultsListControl : UserControl
    {
        public delegate void SearchDelegate(PDFSearchResult search_result);
        public event SearchDelegate SearchSelectionChanged;
        public event SearchDelegate SearchClicked;
        
        public SearchResultsListControl()
        {
            Theme.Initialize();

            InitializeComponent();

            ListSearchResults.SelectionChanged += ListSearchResults_SelectionChanged;
            ListSearchResults.MouseUp += ListSearchResults_MouseUp;

            this.DataContextChanged += SearchResultsListControl_DataContextChanged;
            ReflectDataContext();
        }

        void ListSearchResults_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PDFSearchResult search_result = ListSearchResults.SelectedItem as PDFSearchResult;
            SearchClicked?.Invoke(search_result);
        }

        void ListSearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PDFSearchResult search_result = ListSearchResults.SelectedItem as PDFSearchResult;
            SearchSelectionChanged?.Invoke(search_result);
        }

        void SearchResultsListControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ReflectDataContext();
        }

        private void ReflectDataContext()
        {
            List<PDFSearchResult> list = this.DataContext as List<PDFSearchResult>;

            if (null == list || 0 == list.Count)
            {
                ListSearchResults.Visibility = Visibility.Collapsed;
            }
            else
            {
                ListSearchResults.Visibility = Visibility.Visible;
            }
        }
    }
}
