using System;
using System.Windows.Media;
using Qiqqa.Common.Configuration;
using Utilities.GUI;

namespace Qiqqa.Documents.PDF.PDFControls.Page.Tools
{
    public class TransparentBoxBrushPair
    {
        Color color;
        Color color_transparent;
        Brush brush_border;
        Brush brush_fill;

        public TransparentBoxBrushPair(Color color)
        {
            int TRANSPARENCY = (int)Math.Round(ConfigurationManager.Instance.ConfigurationRecord.GUI_HighlightScreenTransparency * 255);
            
            this.color = color;
            this.color_transparent = ColorTools.MakeTransparentColor(color, (byte)TRANSPARENCY);
            this.brush_border = new SolidColorBrush(color);
            this.brush_fill = new SolidColorBrush(color_transparent);
        }

        public Brush BorderBrush
        {
            get
            {
                return brush_border;
            }
        }

        public Brush FillBrush
        {
            get
            {
                return brush_fill;
            }
        }
    }
}
