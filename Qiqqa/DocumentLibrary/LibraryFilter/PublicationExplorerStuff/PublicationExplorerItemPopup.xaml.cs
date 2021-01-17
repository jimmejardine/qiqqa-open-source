using System.Windows;
using System.Windows.Controls;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.LibraryFilter.PublicationExplorerStuff
{
    /// <summary>
    /// Interaction logic for PublicationExplorerItemPopup.xaml
    /// </summary>
    public partial class PublicationExplorerItemPopup : UserControl
    {
        private WebLibraryDetail web_library_detail;
        private string source_tag;
        private AugmentedPopup popup;

        public PublicationExplorerItemPopup(WebLibraryDetail web_library_detail, string source_tag)
        {
            this.web_library_detail = web_library_detail;
            this.source_tag = source_tag;

            InitializeComponent();

            MenuRenamePublication.Click += MenuRenamePublication_Click;

            popup = new AugmentedPopup(this);
        }

        private void MenuRenamePublication_Click(object sender, RoutedEventArgs e)
        {
                PublicationExplorerItemRenameWindow dialog = new PublicationExplorerItemRenameWindow(web_library_detail, source_tag);
                dialog.ShowDialog();
        }

        public void Open()
        {
            popup.IsOpen = true;
        }
    }
}
