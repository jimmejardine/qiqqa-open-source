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
