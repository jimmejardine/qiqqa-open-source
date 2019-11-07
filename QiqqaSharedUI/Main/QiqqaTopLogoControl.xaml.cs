using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using icons;
using Qiqqa.Common;

namespace Qiqqa.Main
{
    /// <summary>
    /// Interaction logic for QiqqaTopLogoControl.xaml
    /// </summary>
    public partial class QiqqaTopLogoControl : UserControl
    {
        public QiqqaTopLogoControl()
        {
            InitializeComponent();

            ImageQiqqaLogo.Source = Icons.GetAppIcon(Icons.Qiqqa);
            ImageQiqqaLogo.Stretch = Stretch.Uniform;
            ImageQiqqaLogo.ToolTip = "Go to Qiqqa.com";

            RenderOptions.SetBitmapScalingMode(ImageQiqqaLogo, BitmapScalingMode.HighQuality);

            ImageQiqqaLogo.MouseEnter += ImageQiqqaLogo_MouseEnter;
            ImageQiqqaLogo.MouseLeave += ImageQiqqaLogo_MouseLeave;
            ImageQiqqaLogo.MouseUp += ImageQiqqaLogo_MouseUp;

            Opacity = 0.5;
        }

        private void ImageQiqqaLogo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainWindowServiceDispatcher.Instance.OpenQiqqaWebsite();
            e.Handled = true;
        }

        private void ImageQiqqaLogo_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0.5, new Duration(TimeSpan.FromMilliseconds(500)));
            BeginAnimation(OpacityProperty, animation);
        }

        private void ImageQiqqaLogo_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(500)));
            BeginAnimation(OpacityProperty, animation);
        }
    }
}
