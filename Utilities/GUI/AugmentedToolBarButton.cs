#if !HAS_NO_GUI

using System.Windows;
using System.Windows.Controls;

namespace Utilities.GUI
{
    public class AugmentedToolBarButton : AugmentedButton
    {
        public AugmentedToolBarButton() : base()
        {
            Background = null;
            BorderBrush = null;
            Style = FindResource(ToolBar.ButtonStyleKey) as Style;
        }
    }
}

#endif
