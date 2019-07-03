using System.Windows;
using System.Windows.Controls;

namespace Utilities.GUI
{
    /// <summary>
    /// This Grid substiture always wants to be as small as possible.  It only has size when the parent container forces it to.
    /// Useful for surrounding images that would wish to resize behind other controls, but let those other controls determine the size.
    /// </summary>
    public class SpaceAvoidingGrid : Grid
    {
        private static readonly Size ZERO_SIZE = new Size(0, 0);
        
        protected override Size MeasureOverride(Size constraint)
        {
            return ZERO_SIZE;
        }
    }
}
