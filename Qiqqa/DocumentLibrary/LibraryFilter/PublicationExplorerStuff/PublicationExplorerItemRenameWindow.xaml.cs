using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using icons;
using Qiqqa.Common.GUI;
using Qiqqa.Documents.PDF;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.LibraryFilter.PublicationExplorerStuff
{
    /// <summary>
    /// Interaction logic for PublicationExplorerItemRenameWindow.xaml
    /// </summary>
    public partial class PublicationExplorerItemRenameWindow : StandardWindow
    {
        private Library library;
        private string tag;

        public PublicationExplorerItemRenameWindow(Library library, string tag)
        {
            this.library = library;
            this.tag = tag;

            InitializeComponent();

            Title = "Qiqqa - Rename or Delete Publication";

            CmdGenerate.Caption = "Rename";
            CmdGenerate.Icon = Icons.GetAppIcon(Icons.LibraryAnnotationsReport);
            CmdCancel.Caption = "Cancel";
            CmdCancel.Icon = Icons.GetAppIcon(Icons.Cancel);

            CmdGenerate.Click += CmdGenerate_Click;
            CmdCancel.Click += CmdCancel_Click;

            TextNewTagName.Text = tag;
            TextNewTagName.SelectAll();
            TextNewTagName.Focus();
            TextNewTagName.KeyUp += TextNewTagName_KeyUp;

            RefreshSpans();
        }

        private static void SetSpan(Span span, string text)
        {
            span.Inlines.Clear();
            span.Inlines.Add(text);
        }

        private void TextNewTagName_KeyUp(object sender, KeyEventArgs e)
        {
            if (Key.Enter == e.Key)
            {
                CmdGenerate_Click(null, null);
                return;
            }
            else if (Key.Escape == e.Key)
            {
                CmdCancel_Click(null, null);
                return;
            }

            RefreshSpans();
        }

        private void RefreshSpans()
        {
            SetSpan(RegionOldTagName, tag);
            SetSpan(RegionOldTagDocumentCount, "" + CountDocumentsWithTag(library, tag));

            string new_tag = TextNewTagName.Text;
            SetSpan(RegionNewTagDocumentCount, "" + CountDocumentsWithTag(library, new_tag));

            if (String.IsNullOrEmpty(new_tag))
            {
                CmdGenerate.Caption = String.Format("Delete Publication '{0}'", tag);
            }
            else
            {
                CmdGenerate.Caption = String.Format("Rename Publication '{0} to '{1}'", tag, new_tag);
            }
        }

        private void CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void CmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            string new_tag = TextNewTagName.Text;

            if (String.IsNullOrEmpty(new_tag))
            {
                if (!MessageBoxes.AskQuestion("You have specified an EMPTY new publication name.  This will effectively delete publication '{0}' from your library.  Are you sure you want to do this?", tag))
                {
                    return;
                }
            }

            // Rename all the tags in the documents
            foreach (PDFDocument pdf_document in library.PDFDocuments)
            {
                if (0 == String.Compare(pdf_document.Publication, tag))
                {
                    pdf_document.Publication = new_tag;
                    pdf_document.Bindable.NotifyPropertyChanged(() => pdf_document.Publication);
                }
            }

            DialogResult = true;
            Close();
        }

        private static int CountDocumentsWithTag(Library library, string search_tag)
        {
            int count = 0;
            foreach (PDFDocument pdf_document in library.PDFDocuments)
            {
                if (0 == String.Compare(search_tag, pdf_document.Publication))
                {
                    ++count;
                }
            }
            return count;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // base.OnClosed() invokes this class' Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

            library = null;
        }
    }
}
