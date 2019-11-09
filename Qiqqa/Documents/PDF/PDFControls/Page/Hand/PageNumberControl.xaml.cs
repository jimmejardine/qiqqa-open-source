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

            Opacity = 0.5;
            Background = ThemeColours.Background_Brush_Blue_LightToDark;
        }

        public void SetPageNumber(string value)
        {
            TxtPageNumber.Text = value;
        }
    }
}
