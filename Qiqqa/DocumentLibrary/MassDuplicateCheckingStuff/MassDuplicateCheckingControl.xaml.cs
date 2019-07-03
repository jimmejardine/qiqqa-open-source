using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Qiqqa.Common;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary.LibraryCatalog;
using Qiqqa.Documents.PDF;
using Qiqqa.Documents.PDF.InfoBarStuff.DuplicateDetectionStuff;
using Utilities;
using Utilities.Misc;

namespace Qiqqa.DocumentLibrary.MassDuplicateCheckingStuff
{
    /// <summary>
    /// Interaction logic for MassDuplicateCheckingControl.xaml
    /// </summary>
    public partial class MassDuplicateCheckingControl : UserControl
    {
        object locker = new object();
        bool already_finding_duplicates = false;

        public MassDuplicateCheckingControl()
        {
            InitializeComponent();

            TreeDuplicates.SelectedItemChanged += TreeDuplicates_SelectedItemChanged;
            TreeDuplicates.KeyDown += TreeDuplicates_KeyDown;
        }

        void TreeDuplicates_KeyDown(object sender, KeyEventArgs e)
        {
            TreeViewItem tvi = TreeDuplicates.SelectedItem as TreeViewItem;
            PDFDocument pdf_document = (null != tvi) ? (PDFDocument)tvi.Tag : null;
            
            if (false) {}
            else if (Key.Delete == e.Key)
            {
                if (null != pdf_document)
                {
                    pdf_document.Deleted = true;
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.Deleted);
                    e.Handled = true;
                }
            }
            else if (Key.Insert == e.Key)
            {
                if (null != pdf_document)
                {
                    pdf_document.Deleted = false;
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.Deleted);
                    e.Handled = true;
                }
            }
        }

        void TreeDuplicates_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem tvi = TreeDuplicates.SelectedItem as TreeViewItem;
            if (null != tvi)
            {
                PDFDocument pdf_document = (PDFDocument)tvi.Tag;
                ObjDocumentMetadataControlsPanel.DataContext = pdf_document.Bindable;
            }
            else
            {
                ObjDocumentMetadataControlsPanel.DataContext = null;
            }
        }

        public void FindDuplicates(Library library)
        {
            lock(locker)
            {
                if (already_finding_duplicates)
                {
                    Logging.Warn("Not finding duplicates while a previous invocation is still running.");
                    return;
                }
                else
                {
                    already_finding_duplicates = true;
                }
            }

            SafeThreadPool.QueueUserWorkItem(o => FindDuplicates_BACKGROUND(library));
        }

        private void FindDuplicates_BACKGROUND(Library library)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    TxtLibraryName.Text = library.WebLibraryDetail.Title;
                    TreeDuplicates.Items.Clear();
                    TxtNoDuplicatesFound.Visibility = Visibility.Collapsed;
                }
                ));

                List<PDFDocument> pdf_documents = library.PDFDocuments;

                // Sort them by their titles
                StatusManager.Instance.UpdateStatus("DuplicateChecking", "Sorting titles");
                pdf_documents.Sort(delegate(PDFDocument d1, PDFDocument d2)
                {
                    return String.Compare(d1.TitleCombined, d2.TitleCombined);
                });

                StatusManager.Instance.UpdateStatus("DuplicateChecking", "Caching titles");
                DuplicateDetectionControl.TitleCombinedCache cache = new DuplicateDetectionControl.TitleCombinedCache(pdf_documents);
                
                // Do the n^2                
                bool have_duplicates = false;
                StatusManager.Instance.ClearCancelled("DuplicateChecking");
                for (int i = 0; i < pdf_documents.Count; ++i)
                {
                    StatusManager.Instance.UpdateStatus("DuplicateChecking", "Checking for duplicates", i, pdf_documents.Count, true);
                    if (StatusManager.Instance.IsCancelled("DuplicateChecking"))
                    {
                        Logging.Warn("User cancelled duplicate checking");
                        break;
                    }
                    
                    PDFDocument pdf_document = pdf_documents[i];
                    List<PDFDocument> duplicate_pdf_documents = DuplicateDetectionControl.FindDuplicates(pdf_document, cache);
                    if (0 < duplicate_pdf_documents.Count)
                    {
                        have_duplicates = true;
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            TreeViewItem tvi_parent = new TreeViewItem();
                            AttachEvents(tvi_parent, pdf_document);
                            foreach (PDFDocument pdf_document_child in duplicate_pdf_documents)
                            {
                                TreeViewItem tvi_child = new TreeViewItem();
                                AttachEvents(tvi_child, pdf_document_child);
                                tvi_parent.Items.Add(tvi_child);
                            }

                            TreeDuplicates.Items.Add(tvi_parent);
                        }
                        ));
                    }
                }

                if (!have_duplicates)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        TxtNoDuplicatesFound.Visibility = Visibility.Visible;
                    }
                    ));
                }

                StatusManager.Instance.UpdateStatus("DuplicateChecking", "Finished checking for duplicates");
            }

            finally
            {
                lock (locker)
                {
                    already_finding_duplicates = false;
                }
            }
        }

        private void AttachEvents(TreeViewItem tvi, PDFDocument pdf_document)
        {
            string prefix = 
                pdf_document.ReadingStage 
                + " - "
                + (String.IsNullOrEmpty(pdf_document.BibTex) ? "NoBibTeX" : "HasBibTeX")
                + " - "
                + (!pdf_document.DocumentExists ? "NoPDF" : "HasPDF")
                + " - "
                ;
            tvi.Header = ListFormattingTools.GetPDFDocumentDescription(pdf_document, prefix);
            tvi.Tag = pdf_document;
            tvi.MouseRightButtonUp += tvi_MouseRightButtonUp;
        }

        void tvi_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)sender;
            PDFDocument pdf_document = (PDFDocument)tvi.Tag;
            tvi.IsSelected = true;

            List<PDFDocument> pdf_documents = new List<PDFDocument>();
            pdf_documents.Add(pdf_document);
            LibraryCatalogPopup popup = new LibraryCatalogPopup(pdf_documents);
            popup.Open();

            e.Handled = true;
        }


        public static void FindDuplicatesForLibrary(Library library)
        {
            MassDuplicateCheckingControl control = new MassDuplicateCheckingControl();
            MainWindowServiceDispatcher.Instance.OpenUserControl("Find Duplicates", Icons.GetAppIcon(Icons.LibraryFindDuplicates), control);
            control.FindDuplicates(library);
        }
    }
}
