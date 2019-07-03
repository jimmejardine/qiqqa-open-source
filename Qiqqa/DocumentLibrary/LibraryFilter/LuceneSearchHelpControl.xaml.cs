using System.Windows;
using icons;
using Qiqqa.Common;
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
            MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(@"http://lucene.apache.org/core/old_versioned_docs/versions/2_9_1/queryparsersyntax.html");
        }
    }
}
