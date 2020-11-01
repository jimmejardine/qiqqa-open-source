using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.TagExplorerStuff
{
    /// <summary>
    /// Interaction logic for TagExplorerItemPopup.xaml
    /// </summary>
    public partial class TagExplorerItemPopup : UserControl
    {
        private WebLibraryDetail web_library_detail;
        private string source_tag;
        private AugmentedPopup popup;


        public TagExplorerItemPopup(WebLibraryDetail web_library_detail, string source_tag)
        {
            this.web_library_detail = web_library_detail;
            this.source_tag = source_tag;

            InitializeComponent();

            MenuRenameTag.Click += MenuRenameTag_Click;
            MenuExploreTag.Click += MenuExploreTag_Click;

            popup = new AugmentedPopup(this);
        }

        private void MenuExploreTag_Click(object sender, RoutedEventArgs e)
        {
            using (var c = popup.AutoCloser)
            {
                MainWindowServiceDispatcher.Instance.ExploreTagInBrainstorm(web_library_detail.Id, source_tag);
            }
        }

        private void MenuRenameTag_Click(object sender, RoutedEventArgs e)
        {
            using (var c = popup.AutoCloser)
            {
                TagExplorerItemRenameWindow dialog = new TagExplorerItemRenameWindow(web_library_detail, source_tag);
                dialog.ShowDialog();
            }
        }

        public void Open()
        {
            popup.IsOpen = true;
        }
    }
}
