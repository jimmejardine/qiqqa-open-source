using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using icons;

namespace Utilities.GUI.DualTabbedLayoutStuff
{
    /// <summary>
    /// Interaction logic for WindowControlsHeader.xaml
    /// </summary>
    public partial class WindowControlsHeader : UserControl
    {
        DualTabbedLayoutItem item;
        Window window;

        internal WindowControlsHeader(DualTabbedLayoutItem item, Window window)        
        {
            this.item = item;
            this.window = window;

            InitializeComponent();

            ImageMinimize.VerticalAlignment =
                ImageMaximize.VerticalAlignment =
                ImageClose.VerticalAlignment =
                VerticalAlignment.Center;

            ImageMinimize.Width = ImageMaximize.Width = ImageClose.Width = 24;

            ImageMinimize.Cursor = ImageMaximize.Cursor = ImageClose.Cursor = Cursors.Hand;

            ImageMinimize.Source = Icons.GetAppIcon(Icons.DualTabbed_Minimize);
            ImageMaximize.Source = Icons.GetAppIcon(Icons.DualTabbed_Maximize);
            ImageClose.Source = Icons.GetAppIcon(Icons.DualTabbed_Close);

            RenderOptions.SetBitmapScalingMode(this.ImageMinimize, BitmapScalingMode.HighQuality);
            RenderOptions.SetBitmapScalingMode(this.ImageMaximize, BitmapScalingMode.HighQuality);
            RenderOptions.SetBitmapScalingMode(this.ImageClose, BitmapScalingMode.HighQuality);

            ImageClose.Visibility = item.can_close ? Visibility.Visible : Visibility.Collapsed;

            ImageMinimize.ToolTip = "Minimize window";
            ImageMaximize.ToolTip = "Maximize window";
            ImageClose.ToolTip = "Close window";

            ImageMinimize.MouseDown += ImageMinimize_MouseDown;
            ImageMaximize.MouseDown += ImageMaximize_MouseDown;
            ImageClose.MouseDown += ImageClose_MouseDown;
        }

        void ImageMinimize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            window.WindowState = WindowState.Minimized;
        }

        void ImageMaximize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Normal;
            }
            else
            {
                window.WindowState = WindowState.Maximized;
            }            
        }

        void ImageClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            item.WantsClose();
        }
    }
}
