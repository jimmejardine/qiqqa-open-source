using System.Drawing;

namespace Utilities.GUI.Charting
{
	public class ChartTools
	{
		public static void renderNoDatasetMessage(Graphics g)
		{
            using (Font font = new Font(FontFamily.GenericSansSerif, 14.0f))
            {
                Brush brush = Brushes.Green;
                g.DrawString("No dataset to render!", font, brush, 10, 10);
            }
		}
	}
}
