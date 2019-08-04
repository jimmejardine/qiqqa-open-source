using System.Windows;
using icons;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;

namespace Qiqqa.DocumentLibrary.LibraryFilter
{
    /// <summary>
    /// Interaction logic for LuceneSearchHelpControl.xaml
    /// </summary>
    public partial class LuceneSearchHelpControl : StandardWindow
    {
        public LuceneSearchHelpControl()
        {
            InitializeComponent();

            Header.Img = Icons.GetAppIcon(Icons.Search);

            HyperlinkLuceneExamples.Click += HyperlinkLuceneExamples_Click;
        }

        void HyperlinkLuceneExamples_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_LuceneQuerySyntax);
        }
    }
}
