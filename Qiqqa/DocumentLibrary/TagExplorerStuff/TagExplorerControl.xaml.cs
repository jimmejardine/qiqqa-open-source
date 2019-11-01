using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Qiqqa.Common.TagManagement;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.Collections;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.TagExplorerStuff
{
    /// <summary>
    /// Interaction logic for TagExplorerControl.xaml
    /// </summary>
    public partial class TagExplorerControl : UserControl
    {
        private Library library;

        public delegate void OnTagSelectionChangedDelegate(HashSet<string> fingerprints, Span descriptive_span);
        public event OnTagSelectionChangedDelegate OnTagSelectionChanged;

        private static readonly string NO_TAG_KEY = "<Untagged>";

        public TagExplorerControl()
        {
            Theme.Initialize();

            InitializeComponent();

            ToolTip = "Here are the Tags you have added to your documents.  " + GenericLibraryExplorerControl.YOU_CAN_FILTER_TOOLTIP;

            TagExplorerTree.DescriptionTitle = "Tags";

            TagExplorerTree.GetNodeItems = GetNodeItems;
            TagExplorerTree.OnItemPopup = OnItemPopup;
            TagExplorerTree.OnItemDragOver = OnItemDragOver;
            TagExplorerTree.OnItemDrop = OnItemDrop;

            TagExplorerTree.OnTagSelectionChanged += TagExplorerTree_OnTagSelectionChanged;
        }

        // -----------------------------

        public Library Library
        {
            get => library;
            set
            {
                library = value;
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
            Logging.Info("+Getting node items for Tags");

            List<PDFDocument> pdf_documents = null;
            if (null == parent_fingerprints)
            {
                pdf_documents = library.PDFDocuments;
            }
            else
            {
                pdf_documents = library.GetDocumentByFingerprints(parent_fingerprints);
            }
            Logging.Debug特("TagExplorerControl: processing {0} documents from library {1}", pdf_documents.Count, library.WebLibraryDetail.Title);

            // Load all the annotations upfront so we dont have to go to the database for each PDF
            Dictionary<string, byte[]> library_items_annotations_cache = library.LibraryDB.GetLibraryItemsAsCache(PDFDocumentFileLocations.ANNOTATIONS);

            // Build up the map of PDFs associated with each tag
            MultiMapSet<string, string> tags_with_fingerprints = new MultiMapSet<string, string>();
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                bool has_tag = false;
                foreach (string tag in TagTools.ConvertTagBundleToTags(pdf_document.Tags))
                {
                    tags_with_fingerprints.Add(tag, pdf_document.Fingerprint);
                    has_tag = true;
                }

                // And check the annotations                
                foreach (var pdf_annotation in pdf_document.GetAnnotations(library_items_annotations_cache))
                {
                    if (!pdf_annotation.Deleted)
                    {
                        foreach (string annotation_tag in TagTools.ConvertTagBundleToTags(pdf_annotation.Tags))
                        {
                            tags_with_fingerprints.Add(annotation_tag, pdf_document.Fingerprint);
                            has_tag = true;
                        }
                    }
                }

                if (!has_tag)
                {
                    tags_with_fingerprints.Add(NO_TAG_KEY, pdf_document.Fingerprint);
                }
            }

            Logging.Info("-Getting node items");
            return tags_with_fingerprints;
        }

        private void OnItemPopup(Library library, string item_tag)
        {
            TagExplorerItemPopup popup = new TagExplorerItemPopup(library, item_tag);
            popup.Open();
        }

        private void OnItemDragOver(Library library, string item_tag, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PDFDocument)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(List<PDFDocument>)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void OnItemDrop(Library library, string item_tag, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PDFDocument)))
            {
                PDFDocument pdf_document = (PDFDocument)e.Data.GetData(typeof(PDFDocument));
                Logging.Info("The PDF dropped onto tag {1} is {0}", pdf_document, item_tag);

                pdf_document.AddTag(item_tag);
            }
            else if (e.Data.GetDataPresent(typeof(List<PDFDocument>)))
            {
                List<PDFDocument> pdf_documents = (List<PDFDocument>)e.Data.GetData(typeof(List<PDFDocument>));
                Logging.Info("The PDF list dropped onto tag {1} has {0} items", pdf_documents.Count, item_tag);

                foreach (PDFDocument pdf_document in pdf_documents)
                {
                    pdf_document.AddTag(item_tag);
                }
            }

            e.Handled = true;
        }

        private void TagExplorerTree_OnTagSelectionChanged(HashSet<string> fingerprints, Span descriptive_span)
        {
            OnTagSelectionChanged?.Invoke(fingerprints, descriptive_span);
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestHarness()
        {
            Library library = Library.GuestInstance;
            TagExplorerControl tec = new TagExplorerControl();
            tec.Library = library;

            ControlHostingWindow w = new ControlHostingWindow("Tags", tec);
            w.Show();
        }
#endif

        #endregion
    }
}
