using System;
using System.Windows.Media;
using Qiqqa.Common.Configuration;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    public class TransparentBoxBrushPair
    {
        private Color color;
        private Color color_transparent;
        private Brush brush_border;
        private Brush brush_fill;

        public TransparentBoxBrushPair(Color color)
        {
            int TRANSPARENCY = (int)Math.Round(ConfigurationManager.Instance.ConfigurationRecord.GUI_HighlightScreenTransparency * 255);

            this.color = color;
            color_transparent = ColorTools.MakeTransparentColor(color, (byte)TRANSPARENCY);
            brush_border = new SolidColorBrush(color);
            brush_fill = new SolidColorBrush(color_transparent);
        }

        public Brush BorderBrush => brush_border;

        public Brush FillBrush => brush_fill;
    }
}
