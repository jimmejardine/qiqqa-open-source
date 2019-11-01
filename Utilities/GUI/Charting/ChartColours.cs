using System.Drawing;

namespace Utilities.GUI.Charting
{
    public class ChartColours
    {
        public static Color getOrderedColour(int i)
        {
            switch (i)
            {
                case 0:
                    return Color.Red;
                case 1:
                    return Color.Orange;
                case 2:
                    return Color.Blue;
                case 3:
                    return Color.Green;
                case 4:
                    return Color.Violet;
                case 5:
                    return Color.Indigo;
                case 6:
                    return Color.Yellow;
                case 7:
                    return Color.Brown;
                case 8:
                    return Color.Aquamarine;
                case 9:
                    return Color.Gold;
                case 10:
                    return Color.Salmon;
                case 11:
                    return Color.Turquoise;
                case 12:
                    return Color.Thistle;
                case 13:
                    return Color.Sienna;
                case 14:
                    return Color.Plum;
                case 15:
                    return Color.Moccasin;
                case 16:
                    return Color.LimeGreen;
                case 17:
                    return Color.Lavender;
                case 18:
                    return Color.Gainsboro;
                case 19:
                    return Color.Azure;
                case 20:
                    return Color.CadetBlue;
                case 21:
                    return Color.DarkRed;
                case 22:
                    return Color.Honeydew;
                case 23:
                    return Color.LemonChiffon;
                case 24:
                    return Color.LightSeaGreen;
                case 25:
                    return Color.Maroon;
                default:
                    {
                        return Color.Black;
                    }
            }
        }
    }
}
