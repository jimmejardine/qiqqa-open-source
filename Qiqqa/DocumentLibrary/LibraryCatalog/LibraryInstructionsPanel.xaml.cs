using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common.Configuration;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.LibraryCatalog
{
    /// <summary>
    /// Interaction logic for LibraryInstructionsPanel.xaml
    /// </summary>
    public partial class LibraryInstructionsPanel : UserControl
    {
        public LibraryInstructionsPanel()
        {
            InitializeComponent();

            ObjScrollerHelp.Visibility = Visibility.Collapsed;
            ObjHyperlinkHelp.Click += ObjHyperlinkHelp_Click;

            HyperlinkPublicStatus.Click += HyperlinkPublicStatus_Click;
            DataContextChanged += LibraryInstructionsPanel_DataContextChanged;
        }

        private void LibraryInstructionsPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Library library = DataContext as Library;
            if (null != library)
            {
                if (!library.WebLibraryDetail.IsWebLibrary)
                {
                    BorderPublicStatus.Visibility = Visibility.Collapsed;
                }
                else
                {
                    BorderPublicStatus.Visibility = Visibility.Visible;
                }
            }
        }

        private void HyperlinkPublicStatus_Click(object sender, RoutedEventArgs e)
        {
            Library library = DataContext as Library;
            if (null != library)
            {
                MessageBoxes.Info(
                    ""
                    + "You can make this library publicly available.  Others will be able to see your paper properties (title, authors, bibtex, tags), but not download the actual PDFs."
                    + "\n\nYou will get a permanent web address to share by email or link to from your homepage that allows others to explore and follow your up-to-date reading list."
                );

                WebsiteAccess.ChangeLibraryPublicStatus(library.WebLibraryDetail.ShortWebId);
            }
        }

        private void ObjHyperlinkHelp_Click(object sender, RoutedEventArgs e)
        {
            ObjScrollerHelp.Visibility =
                Visibility.Collapsed == ObjScrollerHelp.Visibility
                ? Visibility.Visible
                : Visibility.Collapsed
                ;
        }
    }
}
