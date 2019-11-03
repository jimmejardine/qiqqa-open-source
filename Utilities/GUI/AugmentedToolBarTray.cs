using System.Windows;
using System.Windows.Controls;

namespace Utilities.GUI
{
    public class AugmentedToolBarTray : ScrollViewer
    {
        public AugmentedToolBarTray()
        {
            Theme.Initialize();

            Background = ThemeColours.Background_Brush_Blue_VeryDarkToDark;

            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            VerticalContentAlignment = VerticalAlignment.Top;
            //Margin = new System.Windows.Thickness(0, 0, 0, 0);
        }
    }
}
