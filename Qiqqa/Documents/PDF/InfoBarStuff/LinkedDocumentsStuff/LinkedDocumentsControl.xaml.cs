using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Qiqqa.Common.GUI;
using Qiqqa.Documents.PDF.InfoBarStuff.CitationsStuff;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.Language.TextIndexing;
using Utilities.Misc;

namespace Qiqqa.Documents.PDF.InfoBarStuff.LinkedDocumentsStuff
{
    /// <summary>
    /// Interaction logic for LinkedDocumentsControl.xaml
    /// </summary>
    public partial class LinkedDocumentsControl : UserControl
    {
        private PDFDocument pdf_document;

        public LinkedDocumentsControl()
        {
            InitializeComponent();
            PreviewKeyDown += LinkedDocumentsControl_PreviewKeyDown;
            ObjSearchBox.OnSoftSearch += ObjSearchBox_OnSoftSearch;
        }

        private void LinkedDocumentsControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Enter == e.Key)
            {
                LinkSelectedDocument();
                e.Handled = true;
            }
            else if (Key.Escape == e.Key)
            {
                ObjSearchBox.Clear();
                e.Handled = true;
            }
            else if (Key.Up == e.Key)
            {
                try
                {
                    --ObjPDFDocuments.SelectedIndex;
                    ObjPDFDocuments.ScrollIntoView(ObjPDFDocuments.SelectedItem);
                    e.Handled = true;
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Key UP");
                }
            }
            else if (Key.Down == e.Key)
            {
                try
                {
                    ++ObjPDFDocuments.SelectedIndex;
                    ObjPDFDocuments.ScrollIntoView(ObjPDFDocuments.SelectedItem);
                    e.Handled = true;
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Key DOWN");
                }
            }
        }

        public void SetPDFDocument(PDFDocument doc)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            pdf_document = doc;

            WPFDoEvents.InvokeAsyncInUIThread(() =>
            {
                ObjPDFDocuments.ItemsSource = null;

                string query = ObjSearchBox.Text;

                SafeThreadPool.QueueUserWorkItem(() =>
                {
                    ReSearch(doc, query);
                    RepopulatePanels(doc);
                });
            });
        }

        private void RepopulatePanels(PDFDocument doc)
        {
            var links = doc.PDFDocumentCitationManager.GetLinkedDocuments();
            WPFDoEvents.InvokeInUIThread(() =>
            {
                CitationsUserControl.PopulatePanelWithCitations(DocsPanel_Linked, doc, links, Features.LinkedDocument_InfoBar_OpenDoc);
            });
        }

        private void ReSearch(PDFDocument doc, string query)
        {
            int MAX_DOCUMENTS = 20;

            if (null == doc) return;

            if (!String.IsNullOrEmpty(query))
            {
                List<IndexResult> matches = doc.LibraryRef.Xlibrary.LibraryIndex.GetFingerprintsForQuery(query);

                WPFDoEvents.InvokeInUIThread(() =>
                {
                    List<TextBlock> text_blocks = new List<TextBlock>();
                    bool alternator = false;
                    for (int i = 0; i < MAX_DOCUMENTS && i < matches.Count; ++i)
                    {
                        PDFDocument pdf_document = doc.LibraryRef.Xlibrary.GetDocumentByFingerprint(matches[i].fingerprint);
                        if (null == pdf_document || pdf_document.Deleted) continue;

                        string prefix = String.Format("{0:0%} - ", matches[i].score);
                        TextBlock text_block = ListFormattingTools.GetDocumentTextBlock(pdf_document, ref alternator, null, MouseButtonEventHandler, prefix, null);
                        text_blocks.Add(text_block);
                    }

                    ObjPDFDocuments.ItemsSource = text_blocks;
                    if (0 < text_blocks.Count)
                    {
                        ObjPDFDocuments.SelectedIndex = 0;
                    }
                });
            }
        }

        private void MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
        {
            LinkSelectedDocument();
        }

        private void LinkSelectedDocument()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.LinkedDocument_Create);

            TextBlock text_block = ObjPDFDocuments.SelectedItem as TextBlock;
            if (null != text_block)
            {
                ListFormattingTools.DocumentTextBlockTag tag = (ListFormattingTools.DocumentTextBlockTag)text_block.Tag;
                PDFDocument pdf_document_other = tag.pdf_document;
                pdf_document.PDFDocumentCitationManager.AddLinkedDocument(pdf_document_other.Fingerprint);
                pdf_document_other.PDFDocumentCitationManager.AddLinkedDocument(pdf_document.Fingerprint);
            }

            ObjSearchBox.Clear();

            var doc = this.pdf_document;

            SafeThreadPool.QueueUserWorkItem(() =>
            {
                RepopulatePanels(doc);
            });
        }

        private void ObjSearchBox_OnSoftSearch()
        {
            WPFDoEvents.SafeExec(() =>
            {
                var doc = this.pdf_document;
                string query = ObjSearchBox.Text;

                SafeThreadPool.QueueUserWorkItem(() =>
                {
                    ReSearch(doc, query);
                });
            });
        }
    }
}
