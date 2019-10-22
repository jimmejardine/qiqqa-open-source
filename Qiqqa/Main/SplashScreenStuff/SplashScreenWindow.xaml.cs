using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;
using Application = System.Windows.Forms.Application;

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
                try
                {
                    throw new ApplicationException($"Cannot locate the application splash image file '{image_filename}'. Looks like the Qiqqa installation is buggered. Please report this at https://github.com/jimmejardine/qiqqa-open-source/issues");
                }
                catch (Exception ex)
                {
                    AppDomain domain = AppDomain.CreateDomain("MyDomain", null);

                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(ex.Message + "\n -- ");
                    sb.AppendLine($"Designer: {Runtime.IsRunningInVisualStudioDesigner},");
                    sb.AppendLine($"S1:{System.Reflection.Assembly.GetExecutingAssembly().CodeBase},");
                    sb.AppendLine($"s2:{System.Reflection.Assembly.GetExecutingAssembly().Location},");
                    sb.AppendLine($"S3:{System.Windows.Forms.Application.StartupPath},");

                    // c# getassembly is a type, which is not valid in the given context 
                    TriState to = new TriState();
                    Type t = to.GetType();

                    sb.AppendLine($"S4:{System.Reflection.Assembly.GetAssembly(t).CodeBase},");
                    sb.AppendLine($"s5:{System.Reflection.Assembly.GetAssembly(t).Location},");

                    // Output to the console.
                    sb.AppendLine("Host domain: " + AppDomain.CurrentDomain.FriendlyName);
                    sb.AppendLine("New domain: " + domain.FriendlyName);
                    sb.AppendLine("Application base is: " + domain.BaseDirectory);
                    sb.AppendLine("Relative search path is: " + domain.RelativeSearchPath);
                    sb.AppendLine("Shadow copy files is set to: " + domain.ShadowCopyFiles);
                    AppDomain.Unload(domain);

                    sb.AppendLine(Utilities.Constants.QiqqaDevSolutionDir);
                    sb.AppendLine(Utilities.Constants.QiqqaDevProjectDir);
                    sb.AppendLine(Utilities.Constants.QiqqaDevTargetDir);
                    sb.AppendLine(Utilities.Constants.QiqqaDevBuild);

                    Logging.Error(sb.ToString());

                    throw new Exception(sb.ToString(), ex);
                }
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
