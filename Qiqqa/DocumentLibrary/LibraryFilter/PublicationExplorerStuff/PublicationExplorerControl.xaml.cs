using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using Qiqqa.DocumentLibrary.TagExplorerStuff;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Collections;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.LibraryFilter.PublicationExplorerStuff
{
    /// <summary>
    /// Interaction logic for TagExplorerControl.xaml
    /// </summary>
    public partial class PublicationExplorerControl : UserControl
    {
        private WebLibraryDetail web_library_detail;

        public delegate void OnTagSelectionChangedDelegate(HashSet<string> fingerprints, Span descriptive_span);
        public event OnTagSelectionChangedDelegate OnTagSelectionChanged;

        public PublicationExplorerControl()
        {
            InitializeComponent();

            ToolTip = "Here are the Publications of your documents.  " + GenericLibraryExplorerControl.YOU_CAN_FILTER_TOOLTIP;

            TagExplorerTree.DescriptionTitle = "Publication";

            TagExplorerTree.GetNodeItems = GetNodeItems;

            TagExplorerTree.OnItemPopup = OnItemPopup;

            TagExplorerTree.OnTagSelectionChanged += TagExplorerTree_OnTagSelectionChanged;
        }

        // -----------------------------

        public WebLibraryDetail LibraryRef
        {
            get => web_library_detail;
            set
            {
                web_library_detail = value;
                TagExplorerTree.LibraryRef = value;
            }
        }

        public void Reset()
        {
            TagExplorerTree.Reset();
        }

        // -----------------------------

        internal static MultiMapSet<string, string> GetNodeItems(WebLibraryDetail web_library_detail, HashSet<string> parent_fingerprints)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            List<PDFDocument> pdf_documents = null;
            if (null == parent_fingerprints)
            {
                pdf_documents = web_library_detail.Xlibrary.PDFDocuments;
            }
            else
            {
                pdf_documents = web_library_detail.Xlibrary.GetDocumentByFingerprints(parent_fingerprints);
            }
            Logging.Debug特("PublicationExplorerControl: processing {0} documents from library {1}", pdf_documents.Count, web_library_detail.Title);

            MultiMapSet<string, string> tags_with_fingerprints = new MultiMapSet<string, string>();
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                tags_with_fingerprints.Add(pdf_document.Publication ?? "(none)", pdf_document.Fingerprint);
            }

            return tags_with_fingerprints;
        }

        private void TagExplorerTree_OnTagSelectionChanged(HashSet<string> fingerprints, Span descriptive_span)
        {
            WPFDoEvents.SafeExec(() =>
            {
                OnTagSelectionChanged?.Invoke(fingerprints, descriptive_span);
            });
        }

        private void OnItemPopup(WebLibraryDetail web_library_detail, string item_tag)
        {
            PublicationExplorerItemPopup popup = new PublicationExplorerItemPopup(web_library_detail, item_tag);
            popup.Open();
        }
    }
}
