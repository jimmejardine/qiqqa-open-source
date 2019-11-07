using System.Windows;
using System.Windows.Controls;
using icons;

namespace Qiqqa.DocumentLibrary.LibraryFilter
{
    /// <summary>
    /// Interaction logic for LuceneSearchHelpButton.xaml
    /// </summary>
    public partial class LuceneSearchHelpButton : UserControl
    {
        public LuceneSearchHelpButton()
        {
            InitializeComponent();

            SearchQuickHelp.Icon = Icons.GetAppIcon(Icons.Help);
            SearchQuickHelp.IconHeight = 16;
            SearchQuickHelp.Click += SearchQuickHelp_Click;

        }

        private void SearchQuickHelp_Click(object sender, RoutedEventArgs e)
        {
            LuceneSearchHelpControl control = new LuceneSearchHelpControl();
            control.Show();
        }
    }
}
