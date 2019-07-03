using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Utilities.GUI
{
    [ContentProperty("InnerChild")]
    public class AugmentedToolBarPopup : Popup
    {        
        AugmentedBorder border = new AugmentedBorder();
        Grid grid = new Grid();

        public AugmentedToolBarPopup()
        {
            this.AllowsTransparency = true;
            
            this.border.Background = ThemeColours.Background_Brush_Blue_LightToDark;
            this.border.Child = grid;
            this.Child = this.border;
        }

        public FrameworkElement InnerChild
        {
            set
            {
                this.grid.Children.Clear();
                this.grid.Children.Add(value);
            }
        }

        public void Close()
        {
            this.IsOpen = false;
        }
    }
}
