using System.Windows;
using System.Windows.Controls;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.LibraryFilter.PublicationExplorerStuff
{
    /// <summary>
    /// Interaction logic for PublicationExplorerItemPopup.xaml
    /// </summary>
    public partial class PublicationExplorerItemPopup : UserControl
    {
        Library library;
        string source_tag;
        
        AugmentedPopup popup;


        public PublicationExplorerItemPopup(Library library, string source_tag)
        {
            this.library = library;
            this.source_tag = source_tag;

            InitializeComponent();

            MenuRenamePublication.Click += MenuRenamePublication_Click;

            popup = new AugmentedPopup(this);
        }

        void MenuRenamePublication_Click(object sender, RoutedEventArgs e)
        {
            using (var c = popup.AutoCloser)
            {
                PublicationExplorerItemRenameWindow dialog = new PublicationExplorerItemRenameWindow(library, source_tag);
                dialog.ShowDialog();
            }
        }
        
        public void Open()
        {
            popup.IsOpen = true;
        }
    }
}
