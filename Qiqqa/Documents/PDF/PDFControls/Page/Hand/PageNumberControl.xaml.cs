using System.Windows.Controls;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Hand
{
    /// <summary>
    /// Interaction logic for PageNumberControl.xaml
    /// </summary>
    public partial class PageNumberControl : Border
    {
        public PageNumberControl()
        {
            Theme.Initialize();

            InitializeComponent();

            this.Opacity = 0.5;
            this.Background = ThemeColours.Background_Brush_Blue_LightToDark;
        }

        public string PageNumber
        {
            set
            {
                TxtPageNumber.Text = value;
            }
        }
    }
}
