using System.Windows;
using System.Windows.Controls;
using icons;
using Qiqqa.Documents.PDF.PDFControls.Page.Text;

namespace Qiqqa.Documents.PDF.PDFControls.CanvasToolbars
{
    /// <summary>
    /// Interaction logic for InkCanvasToolbarControl.xaml
    /// </summary>
    public partial class TextCanvasToolbarControl : UserControl
    {
        public TextCanvasToolbarControl()
        {
            InitializeComponent();

            ButtonSelectSentence.Icon = Icons.GetAppIcon(Icons.TextSentenceSelect);
            ButtonSelectBlock.Icon = Icons.GetAppIcon(Icons.TextSelect);

            ButtonSelectSentence.ToolTip = "Select text with column-wrapping sentences.";
            ButtonSelectBlock.ToolTip = "Select a block of text.  Use this for tabular data or if your PDF has bad text layout information.";

            ButtonSelectSentence.Click += ButtonSelectSentence_Click;
            ButtonSelectBlock.Click += ButtonSelectBlock_Click;
        }

        private PDFRendererControl pdf_renderer_control = null;
        public PDFRendererControl PDFRendererControl
        {
            get => pdf_renderer_control;
            set => pdf_renderer_control = value;
        }

        private void RaiseTextSelectModeChange(TextLayerSelectionMode textLayerSelectionMode)
        {
            pdf_renderer_control.RaiseTextSelectModeChange(textLayerSelectionMode);
        }

        private void ButtonSelectSentence_Click(object sender, RoutedEventArgs e)
        {
            RaiseTextSelectModeChange(TextLayerSelectionMode.Sentence);
        }

        private void ButtonSelectBlock_Click(object sender, RoutedEventArgs e)
        {
            RaiseTextSelectModeChange(TextLayerSelectionMode.Block);
        }
    }
}
