using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.MetadataControls
{
    /// <summary>
    /// Interaction logic for DocumentMetadataTagControl.xaml
    /// </summary>
    public partial class DocumentMetadataTagControl : StackPanel
    {
        private PDFDocument pdf_document;
        private string tag;

        public DocumentMetadataTagControl(PDFDocument pdf_document, string tag)
        {
            this.pdf_document = pdf_document;
            this.tag = tag;

            InitializeComponent();

            Background = new SolidColorBrush(ColorTools.MakeTransparentColor(Colors.Silver, 128));
            Margin = new Thickness(2, 2, 0, 0);
            MinWidth = 10;
            MinHeight = 10;

            TextTag.Text = tag;
            TextTag.VerticalAlignment = VerticalAlignment.Center;
            TextTag.Padding = new Thickness(4, 4, 4, 4);

            ImageClear.Source = Icons.GetAppIcon(Icons.Clear);
            ImageClear.IsHitTestVisible = true;
            ImageClear.MouseLeftButtonUp += ImageClear_MouseLeftButtonUp;
            ImageClear.Cursor = Cursors.Hand;
            ImageClear.VerticalAlignment = VerticalAlignment.Center;
            ImageClear.ToolTip = "Click here to remove this tag from the document.";
        }

        private void ImageClear_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Document_RemoveTag);

            pdf_document.RemoveTag(tag);
            e.Handled = true;
        }
    }
}
