using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary.LibraryFilter;
using Qiqqa.Documents.PDF;
using Utilities;
using Utilities.GUI;
using Utilities.Reflection;

namespace Qiqqa.DocumentLibrary.LibraryCatalog
{
    /// <summary>
    /// Interaction logic for LibraryCatalogControl.xaml
    /// </summary>
    public partial class LibraryCatalogControl : UserControl
    {
        DragToLibraryManager drag_to_library_manager;

        public delegate void SelectionChangedDelegate(List<PDFDocument> selected_pdf_documents);
        public event SelectionChangedDelegate SelectionChanged;

        public LibraryCatalogControl()
        {
            Theme.Initialize();

            InitializeComponent();

            ListPDFDocuments.Background = ThemeColours.Background_Brush_Blue_LightToDark;

            ListPDFDocuments.SelectionChanged += ListPDFDocuments_SelectionChanged;
            ListPDFDocuments.MouseDoubleClick += ListPDFDocuments_MouseDoubleClick;
            ListPDFDocuments.IsVisibleChanged += ListPDFDocuments_IsVisibleChanged;
            ReconsiderPDFDocumentDetail();
        }

        private void ListPDFDocuments_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ListPDFDocuments.IsVisible)
            {
                ListPDFDocuments.UpdateLayout();
            }
        }

        internal void OnFilterChanged(LibraryFilterControl library_filter_control, List<PDFDocument> pdf_documents, Span descriptive_span, string filter_terms, Dictionary<string, double> search_scores, PDFDocument pdf_document_to_focus_on)
        {
            SetPDFDocuments(pdf_documents, pdf_document_to_focus_on, filter_terms, search_scores);
        }
        
        void ListPDFDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReconsiderPDFDocumentDetail();

            SelectionChanged?.Invoke(SelectedPDFDocuments);
        }

        private void ReconsiderPDFDocumentDetail()
        {
            List<PDFDocument> selected_pdf_documents = SelectedPDFDocuments;
            if (0 == selected_pdf_documents.Count)
            {
                ObjLibraryInstructionsPanel.Visibility = Visibility.Visible;

                ObjDocumentMetadataControlsPanel.DataContext = null;
                ObjDocumentMetadataControlsPanel.Visibility = Visibility.Collapsed;

                ObjMultipleDocumentsSelectedPanel.DataContext = null;
                ObjMultipleDocumentsSelectedPanel.Visibility = Visibility.Collapsed;
            }
            else if (1 == selected_pdf_documents.Count)
            {
                ObjLibraryInstructionsPanel.Visibility = Visibility.Collapsed;

                ObjDocumentMetadataControlsPanel.DataContext = selected_pdf_documents[0].Bindable;
                ObjDocumentMetadataControlsPanel.Visibility = Visibility.Visible;

                ObjMultipleDocumentsSelectedPanel.DataContext = null;
                ObjMultipleDocumentsSelectedPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                ObjLibraryInstructionsPanel.Visibility = Visibility.Collapsed;

                ObjDocumentMetadataControlsPanel.DataContext = null;
                ObjDocumentMetadataControlsPanel.Visibility = Visibility.Collapsed;

                ObjMultipleDocumentsSelectedPanel.DataContext = selected_pdf_documents;
                ObjMultipleDocumentsSelectedPanel.Visibility = Visibility.Visible;
            }
        }

        void ListPDFDocuments_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            List<PDFDocument> selected_pdf_documents = SelectedPDFDocuments;
            foreach (PDFDocument selected_pdf_document in selected_pdf_documents)
            {
                MainWindowServiceDispatcher.Instance.OpenDocument(selected_pdf_document);
            }
        }

        private string filter_terms;
        public string FilterTerms
        {
            get
            {
                return filter_terms;
            }
        }

        private Dictionary<string, double> search_scores;
        public Dictionary<string, double> SearchScores
        {
            get
            {
                return search_scores;
            }
        }


        private Library library;        
        public Library Library
        {
            set
            {
                if (null != library)
                {
                    throw new Exception("Library can only be assigned once!");
                }

                this.library = value;
                this.DataContext = value;

                drag_to_library_manager = new DragToLibraryManager(library);
                drag_to_library_manager.RegisterControl(this);
            }
        }

        public void SetPDFDocuments(IEnumerable<PDFDocument> pdf_documents, PDFDocument pdf_document_to_focus_on)
        {
            SetPDFDocuments(pdf_documents, pdf_document_to_focus_on, null, null);
        }

        public void SetPDFDocuments(IEnumerable<PDFDocument> pdf_documents, PDFDocument pdf_document_to_focus_on, string filter_terms, Dictionary<string, double> search_scores)
        {
            this.filter_terms = filter_terms;
            this.search_scores = search_scores;

            ListPDFDocuments.SelectedValue = null;

            List<AugmentedBindable<PDFDocument>> pdf_documents_bindable = new List<AugmentedBindable<PDFDocument>>();
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                if (!pdf_document.Deleted)
                {
                    pdf_documents_bindable.Add(pdf_document.Bindable);
                }
            }
            ListPDFDocuments.DataContext = pdf_documents_bindable;

            // Scroll to the top
            if (null == pdf_document_to_focus_on)
            {
                GUITools.ScrollToTop(ListPDFDocuments);
            }
            else
            {
                FocusOnDocument(pdf_document_to_focus_on);
            }
        }

        public void FocusOnDocument(PDFDocument pdf_document_to_focus_on)
        {
            // Nothing to do here
            if (null == pdf_document_to_focus_on)
            {
                return;
            }
            
            // Find the selected document
            int selected_index = -1;
            int count = 0;
            List<AugmentedBindable<PDFDocument>> pdf_documents_bindable = (List<AugmentedBindable<PDFDocument>>)ListPDFDocuments.DataContext;
            foreach (AugmentedBindable<PDFDocument> pdf_document_bindable in pdf_documents_bindable)
            {
                PDFDocument pdf_document = pdf_document_bindable.Underlying;

                if (pdf_document == pdf_document_to_focus_on)
                {
                    selected_index = count;
                }
                ++count;
            }

            try
            {
                if (-1 == selected_index)
                {
                    GUITools.ScrollToTop(ListPDFDocuments);
                }
                else if (0 == selected_index)
                {
                    ListPDFDocuments.SelectedIndex = selected_index;
                    GUITools.ScrollToTop(ListPDFDocuments);
                }
                else
                {
                    ListPDFDocuments.SelectedIndex = selected_index;
                    ListPDFDocuments.UpdateLayout();
                    ListPDFDocuments.ScrollIntoView(pdf_documents_bindable[selected_index]);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "unexpected exception in FocusOnDocument");
            }
        }

        /// <summary>
        /// Returns a list of the currently selected PDF documents.
        /// You can change this list as you see fit.
        /// </summary>
        public List<PDFDocument> SelectedPDFDocuments
        {
            get
            {
                // Get a set of the selected docs
                HashSet<string> selected_documents = new HashSet<string>();
                foreach (AugmentedBindable<PDFDocument> bindable in ListPDFDocuments.SelectedItems)
                {
                    selected_documents.Add(bindable.Underlying.Fingerprint);
                }

                List<PDFDocument> pdf_documents = new List<PDFDocument>();

                // Now select the docs in the correct order
                List<AugmentedBindable<PDFDocument>> pdf_documents_bindable = ListPDFDocuments.DataContext as List<AugmentedBindable<PDFDocument>>;
                if (null != pdf_documents_bindable)
                {
                    foreach (AugmentedBindable<PDFDocument> bindable in pdf_documents_bindable)
                    {
                        if (selected_documents.Contains(bindable.Underlying.Fingerprint))
                        {
                            pdf_documents.Add(bindable.Underlying);
                        }
                    }
                }

                return pdf_documents;
            }
        }

        /// <summary>
        /// Returns a list of the currently selected PDF documents.  If nothing is selected, return everything
        /// You can change this list as you see fit.
        /// </summary>
        public List<PDFDocument> SelectedPDFDocumentsElseEverything
        {
            get
            {
                List<PDFDocument> pdf_documents = SelectedPDFDocuments;

                bool use_all_pdf_document = false;

                if (1 == pdf_documents.Count)
                {
                    if (MessageBoxes.AskQuestion("You have selected only one document for this operation.  Would you prefer to use all the documents in the library?"))
                    {
                        use_all_pdf_document = true;
                    }
                }

                if (0 == pdf_documents.Count)
                {
                    use_all_pdf_document = true;
                }

                if (use_all_pdf_document)
                {
                    pdf_documents.Clear();
                    foreach (AugmentedBindable<PDFDocument> bindable in ListPDFDocuments.Items)
                    {
                        pdf_documents.Add(bindable.Underlying);
                    }
                }

                return pdf_documents;
            }
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestHarness()
        {
            Library library = Library.GuestInstance;
            while (!library.LibraryIsLoaded) { Thread.Sleep(1000); }
            LibraryCatalogControl lcc = new LibraryCatalogControl();
            lcc.Library = library;


            ControlHostingWindow window = new ControlHostingWindow("Fast library control", lcc);
            window.Show();
        }
#endif

        #endregion
    }    
}
