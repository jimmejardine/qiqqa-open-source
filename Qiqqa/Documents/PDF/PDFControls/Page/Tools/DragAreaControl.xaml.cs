using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    /// <summary>
    /// Interaction logic for DragAreaControl.xaml
    /// </summary>
    public partial class DragAreaControl : Canvas
    {
        public DragAreaControl(bool visible)
        {
            InitializeComponent();

            if (visible)
            {
                Color color = Colors.DarkBlue;

                Border.BorderThickness = new Thickness(1);
                Border.BorderBrush = new SolidColorBrush(ColorTools.MakeTransparentColor(color, 128));
                Background = new SolidColorBrush(ColorTools.MakeTransparentColor(color, 64));
            }

            SizeChanged += DragAreaControl_SizeChanged;
        }

        void DragAreaControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Border.Width = e.NewSize.Width;
            Border.Height = e.NewSize.Height;
        }
    }
}
