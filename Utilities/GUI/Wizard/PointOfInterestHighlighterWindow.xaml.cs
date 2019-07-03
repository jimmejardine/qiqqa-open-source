using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;

namespace Utilities.GUI.Wizard
{
    /// <summary>
    /// Interaction logic for PointOfInterestHighlighterWindow.xaml
    /// </summary>
    public partial class PointOfInterestHighlighterWindow : Window
    {
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public PointOfInterestHighlighterWindow()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.BorderThickness = new Thickness(0);
            this.ResizeMode = ResizeMode.NoResize;
            this.Opacity = 0.1;
            this.IsHitTestVisible = false;
            this.AllowsTransparency = true;
            this.Topmost = true;

            DoubleAnimation animation = new DoubleAnimation(0.0, 0.3, new Duration(new TimeSpan(0, 0, 0, 0, 1000)));
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.AutoReverse = true;
            animation.AccelerationRatio = 0.1;
            this.BeginAnimation(OpacityProperty, animation);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Get this window's handle
            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // Change the extended window style to include WS_EX_TRANSPARENT
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
    }
}
