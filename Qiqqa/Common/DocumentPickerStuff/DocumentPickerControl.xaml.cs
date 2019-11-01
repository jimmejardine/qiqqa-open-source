using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF;
using Qiqqa.UtilisationTracking;
using Utilities;

namespace Qiqqa.Common.DocumentPickerStuff
{
    /// <summary>
    /// Interaction logic for DocumentPickerControl.xaml
    /// </summary>
    public partial class DocumentPickerControl : UserControl
    {
        public Feature Feature { get; set; }

        public DocumentPickerControl()
        {
            InitializeComponent();

            ObjLibraryPicker.OnWebLibraryPicked += ObjLibraryPicker_OnWebLibraryPicked;
            ObjSearchFilter.TextChanged += ObjSearchFilter_TextChanged;
        }

        private void ObjSearchFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyNewSearch();
        }

        private void ObjLibraryPicker_OnWebLibraryPicked(WebLibraryDetail web_library_detail)
        {
            ApplyNewSearch();
        }

        private void ApplyNewSearch()
        {
            ObjDocumentsPanel.Children.Clear();

            WebLibraryDetail web_library_detail = ObjLibraryPicker.WebLibraryDetail;
            if (null != web_library_detail)
            {
                string query = ObjSearchFilter.Text;
                if (!String.IsNullOrEmpty(query))
                {
                    HashSet<string> fingerprints = web_library_detail.library.GetDocumentFingerprintsWithKeyword(query);
                    List<PDFDocument> pdf_documents = web_library_detail.library.GetDocumentByFingerprints(fingerprints);

                    List<PDFDocument> pdf_documents_sorted = new List<PDFDocument>(pdf_documents.OrderBy(x => x.TitleCombined));
                    bool alternator = false;
                    foreach (PDFDocument pdf_document in pdf_documents_sorted)
                    {
                        TextBlock text_doc = ListFormattingTools.GetDocumentTextBlock(pdf_document, ref alternator, Feature, OnDocumentClicked);
                        ObjDocumentsPanel.Children.Add(text_doc);
                    }

                    // If the panel is empty, put NONE
                    if (0 == ObjDocumentsPanel.Children.Count)
                    {
                        TextBlock text_doc = new TextBlock();
                        text_doc.Text = "(none)";
                        ObjDocumentsPanel.Children.Add(text_doc);
                    }
                }
            }
        }

        private void OnDocumentClicked(object sender, MouseButtonEventArgs e)
        {
            TextBlock text_doc = (TextBlock)sender;
            ListFormattingTools.DocumentTextBlockTag tag = (ListFormattingTools.DocumentTextBlockTag)text_doc.Tag;

            Logging.Info("{0} was clicked", tag.pdf_document);
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void Test()
        {
            DocumentPickerControl dpc = new DocumentPickerControl();
            ControlHostingWindow w = new ControlHostingWindow("Document picker", dpc);
            w.Show();
        }
#endif

        #endregion
    }
}
