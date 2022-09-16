using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using icons;
using Qiqqa.Common.GUI;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.Reflection;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for GoogleBibTexSnifferControl.xaml
    /// </summary>
    public partial class MetadataCommentEditorControl : StandardWindow
    {
        private AugmentedBindable<PDFDocument> pdf_document_bindable;

        public MetadataCommentEditorControl()
        {
            //Theme.Initialize(); -- already done in StandardWindow base class

            InitializeComponent();

            Title = "Qiqqa Metadata Comment Editor";

            ButtonCancel.Icon = Icons.GetAppIcon(Icons.GoogleBibTexCancel);
            ButtonCancel.Caption = "Close";
            ButtonCancel.Click += ButtonCancel_Click;

            PreviewKeyDown += MetadataCommentEditorControl_PreviewKeyDown;
        }

        private void MetadataCommentEditorControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                Close();
            }
        }

        public void Show(AugmentedBindable<PDFDocument> pdf_document_bindable)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_MetadataCommentEditor);

            Show();
            this.pdf_document_bindable = pdf_document_bindable;
            DataContext = pdf_document_bindable;

            Keyboard.Focus(TextComments);
            TextComments.ScrollToEnd();
            TextComments.SelectionStart = TextComments.Text.Length;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Logging.Info("User cancelled the GoogleBibTexSniffer");
            Close();
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

            // base.OnClosed() invokes this class' Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

            pdf_document_bindable = null;
        }
    }
}
