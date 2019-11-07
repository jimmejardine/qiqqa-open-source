using System.Windows.Controls;
using icons;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    /// <summary>
    /// Interaction logic for OCRNotAvailableControl.xaml
    /// </summary>
    public partial class OCRNotAvailableControl : Image
    {
        public OCRNotAvailableControl()
        {
            InitializeComponent();

            Source = Icons.GetAppIcon(Icons.OCRNotComplete);
            ToolTip = "Text recognition of this page is not yet available.  Please try again in a few minutes.";
            Width = 40;
            Height = 40;
            Canvas.SetLeft(this, 20);
            Canvas.SetTop(this, 20);
        }
    }
}
