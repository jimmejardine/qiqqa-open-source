using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using icons;
using Qiqqa.Common.GUI;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.Reflection;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for GoogleBibTexSnifferControl.xaml
    /// </summary>
    public partial class MetadataCommentEditorControl : StandardWindow
    {
        AugmentedBindable<PDFDocument> pdf_document_bindable;

        public MetadataCommentEditorControl()
        {
            InitializeComponent();

            this.Title = "Qiqqa Metadata Comment Editor";

            ButtonCancel.Icon = Icons.GetAppIcon(Icons.GoogleBibTexCancel);
            ButtonCancel.Caption = "Close";
            ButtonCancel.Click += ButtonCancel_Click;

            this.PreviewKeyDown += MetadataCommentEditorControl_PreviewKeyDown;
        }

        void MetadataCommentEditorControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }

        public void Show(AugmentedBindable<PDFDocument> pdf_document_bindable)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_MetadataCommentEditor);

            this.Show();
            this.pdf_document_bindable = pdf_document_bindable;
            this.DataContext = pdf_document_bindable;

            Keyboard.Focus(TextComments);
            TextComments.ScrollToEnd();
            TextComments.SelectionStart = TextComments.Text.Length;
        }

        void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Logging.Info("User cancelled the GoogleBibTexSniffer");
            this.Close();
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestHarness()
        {
            MetadataCommentEditorControl c = new MetadataCommentEditorControl();
            c.Show();
        }
#endif

        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}
