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
    public partial class MetadataBibTeXEditorControl : StandardWindow
    {
        private AugmentedBindable<PDFDocument> pdf_document_bindable;

        public MetadataBibTeXEditorControl()
        {
            InitializeComponent();

            Title = "BibTeX Editor";

            ButtonCancel.Icon = Icons.GetAppIcon(Icons.GoogleBibTexCancel);
            ButtonCancel.Caption = "Close";
            ButtonCancel.Click += ButtonCancel_Click;

            ButtonSniffer.Icon = Icons.GetAppIcon(Icons.BibTexSniffer);
            ButtonSniffer.Caption = "Sniffer";
            ButtonSniffer.Click += ButtonSniffer_Click;

            ButtonToggleBibTeX.Click += ButtonToggleBibTeX_Click;
            ButtonAckBibTeXParseErrors.Click += ButtonAckBibTeXParseErrors_Click;
            ButtonUndoBibTeXEdit.Click += ButtonUndoBibTeXEdit_Click;
            ObjBibTeXEditorControl.RegisterOverlayButtons(ButtonAckBibTeXParseErrors, ButtonToggleBibTeX, ButtonUndoBibTeXEdit);

            PreviewKeyDown += MetadataCommentEditorControl_PreviewKeyDown;
        }

        private void ButtonUndoBibTeXEdit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxes.Error("Sorry!\n\nMethod has not been implemented yet!");
        }

        private void ButtonAckBibTeXParseErrors_Click(object sender, RoutedEventArgs e)
        {
            ObjBibTeXEditorControl.ToggleBibTeXErrorView();
        }

        private void ButtonToggleBibTeX_Click(object sender, RoutedEventArgs e)
        {
            ObjBibTeXEditorControl.ToggleBibTeXMode(TriState.Arbitrary);
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

            Keyboard.Focus(ObjBibTeXEditorControl);
        }

        private void ButtonSniffer_Click(object sender, RoutedEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = DataContext as AugmentedBindable<PDFDocument>;
            if (null == pdf_document_bindable)
            {
                return;
            }

            GoogleBibTexSnifferControl sniffer = new GoogleBibTexSnifferControl();
            sniffer.Show(pdf_document_bindable.Underlying);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Logging.Info("User cancelled the BibTeX editor");
            Close();
        }

        #region --- Test ------------------------------------------------------------------------

#if TEST
        public static void TestHarness()
        {
            MetadataBibTeXEditorControl c = new MetadataBibTeXEditorControl();
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

            ObjBibTeXEditorControl.RegisterOverlayButtons(null, null, null);
        }
    }
}
