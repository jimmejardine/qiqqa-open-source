#if !HAS_NO_GUI

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using icons;

namespace Utilities.GUI
{
    [ContentProperty("ClientContent")]
    public class AugmentedInfoBarItemControl : DockPanel
    {
        private Image ObjIcon;
        private TextBlock ObjTextHeader;
        private ContentControl ObjClientControl;
        private AugmentedSpacer spacer1;
        private DockPanel dock_panel_header;
        private Brush dock_panel_header_background_brush;

        public AugmentedInfoBarItemControl() : base()
        {
            Theme.Initialize();

            spacer1 = new AugmentedSpacer();
            SetDock(spacer1, Dock.Bottom);
            Children.Add(spacer1);

            AugmentedBorder augmented_border = new AugmentedBorder();
            Children.Add(augmented_border);

            DockPanel dock_panel = new DockPanel();
            augmented_border.ClientContent = dock_panel;

            dock_panel_header = new DockPanel();
            dock_panel_header_background_brush = ThemeColours.Background_Brush_Blue_VeryDarkToDark;
            dock_panel_header.Background = dock_panel_header_background_brush;
            dock_panel_header.MouseEnter += dock_panel_header_MouseEnter;
            dock_panel_header.MouseLeave += dock_panel_header_MouseLeave;
            SetDock(dock_panel_header, Dock.Top);

            ObjIcon = new Image();
            ObjIcon.Source = Icons.GetAppIcon(Icons.Minus);
            ObjIcon.Height = 12;
            ObjIcon.Width = 12;
            ObjIcon.Margin = new Thickness(5, 0, 5, 0);
            ObjIcon.IsHitTestVisible = false;
            ObjIcon.SnapsToDevicePixels = true;
            SetDock(ObjIcon, Dock.Right);
            dock_panel_header.Children.Add(ObjIcon);

            ObjTextHeader = new TextBlock();
            ObjTextHeader.Padding = new Thickness(2, 2, 2, 4);
            dock_panel_header.Children.Add(ObjTextHeader);

            dock_panel.Children.Add(dock_panel_header);
            ObjTextHeader.Text = "UNTITLED";
            ObjTextHeader.FontSize = ThemeColours.HEADER_FONT_SIZE;
            ObjTextHeader.FontFamily = ThemeTextStyles.FontFamily_Header;
            ObjTextHeader.Cursor = Cursors.Hand;

            ObjClientControl = new ContentControl();
            dock_panel.Children.Add(ObjClientControl);

            dock_panel_header.MouseLeftButtonUp += ObjTextHeader_MouseLeftButtonUp;
        }

        public Dock CollapserDock
        {
            get => GetDock(ObjIcon);
            set => SetDock(ObjIcon, value);
        }

        private void dock_panel_header_MouseLeave(object sender, MouseEventArgs e)
        {
            DockPanel dock_panel_header = (DockPanel)sender;
            dock_panel_header.Background = dock_panel_header_background_brush;
        }

        private void dock_panel_header_MouseEnter(object sender, MouseEventArgs e)
        {
            DockPanel dock_panel_header = (DockPanel)sender;
            dock_panel_header.Background = ThemeColours.Background_Brush_Blue_LightToVeryLight;
        }

        private void ObjTextHeader_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ObjClientControl.Visibility == Visibility.Visible)
            {
                Collapse();
            }
            else
            {
                Expand();
            }

            e.Handled = true;
        }

        public void Collapse()
        {
            ObjClientControl.Visibility = Visibility.Collapsed;
            ObjIcon.Source = Icons.GetAppIcon(Icons.Plus);
        }

        public void Expand()
        {
            ObjClientControl.Visibility = Visibility.Visible;
            ObjIcon.Source = Icons.GetAppIcon(Icons.Minus);
        }

        public bool Collapsed
        {
            get => (ObjClientControl.Visibility == Visibility.Collapsed);
            set
            {
                if (value)
                {
                    Collapse();
                }
                else
                {
                    Expand();
                }
            }
        }

        public Visibility BottomSpacerVisibility
        {
            get => spacer1.Visibility;

            set => spacer1.Visibility = value;
        }

        [Bindable(true)]
        public string Header
        {
            get => ObjTextHeader.Text;
            set => ObjTextHeader.Text = value;
        }

        public Brush HeaderBackground
        {
            get => dock_panel_header_background_brush;
            set
            {
                dock_panel_header_background_brush = value;
                dock_panel_header.Background = dock_panel_header_background_brush;
            }
        }

        [Bindable(true)]
        public UIElement ClientContent
        {
            set => ObjClientControl.Content = value;
            get => (UIElement)ObjClientControl.Content;
        }
    }
}

#endif
