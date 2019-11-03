using System.Text;
using System.Windows.Media;
using Utilities.Random;

namespace Utilities.GUI
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

        public static System.Drawing.Color ConvertWindowsToDrawingColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Color ConvertDrawingToWindowsColor(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Color GetTransparentRainbowColour(int i, byte transparency)
        {
            return MakeTransparentColor(GetRainbowColour(i), transparency);
        }

        public static Color GetRainbowColour(int i)
        {
            switch (i % 7)
            {
                case 0:
                    return Colors.Red;
                case 1:
                    return Colors.Orange;
                case 2:
                    return Colors.GreenYellow;
                case 3:
                    return Colors.Green;
                case 4:
                    return Colors.Blue;
                case 5:
                    return Colors.Indigo;
                case 6:
                    return Colors.Violet;
                default:
                    return Colors.Black;   // ??????
            }
        }

        private static RandomAugmented random_aumented = new RandomAugmented();
        public static Color GetRandomColor()
        {
            byte[] buffer = new byte[3];
            random_aumented.NextBytes(buffer);

            return Color.FromRgb(buffer[0], buffer[1], buffer[2]);
        }

        public static Color MakeTransparentColor(Color base_color, byte transparency)
        {
            return Color.FromArgb(transparency, base_color.R, base_color.G, base_color.B);
        }

        public static Color MakeLighterColor(Color base_color)
        {
            return MakeLighterColor(base_color, 1.2);
        }

        public static Color MakeLighterColor(Color base_color, double luminosity)
        {
            HSLColor hsl = base_color;
            hsl.Luminosity *= luminosity;
            return hsl;
        }

        public static Color MakeDarkerColor(Color base_color)
        {
            return MakeDarkerColor(base_color, 1.2);

        }

        public static Color MakeDarkerColor(Color base_color, double luminosity)
        {
            HSLColor hsl = base_color;
            hsl.Luminosity /= luminosity;
            return hsl;
        }
    }
}
