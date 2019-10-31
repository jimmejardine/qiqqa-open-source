using System.Windows;
using System.Windows.Controls;

namespace Utilities.GUI
{
    public class AugmentedToolBarButton : AugmentedButton
    {
        public AugmentedToolBarButton() : base()
        {
            this.Background = null;
            this.BorderBrush = null;
            this.Style = FindResource(ToolBar.ButtonStyleKey) as Style;
        }
    }
}
