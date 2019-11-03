using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Utilities.GUI
{
    [ContentProperty("InnerChild")]
    public class AugmentedToolBarPopup : Popup
    {
        private AugmentedBorder border = new AugmentedBorder();
        private Grid grid = new Grid();

        public AugmentedToolBarPopup()
        {
            Theme.Initialize();

            AllowsTransparency = true;

            border.Background = ThemeColours.Background_Brush_Blue_LightToDark;
            border.Child = grid;
            Child = border;
        }

        public FrameworkElement InnerChild
        {
            get => (FrameworkElement)grid.Children[0];
            set
            {
                grid.Children.Clear();
                grid.Children.Add(value);
            }
        }

        public void Close()
        {
            IsOpen = false;
        }
    }
}
