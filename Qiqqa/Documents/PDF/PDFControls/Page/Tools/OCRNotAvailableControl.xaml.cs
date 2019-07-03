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

            this.Source = Icons.GetAppIcon(Icons.OCRNotComplete);
            this.ToolTip = "Text recognition of this page is not yet available.  Please try again in a few minutes.";
            this.Width = 40;
            this.Height = 40;
            Canvas.SetLeft(this, 20);
            Canvas.SetTop(this, 20);
        }
    }
}
