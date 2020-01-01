using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utilities.GUI;
using Utilities.Misc;

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

            this.Closed += SplashScreenWindow_Closed;

            StatusManager.Instance.OnStatusEntryUpdate += StatusManager_OnStatusEntryUpdate;

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

            StatusManager.Instance.UpdateStatus("AppStart", "Welcome to Qiqqa!");
        }

        private void SplashScreenWindow_Closed(object sender, EventArgs e)
        {
            StatusManager.Instance.OnStatusEntryUpdate -= StatusManager_OnStatusEntryUpdate;
        }

        public void UpdateStatusMessage(string message)
        {
            TxtMessage.Text = message;

            Utilities.GUI.WPFDoEvents.RepaintUIElement(TxtMessage);
        }

        private void StatusManager_OnStatusEntryUpdate(StatusManager.StatusEntry status_entry)
        {
            string msg = status_entry.LastStatusMessage;

            if (status_entry.current_update_number < status_entry.total_update_count)
            {
                msg = String.Format("{0}: {1:P1}", msg, Utilities.Mathematics.Perunage.Calc(status_entry.current_update_number, status_entry.total_update_count));
            }

            WPFDoEvents.InvokeInUIThread(() => UpdateStatusMessage(msg));
        }
    }
}