using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common;
using Qiqqa.WebBrowsing;

namespace Qiqqa.Documents.PDF.InfoBarStuff.OnlineDatabaseLookupStuff
{
    /// <summary>
    /// Interaction logic for OnlineDatabaseLookupControl.xaml
    /// </summary>
    public partial class OnlineDatabaseLookupControl : UserControl
    {
        public OnlineDatabaseLookupControl()
        {
            InitializeComponent();

            ButtonGoogleScholar.Caption = "GoogleScholar";
            ButtonGoogleScholar.CenteredMode = true;
            ButtonGoogleScholar.Click += ButtonGoogleScholar_Click;

            ButtonPubMed.Caption = "PubMed";
            ButtonPubMed.CenteredMode = true;
            ButtonPubMed.Click += ButtonPubMed_Click;

            ButtonArXiv.Caption = "ArXiv";
            ButtonArXiv.CenteredMode = true;
            ButtonArXiv.Click += ButtonArXiv_Click;
        }

        private PDFDocument pdf_document;
        public PDFDocument PDFDocument
        {
            get => pdf_document;
            set => pdf_document = value;
        }

        private void ButtonGoogleScholar_Click(object sender, RoutedEventArgs e)
        {
            DoSearch(WebSearchers.SCHOLAR_KEY);
        }

        private void ButtonPubMed_Click(object sender, RoutedEventArgs e)
        {
            DoSearch(WebSearchers.PUBMED_KEY);
        }

        private void ButtonArXiv_Click(object sender, RoutedEventArgs e)
        {
            DoSearch(WebSearchers.ARXIV_KEY);
        }

        private void DoSearch(string active_search_key)
        {
            if (null == pdf_document) return;

            var web_browser = MainWindowServiceDispatcher.Instance.OpenWebBrowser();
            web_browser.DoWebSearch(pdf_document.TitleCombined);
            web_browser.SelectSearchTab(active_search_key);
        }
    }
}
