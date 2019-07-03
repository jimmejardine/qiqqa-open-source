using System.Collections.Generic;
using System.Windows.Media;

namespace Qiqqa.Common.Common
{
    public class StandardHighlightColours
    {
        private static readonly List<Color> colors = new List<Color>()
        {
            Colors.Yellow,
            Colors.LightPink,
            Colors.LightSalmon,
            Colors.LightGreen,
            Colors.SkyBlue
        };


        public static Color GetColor(int i)
        {            
            return colors[i % colors.Count];
        }

        public static System.Drawing.Color GetColor_Drawing(int i)
        {
            Color color = GetColor(i);

            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
