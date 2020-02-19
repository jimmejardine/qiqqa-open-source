using System.Text;
using System.Windows.Media;

namespace QiqqaLegacyFileFormats          // namespace QiqqaLegacyFileFormats
{
    public class ColorTools
    {
        private static readonly char[] CHARS = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        public static string ColorToHEX(Color color)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('#');
            sb.Append(CHARS[(color.A >> 4) & 15]);
            sb.Append(CHARS[(color.A >> 0) & 15]);
            sb.Append(CHARS[(color.R >> 4) & 15]);
            sb.Append(CHARS[(color.R >> 0) & 15]);
            sb.Append(CHARS[(color.G >> 4) & 15]);
            sb.Append(CHARS[(color.G >> 0) & 15]);
            sb.Append(CHARS[(color.B >> 4) & 15]);
            sb.Append(CHARS[(color.B >> 0) & 15]);
            return sb.ToString();
        }

        public static Color HEXToColor(string hex)
        {
            Color color = (Color)ColorConverter.ConvertFromString(hex);
            return color;
        }
    }
}
