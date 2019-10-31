using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using Qiqqa.DocumentLibrary.TagExplorerStuff;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Collections;

namespace Qiqqa.DocumentLibrary.LibraryFilter.GeneralExplorers
{
    /// <summary>
    /// Interaction logic for TagExplorerControl.xaml
    /// </summary>
    public partial class RatingExplorerControl : UserControl
    {
        private Library library;

        public delegate void OnTagSelectionChangedDelegate(HashSet<string> fingerprints, Span descriptive_span);
        public event OnTagSelectionChangedDelegate OnTagSelectionChanged;

        public RatingExplorerControl()
        {
            InitializeComponent();

            this.ToolTip = "Here are the Ratings of your documents.  " + GenericLibraryExplorerControl.YOU_CAN_FILTER_TOOLTIP;

            TagExplorerTree.DescriptionTitle = "Rating";

            TagExplorerTree.GetNodeItems = GetNodeItems;

            TagExplorerTree.OnTagSelectionChanged += TagExplorerTree_OnTagSelectionChanged;
        }

        // -----------------------------

        public Library Library
        {
            get
            {
                return library;
            }
            set
            {
                this.library = value;
                TagExplorerTree.Library = value;
            }
        }

        public void Reset()
        {
            TagExplorerTree.Reset();
        }

        // -----------------------------

        internal static MultiMapSet<string, string> GetNodeItems(Library library, HashSet<string> parent_fingerprints)
        {
            List<PDFDocument> pdf_documents = null;
            if (null == parent_fingerprints)
            {
                pdf_documents = library.PDFDocuments;
            }
            else
            {
                pdf_documents = library.GetDocumentByFingerprints(parent_fingerprints);
            }
            Logging.Debug特("RatingExplorerControl: processing {0} documents from library {1}", pdf_documents.Count, library.WebLibraryDetail.Title);

            MultiMapSet<string, string> tags_with_fingerprints = new MultiMapSet<string, string>();
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                tags_with_fingerprints.Add(pdf_document.Rating ?? "(none)", pdf_document.Fingerprint);
            }

            return tags_with_fingerprints;
        }

        void TagExplorerTree_OnTagSelectionChanged(HashSet<string> fingerprints, Span descriptive_span)
        {
            OnTagSelectionChanged?.Invoke(fingerprints, descriptive_span);
        }
    }
}
