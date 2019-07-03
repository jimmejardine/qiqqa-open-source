using Qiqqa.Common;
using Qiqqa.WebBrowsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        PDFDocument pdf_document;
        public PDFDocument PDFDocument
        {
            set
            {
                this.pdf_document = value;
            }
        }

        void ButtonGoogleScholar_Click(object sender, RoutedEventArgs e)
        {
            DoSearch(WebSearchers.SCHOLAR_KEY);
        }

        void ButtonPubMed_Click(object sender, RoutedEventArgs e)
        {
            DoSearch(WebSearchers.PUBMED_KEY);
        }

        void ButtonArXiv_Click(object sender, RoutedEventArgs e)
        {
            DoSearch(WebSearchers.ARXIV_KEY);
        }


        void DoSearch(string ACTIVE_SEARCH_KEY)
        {
            if (null == pdf_document) return;
            
            var web_browser = MainWindowServiceDispatcher.Instance.OpenWebBrowser();
            web_browser.DoWebSearch(pdf_document.TitleCombined);
            web_browser.SelectSearchTab(ACTIVE_SEARCH_KEY);
        }
    }
}
