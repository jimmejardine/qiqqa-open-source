using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.TagExplorerStuff
{
    /// <summary>
    /// Interaction logic for TagExplorerItemPopup.xaml
    /// </summary>
    public partial class TagExplorerItemPopup : UserControl
    {
        Library library;
        string source_tag;
        
        AugmentedPopup popup;


        public TagExplorerItemPopup(Library library, string source_tag)
        {
            this.library = library;
            this.source_tag = source_tag;

            InitializeComponent();

            MenuRenameTag.Click += MenuRenameTag_Click;
            MenuExploreTag.Click += MenuExploreTag_Click;

            popup = new AugmentedPopup(this);
        }

        void MenuExploreTag_Click(object sender, RoutedEventArgs e)
        {
            using (var c = popup.AutoCloser)
            {
                MainWindowServiceDispatcher.Instance.ExploreTagInBrainstorm(library.WebLibraryDetail.Id, source_tag);
            }
        }

        void MenuRenameTag_Click(object sender, RoutedEventArgs e)
        {
            using (var c = popup.AutoCloser)
            {
                TagExplorerItemRenameWindow dialog = new TagExplorerItemRenameWindow(library, source_tag);
                dialog.ShowDialog();
            }
        }
        
        public void Open()
        {
            popup.IsOpen = true;
        }
    }
}
