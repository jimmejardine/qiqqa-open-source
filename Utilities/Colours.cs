using System;
using System.Drawing;
using Utilities.Random;

namespace Utilities
{
    public class Colours
    {
        public static int mapToColour(double d)
        {
            return (int)(255 * Math.Abs(d));
        }


        public static Color getRandomColour(RandomAugmented random)
        {
            double r = random.NextDouble();
            double g = random.NextDouble();
            double b = random.NextDouble();
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        public static Color getRandomBrilliantColour(RandomAugmented random)
        {
            double r;
            double g;
            double b;
            getRandomBrilliantColour(random, out r, out g, out b);
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        public static void getRandomBrilliantColour(RandomAugmented random, out double r, out double g, out double b)
        {
            r = g = b = 0.5;
            while
                (
                (isValueNearBottom(r) && isValueNearBottom(g) && isValueNearBottom(b)) ||
                (isValueNearMiddle(r) && isValueNearMiddle(g)) || (isValueNearMiddle(r) && isValueNearMiddle(b)) || (isValueNearMiddle(g) && isValueNearMiddle(b))
                )
            {
                r = random.NextDouble();
                g = random.NextDouble();
                b = random.NextDouble();
            }
        }

        public static bool isValueNearMiddle(double d)
        {
            return (d > 0.25 && d < 0.75);
        }

        public static bool isValueNearBottom(double d)
        {
            return (d < 0.5);
        }


        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color HSVToColor(double hue /*[0..360]*/ , double saturation /* [0..1]*/, double value /* [0..1]*/)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else if (hi == 5)
                return Color.FromArgb(255, v, p, q);
            else
                throw new Exception("ColorFromHSV?!!!");
        }
    }
}
