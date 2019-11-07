using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using icons;

namespace Utilities.GUI.DualTabbedLayoutStuff
{
    /// <summary>
    /// Interaction logic for ItemHeader.xaml
    /// </summary>
    public partial class ItemHeader : UserControl
    {
        private DualTabbedLayoutItem item;
        private AugmentedPopup popup;

        internal ItemHeader(DualTabbedLayoutItem item)
        {
            this.item = item;

            InitializeComponent();

            ImageIcon.VerticalAlignment =
                TextHeader.VerticalAlignment =
                ImageClose.VerticalAlignment =
                VerticalAlignment.Center;

            ImageIcon.Width = 20;
            ImageIcon.Source = item.icon;
            ImageIcon.Visibility = (null != item.icon) ? Visibility.Visible : Visibility.Collapsed;

            RenderOptions.SetBitmapScalingMode(ImageIcon, BitmapScalingMode.HighQuality);

            ImageClose.Width = 20;
            ImageClose.Source = Icons.GetAppIcon(Icons.DualTabbed_Close);
            ImageClose.Visibility = item.can_close ? Visibility.Visible : Visibility.Collapsed;
            ImageClose.ToolTip = "Close window";
            ImageClose.Cursor = Cursors.Hand;
            ImageClose.MouseLeftButtonUp += ImageClose_MouseLeftButtonUp;
            ImageClose.MouseEnter += ImageClose_MouseEnter;
            ImageClose.MouseLeave += ImageClose_MouseLeave;

            MouseRightButtonUp += ItemHeader_MouseRightButtonUp;

            TextHeader.Text = item.header;
            TextHeader.ToolTip = item.header; ;
            TextHeader.TextTrimming = TextTrimming.WordEllipsis;
            TextHeader.MaxWidth = 250;

            if (item.background_color.HasValue && Colors.Transparent != item.background_color.Value)
            {
                Background = new SolidColorBrush(ColorTools.MakeTransparentColor(item.background_color.Value, 64));
            }
        }

        private void ImageClose_MouseLeave(object sender, MouseEventArgs e)
        {
            ImageClose.Source = Icons.GetAppIcon(Icons.DualTabbed_Close);
        }

        private void ImageClose_MouseEnter(object sender, MouseEventArgs e)
        {
            ImageClose.Source = Icons.GetAppIcon(Icons.DualTabbed_CloseGlow);
        }

        private void ImageClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            item.WantsClose();
        }

        private void ItemHeader_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            AugmentedPopup popup = GetWindowLocationPopupMenu();
            popup.IsOpen = true;
        }

        public AugmentedPopup GetWindowLocationPopupMenu()
        {
            AugmentedButton ImageLeft = new AugmentedButton();
            AugmentedButton ImageBottom = new AugmentedButton();
            AugmentedButton ImageRight = new AugmentedButton();
            AugmentedButton ImageFloating = new AugmentedButton();
            AugmentedButton ImageClose = new AugmentedButton();

            ImageLeft.Icon = Icons.GetAppIcon(Icons.DualTabbed_TopLeft);
            ImageBottom.Icon = Icons.GetAppIcon(Icons.DualTabbed_Bottom);
            ImageRight.Icon = Icons.GetAppIcon(Icons.DualTabbed_TopRight);
            ImageFloating.Icon = Icons.GetAppIcon(Icons.DualTabbed_Floating);
            ImageClose.Icon = Icons.GetAppIcon(Icons.DualTabbed_Close);

            ImageLeft.Visibility = item.location == DualTabbedLayoutItem.Location.Left ? Visibility.Collapsed : Visibility.Visible;
            ImageBottom.Visibility = item.location == DualTabbedLayoutItem.Location.Bottom ? Visibility.Collapsed : Visibility.Visible;
            ImageRight.Visibility = item.location == DualTabbedLayoutItem.Location.Right ? Visibility.Collapsed : Visibility.Visible;
            ImageFloating.Visibility = item.can_floating ? (item.location == DualTabbedLayoutItem.Location.Floating ? Visibility.Collapsed : Visibility.Visible) : Visibility.Collapsed;
            ImageClose.Visibility = item.can_close ? Visibility.Visible : Visibility.Collapsed;

            ImageLeft.ToolTip = ImageLeft.Caption = "Move pane to top-left-hand group";
            ImageBottom.ToolTip = ImageBottom.Caption = "Move pane to bottom group";
            ImageRight.ToolTip = ImageRight.Caption = "Move pane to top-right-hand group";
            ImageFloating.ToolTip = ImageFloating.Caption = "Move pane to a floating window";
            ImageClose.ToolTip = ImageClose.Caption = "Close pane";

            ImageLeft.CaptionDock =
            ImageBottom.CaptionDock =
            ImageRight.CaptionDock =
            ImageFloating.CaptionDock =
            ImageClose.CaptionDock =
                Dock.Right;

            ImageLeft.Click += ImageLeft_MouseDown;
            ImageBottom.Click += ImageBottom_MouseDown;
            ImageRight.Click += ImageRight_MouseDown;
            ImageFloating.Click += ImageFloating_MouseDown;
            ImageClose.Click += ImageClose_Click;

            StackPanel popup_panel = new StackPanel();
            popup_panel.Children.Add(ImageLeft);
            popup_panel.Children.Add(ImageBottom);
            popup_panel.Children.Add(ImageRight);
            popup_panel.Children.Add(ImageFloating);
            popup_panel.Children.Add(ImageClose);

            popup = new AugmentedPopup(popup_panel);
            return popup;
        }

        private void ImageClose_Click(object sender, RoutedEventArgs e)
        {
            using (popup.AutoCloser)
            {
                item.WantsClose();
            }
        }

        private void ImageLeft_MouseDown(object sender, RoutedEventArgs e)
        {
            using (popup.AutoCloser)
            {
                item.WantsLeft();
            }
        }

        private void ImageBottom_MouseDown(object sender, RoutedEventArgs e)
        {
            using (popup.AutoCloser)
            {
                item.WantsBottom();
            }
        }

        private void ImageRight_MouseDown(object sender, RoutedEventArgs e)
        {
            using (popup.AutoCloser)
            {

                item.WantsRight();
            }
        }

        private void ImageFloating_MouseDown(object sender, RoutedEventArgs e)
        {
            using (popup.AutoCloser)
            {
                item.WantsFloating();
            }
        }

        public override string ToString()
        {
            return item.header;
        }
    }
}
