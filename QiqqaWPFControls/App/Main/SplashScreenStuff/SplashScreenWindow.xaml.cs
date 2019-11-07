using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Qiqqa.Main.SplashScreenStuff
{
    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow : Window
    {
        public static BitmapImage GetSplashImage()
        {
            string image_filename = Path.GetFullPath(Path.Combine(Common.Configuration.ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"Qiqqa.jpg"));
            if (!File.Exists(image_filename))
            {
                throw new ApplicationException($"Cannot locate the application splash image file '{image_filename}'. Looks like the Qiqqa installation is buggered. Please report this at https://github.com/jimmejardine/qiqqa-open-source/issues");
            }
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(File.ReadAllBytes(image_filename));
            bi.EndInit();

            return bi;
        }

        public SplashScreenWindow()
        {
            InitializeComponent();

            // Borders
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;

            // Resize
            Width = 748;
            Height = 446;

            // Load the image
            ObjImage.Source = SplashScreenWindow.GetSplashImage();

            // Format the text message
            TxtMessage.Background = new SolidColorBrush(Color.FromArgb(128, 64, 64, 64));
            TxtMessage.Foreground = Brushes.White;

            UpdateMessage("Welcome to Qiqqa!");
        }

        public void UpdateMessage(string message, params object[] args)
        {
            UpdateMessage(String.Format(message, args));
        }

        public void UpdateMessage(string message)
        {
            TxtMessage.Text = message;

            Utilities.GUI.WPFDoEvents.RepaintUIElement(TxtMessage);
        }
    }
}
