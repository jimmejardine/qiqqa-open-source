using System;
using System.Drawing;

namespace Utilities
{
	public class Fonts
	{
		public static Font getLargestFont(Graphics g, String font_name, double max_size)
		{
			Font font = null;
			for (int i = 64; i > 1; --i)
			{
				font = new Font(font_name, i);
				SizeF font_size = g.MeasureString("M", font);
                if (font_size.Width < max_size && font_size.Height < max_size)
                {
                    break;
                }
                else
                {
                    font.Dispose();
                }
			}

			return font;
		}

		public static void renderTextInBlockCentre(Graphics g, String text, Font font, Brush brush, float left, float top, float width, float height)
		{
			SizeF font_size = g.MeasureString(text, font);
			float x = (float) (left+(width/2.0)-(font_size.Width/2.0));
			float y = (float) (top+(height/2.0)-(font_size.Height/2.0));
			g.DrawString(text, font, brush, x, y);
		}
	
	}
}
