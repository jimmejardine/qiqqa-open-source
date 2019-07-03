using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Utilities.GUI
{
    public class PixelOffsetTools
    {
        private static Point ZERO_POINT = new Point(0, 0);

        public static Point GetPixelSnapOffset(FrameworkElement fe)
        {
            if (null == PresentationSource.FromVisual(fe)) return ZERO_POINT;

            // Convert to
            Point fe_screen = fe.PointToScreen(ZERO_POINT);

            // Round the coordinates
            fe_screen.X = Math.Round(fe_screen.X);
            fe_screen.Y = Math.Round(fe_screen.Y);

            // Convert back
            Point fe_wpf = fe.PointFromScreen(fe_screen);

            // Should be close to zero...            
            return fe_wpf;
        }
    }
}
