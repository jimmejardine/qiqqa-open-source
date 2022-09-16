using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Utilities.GUI
{
    public class AugmentedToolBar : ToolBar
    {
        public AugmentedToolBar()
        {
            Theme.Initialize();

            // This is a bit of a hack to get the toolbar dropdowns to be the right colour!!!  
            // I guess there must be a way to do it with styles - i dont know how.
            SizeChanged += AugmentedToolBar_SizeChanged;
        }

        private void AugmentedToolBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ToggleButton toggleButton = GetTemplateChild("OverflowButton") as ToggleButton;
            if (toggleButton != null)
            {
                toggleButton.Background = ThemeColours.Background_Brush_Blue_LightToDark;
            }
        }
    }
}
