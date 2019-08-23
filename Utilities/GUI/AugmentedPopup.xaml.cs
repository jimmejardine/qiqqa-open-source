using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Utilities.GUI
{
    /// <summary>
    /// Interaction logic for AugmentedPopup.xaml
    /// </summary>
    public partial class AugmentedPopup : Popup
    {
        public AugmentedPopup(UIElement child)
        {
            Theme.Initialize();

            InitializeComponent();

            AllowsTransparency = true;
            PopupAnimation = PopupAnimation.Fade;
            StaysOpen = false;
            Placement = PlacementMode.MousePoint;

            ObjBorder.Background = ThemeColours.Background_Brush_Blue_VeryDarkToDark;

            Grid left_edge = new Grid();
            left_edge.HorizontalAlignment = HorizontalAlignment.Left;
            left_edge.Width = 20;
            left_edge.Background = ThemeColours.Background_Brush_Blue_VeryDarkToDark;

            DockPanel background = new DockPanel();
            background.Background = ThemeColours.Background_Brush_Blue_Light;

            DockPanel.SetDock(left_edge, Dock.Left);
            background.Children.Add(left_edge);
            background.Children.Add(child);

            ObjBorder.Child = background;
        }

        public AugmentedPopupAutoCloser AutoCloser
        {
            get
            {
                return new AugmentedPopupAutoCloser(this);
            }
        }

        public void Close()
        {
            this.IsOpen = false;
        }
    }
}
