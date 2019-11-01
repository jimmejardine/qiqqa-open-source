using System.Windows;
using System.Windows.Media;
using Utilities.GUI;

namespace Qiqqa.Brainstorm.Nodes
{
    internal class NodeThemes
    {
        public static CornerRadius corner_radius = new CornerRadius(5);
        public static Color control_color_base = Colors.LightGray;
        public static Color background_color = ColorTools.MakeTransparentColor(control_color_base, 192);
        public static Brush background_brush = new SolidColorBrush(background_color);

        public static double image_width = 48;
    }
}
