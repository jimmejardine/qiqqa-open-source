using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities.GUI;

namespace Qiqqa.Brainstorm.SceneManager
{
    /// <summary>
    /// Interaction logic for SelectingNodesControl.xaml
    /// </summary>
    public partial class SelectingNodesControl : UserControl
    {
        private static Color control_color_base = Colors.Gold;
        private static Color background_color = ColorTools.MakeTransparentColor(control_color_base, 64);
        private static Color border_color = ColorTools.MakeTransparentColor(control_color_base, 200);
        private static Brush background_brush = new SolidColorBrush(background_color);
        private static Brush border_brush = new SolidColorBrush(border_color);
        private static Thickness border_thickness = new Thickness(1);

        public SelectingNodesControl()
        {
            InitializeComponent();

            ObjBorder.BorderThickness = border_thickness;
            ObjBorder.BorderBrush = border_brush;
            ObjBorder.Background = background_brush;
        }
    }
}
