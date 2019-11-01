using System;
using System.ComponentModel;
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

            // TODO: size the dialog to fit the screen.

            HyperlinkLuceneExamples.Click += HyperlinkLuceneExamples_Click;
        }

        private void HyperlinkLuceneExamples_Click(object sender, RoutedEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_LuceneQuerySyntax);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // base.OnClosed() invokes this calss Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

        }
    }
}
