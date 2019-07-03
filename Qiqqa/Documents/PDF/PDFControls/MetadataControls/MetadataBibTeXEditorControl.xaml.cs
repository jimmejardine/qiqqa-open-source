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
    public partial class MetadataBibTeXEditorControl : StandardWindow
    {
        AugmentedBindable<PDFDocument> pdf_document_bindable;

        public MetadataBibTeXEditorControl()
        {
            InitializeComponent();

            this.Title = "BibTeX Editor";

            ButtonCancel.Icon = Icons.GetAppIcon(Icons.GoogleBibTexCancel);
            ButtonCancel.Caption = "Close";
            ButtonCancel.Click += ButtonCancel_Click;

            ButtonSniffer.Icon = Icons.GetAppIcon(Icons.BibTexSniffer);
            ButtonSniffer.Caption = "Sniffer";
            ButtonSniffer.Click += ButtonSniffer_Click;

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

            Keyboard.Focus(ObjBibTeXEditorControl);
        }

        void ButtonSniffer_Click(object sender, RoutedEventArgs e)
        {
            AugmentedBindable<PDFDocument> pdf_document_bindable = this.DataContext as AugmentedBindable<PDFDocument>;
            if (null == pdf_document_bindable)
            {
                return;
            }

            GoogleBibTexSnifferControl sniffer = new GoogleBibTexSnifferControl();
            sniffer.Show(pdf_document_bindable.Underlying);
        }

        void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Logging.Info("User cancelled the BibTeX editor");
            this.Close();
        }

        public static void TestHarness()
        {
            MetadataBibTeXEditorControl c = new MetadataBibTeXEditorControl();
            c.Show();
        }
    }
}
