using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utilities.GUI.Brainstorm.SceneManager
{
    /// <summary>
    /// Interaction logic for SelectingNodesControl.xaml
    /// </summary>
    public partial class SelectingNodesControl : UserControl
    {
        static Color control_color_base = Colors.Gold;
        static Color background_color = ColorTools.MakeTransparentColor(control_color_base, 64);
        static Color border_color = ColorTools.MakeTransparentColor(control_color_base, 200);

        static Brush background_brush = new SolidColorBrush(background_color);
        static Brush border_brush = new SolidColorBrush(border_color);
        static Thickness border_thickness = new Thickness(1);

        public SelectingNodesControl()
        {
            InitializeComponent();

            ObjBorder.BorderThickness = border_thickness;
            ObjBorder.BorderBrush = border_brush;
            ObjBorder.Background = background_brush;
        }
    }
}
