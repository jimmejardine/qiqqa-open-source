using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary.AITagsStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.LibraryFilter.AITagExplorerStuff
{
    /// <summary>
    /// Interaction logic for AITagExplorerItemPopup.xaml
    /// </summary>
    public partial class AITagExplorerItemPopup : UserControl
    {
        private WebLibraryDetail web_library_detail;
        private string source_tag;
        private AugmentedPopup popup;

        public AITagExplorerItemPopup(WebLibraryDetail library, string source_tag)
        {
            web_library_detail = library;
            this.source_tag = source_tag;

            InitializeComponent();

            MenuExploreTag.Click += MenuExploreTag_Click;
            MenuAddWhitelist.Click += MenuAddWhitelist_Click;
            MenuAddBlacklist.Click += MenuAddBlacklist_Click;
            MenuPromoteToTag.Click += MenuPromoteToTag_Click;

            popup = new AugmentedPopup(this);
        }

        private void MenuExploreTag_Click(object sender, RoutedEventArgs e)
        {
            using (var c = popup.AutoCloser)
            {
                MainWindowServiceDispatcher.Instance.ExploreAutoTagInBrainstorm(web_library_detail.Id, source_tag);
            }
        }

        private void MenuPromoteToTag_Click(object sender, RoutedEventArgs e)
        {
            using (var c = popup.AutoCloser)
            {
                HashSet<string> fingerprints_with_autotag = web_library_detail.Xlibrary.AITagManager.AITags.GetDocumentsWithTag(source_tag);
                List<PDFDocument> pdf_documents_with_autotag = web_library_detail.Xlibrary.GetDocumentByFingerprints(fingerprints_with_autotag);
                foreach (var pdf_document in pdf_documents_with_autotag)
                {
                    pdf_document.AddTag(source_tag);
                }
            }
        }

        private void MenuAddBlacklist_Click(object sender, RoutedEventArgs e)
        {
            using (var c = popup.AutoCloser)
            {
                var list = web_library_detail.Xlibrary.BlackWhiteListManager.ReadList();
                list.Add(new BlackWhiteListEntry(source_tag, BlackWhiteListEntry.ListType.Black));
                web_library_detail.Xlibrary.BlackWhiteListManager.WriteList(list);
            }
        }

        private void MenuAddWhitelist_Click(object sender, RoutedEventArgs e)
        {
            using (var c = popup.AutoCloser)
            {
                var list = web_library_detail.Xlibrary.BlackWhiteListManager.ReadList();
                list.Add(new BlackWhiteListEntry(source_tag, BlackWhiteListEntry.ListType.White));
                web_library_detail.Xlibrary.BlackWhiteListManager.WriteList(list);
            }
        }

        public void Open()
        {
            popup.IsOpen = true;
        }
    }
}
