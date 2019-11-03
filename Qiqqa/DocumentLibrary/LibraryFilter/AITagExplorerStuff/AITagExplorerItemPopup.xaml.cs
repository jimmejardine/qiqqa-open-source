using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary.AITagsStuff;
using Qiqqa.Documents.PDF;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.LibraryFilter.AITagExplorerStuff
{
    /// <summary>
    /// Interaction logic for AITagExplorerItemPopup.xaml
    /// </summary>
    public partial class AITagExplorerItemPopup : UserControl
    {
        private Library library;
        private string source_tag;
        private AugmentedPopup popup;

        public AITagExplorerItemPopup(Library library, string source_tag)
        {
            this.library = library;
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
                MainWindowServiceDispatcher.Instance.ExploreAutoTagInBrainstorm(library.WebLibraryDetail.Id, source_tag);
            }
        }

        private void MenuPromoteToTag_Click(object sender, RoutedEventArgs e)
        {
            using (var c = popup.AutoCloser)
            {
                HashSet<string> fingerprints_with_autotag = library.AITagManager.AITags.GetDocumentsWithTag(source_tag);
                List<PDFDocument> pdf_documents_with_autotag = library.GetDocumentByFingerprints(fingerprints_with_autotag);
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
                var list = library.BlackWhiteListManager.ReadList();
                list.Add(new BlackWhiteListEntry(source_tag, BlackWhiteListEntry.ListType.Black));
                library.BlackWhiteListManager.WriteList(list);
            }
        }

        private void MenuAddWhitelist_Click(object sender, RoutedEventArgs e)
        {
            using (var c = popup.AutoCloser)
            {
                var list = library.BlackWhiteListManager.ReadList();
                list.Add(new BlackWhiteListEntry(source_tag, BlackWhiteListEntry.ListType.White));
                library.BlackWhiteListManager.WriteList(list);
            }
        }

        public void Open()
        {
            popup.IsOpen = true;
        }
    }
}
