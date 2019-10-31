using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using icons;
using Qiqqa.Common.GUI;
using Qiqqa.Common.TagManagement;
using Qiqqa.Documents.PDF;
using Utilities.GUI;

namespace Qiqqa.DocumentLibrary.TagExplorerStuff
{
    /// <summary>
    /// Interaction logic for TagExplorerItemRenameWindow.xaml
    /// </summary>
    public partial class TagExplorerItemRenameWindow : StandardWindow
    {
        Library library;
        string tag;

        public TagExplorerItemRenameWindow(Library library, string tag)
        {
            this.library = library;
            this.tag = tag;

            InitializeComponent();
            
            this.Title = "Qiqqa - Rename or Delete Tag";

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

        void TextNewTagName_KeyUp(object sender, KeyEventArgs e)
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
                CmdGenerate.Caption = String.Format("Delete Tag '{0}'", tag);
            }
            else
            {
                CmdGenerate.Caption = String.Format("Rename Tag '{0} to '{1}'", tag, new_tag);
            }
        }

        void CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        void CmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            string new_tag = TextNewTagName.Text;

            if (String.IsNullOrEmpty(new_tag))
            {
                if (!MessageBoxes.AskQuestion("You have specified an EMPTY new tag name.  This will effectively delete tag '{0}' from your library.  Are you sure you want to do this?", tag))
                {
                    return;
                }
            }
            
            // Rename all the tags in the documents
            foreach (PDFDocument pdf_document in library.PDFDocuments)
            {
                // Rename the tags in the annotations
                foreach (PDFAnnotation pdf_annotation in pdf_document.GetAnnotations())
                {
                    // This fast search will flag is the tag appears in the substring
                    if (null != pdf_annotation.Tags && pdf_annotation.Tags.Contains(tag))
                    {
                        HashSet<string> tags = TagTools.ConvertTagBundleToTags(pdf_annotation.Tags);

                        // Now do a proper check for the appearance of the tag
                        if (tags.Contains(tag))
                        {
                            tags.Remove(tag);
                            if (!String.IsNullOrEmpty(new_tag))
                            {
                                tags.Add(new_tag);
                            }
                            pdf_annotation.Tags = TagTools.ConvertTagListToTagBundle(tags);
                            pdf_annotation.Bindable.NotifyPropertyChanged(() => pdf_annotation.Tags);
                        }
                    }
                }

                // Rename the document tags
                if (pdf_document.Tags.Contains(tag))
                {
                    pdf_document.RemoveTag(tag);
                    if (!String.IsNullOrEmpty(new_tag))
                    {
                        pdf_document.AddTag(new_tag);
                    }
                }
            }
            
            this.DialogResult = true;
            this.Close();
        }

        static int CountDocumentsWithTag(Library library, string search_tag)
        {
            int count = 0;
            foreach (PDFDocument pdf_document in library.PDFDocuments)
            {
                foreach (string tag in TagTools.ConvertTagBundleToTags(pdf_document.Tags))
                {
                    if (0 == String.Compare(tag, search_tag))
                    {
                        ++count;
                        break;
                    }
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

            // base.OnClosed() invokes this calss Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

            this.library = null;
        }
    }
}
