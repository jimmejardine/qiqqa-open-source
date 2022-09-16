using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using icons;
using Qiqqa.Common.GUI;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for QuickTagsControl.xaml
    /// </summary>
    public partial class QuickAddTagsWindow : StandardWindow
    {
        private List<PDFDocument> pdf_documents;

        public QuickAddTagsWindow(List<PDFDocument> pdf_documents)
        {
            this.pdf_documents = pdf_documents;

            InitializeComponent();

            Title = "Qiqqa - Add Tags";

            CmdGenerate.Caption = "Add tags\n(CTRL+ENTER)";
            CmdGenerate.Icon = Icons.GetAppIcon(Icons.Next);
            CmdCancel.Caption = "Cancel\n(ESC)";
            CmdCancel.Icon = Icons.GetAppIcon(Icons.Cancel);

            CmdGenerate.Click += CmdGenerate_Click;
            CmdCancel.Click += CmdCancel_Click;

            KeyUp += QuickAddTagsWindow_KeyUp;

            SetSpan(RegionDocumentCount, "" + pdf_documents.Count);

            Keyboard.Focus(ObjTagEditorControl.ObjAddControl.ComboBoxNewTag);
            ObjTagEditorControl.ObjAddControl.ComboBoxNewTag.Focus();
        }

        private void QuickAddTagsWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DoClose();
                e.Handled = true;
            }
            else if (e.Key == Key.Enter && KeyboardTools.IsCTRLDown())
            {
                AddTagsFromTextBox();
                DoClose();
                e.Handled = true;
            }
        }

        private void DoClose()
        {
            Close();
        }

        private static void SetSpan(Span span, string text)
        {
            span.Inlines.Clear();
            span.Inlines.Add(text);
        }

        private void CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            DoClose();
        }

        private void AddTagsFromTextBox()
        {
            // Get the tags
            string tags = ObjTagEditorControl.TagsBundle;
            if (String.IsNullOrEmpty(tags))
            {
                Logging.Info("Not adding empty tags.");
                return;
            }

            // Add all the tags
            foreach (PDFDocument pdf_document in pdf_documents)
            {
                pdf_document.AddTag(tags);
            }

            ObjTagEditorControl.TagsBundle = "";
        }

        private void CmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            AddTagsFromTextBox();
            Close();
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

            pdf_documents?.Clear();
            pdf_documents = null;

        }
    }
}
