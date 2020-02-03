using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Qiqqa.Backups;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Main.SplashScreenStuff;
using Qiqqa.UtilisationTracking;
using Qiqqa.WebBrowsing.GeckoStuff;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.Main.LoginStuff
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : StandardWindow
    {
        //class DisableSSLData
        //{
        //    public bool System_DisableSSL { get; set; }
        //}

        private bool is_closing = false;
        private bool have_done_config = false;

        //ProxySettingsControl.StandardProxySettings proxy_settings = new ProxySettingsControl.StandardProxySettings();
        //DisableSSLData disable_ssl_data = new DisableSSLData();

        public LoginWindow()
        {
            InitializeComponent();

            ProgressInfo.Text = "";
            ProgressInfoWrapper.Visibility = Visibility.Collapsed;
            StatusManager.Instance.OnStatusEntryUpdate += StatusManager_OnStatusEntryUpdate;

            Title = String.Format("Welcome to Qiqqa v{0}!", ClientVersion.CurrentBuild);
            if (WebsiteAccess.IsTestEnvironment)
            {
                Title = String.Format("Welcome to Qiqqa v{0}! (TEST ENVIRONMENT)", ClientVersion.CurrentBuild);
            }

            WindowStyle = WindowStyle.SingleBorderWindow;

            Brush BRUSH = new LinearGradientBrush(Colors.White, Color.FromRgb(230, 240, 255), 90);
            Background = BRUSH;

            ImageQiqqaLogo.Source = Icons.GetAppIcon(Icons.QiqqaLogoSmall);

            ObjQiqqaDatabaseLocation.Text = ConfigurationManager.Instance.BaseDirectoryForQiqqa;

            ButtonRestore.Icon = Icons.GetAppIcon(Icons.Backup);
            ButtonRestore.IconWidth = ButtonRestore.IconHeight = 64;
            ButtonRestore.Caption = "Restore";
            ButtonRestore.Click += ButtonRestore_Click;

            ButtonBackup.Icon = Icons.GetAppIcon(Icons.Backup);
            ButtonBackup.IconWidth = ButtonBackup.IconHeight = 64;
            ButtonBackup.Caption = "Backup";
            ButtonBackup.Click += ButtonBackup_Click;

            ButtonGuest.Click += ButtonGuest_Click;
            AugmentedButton.ApplyStyling(ButtonGuest);

            IsEnabled = true;

            Closing += LoginWindow_Closing;
            Closed += LoginWindow_Closed;
            this.ContentRendered += LoginWindow_ContentRendered;
            KeyDown += LoginWindow_KeyDown;
        }

        private void LoginWindow_ContentRendered(object sender, EventArgs e)
        {
            // Once the Login Dialog has been completely rendered on screen, do we remove the splash screen
            MainEntry.RemoveSplashScreen();
        }

        private void UpdateStatusMessage(string message)
        {
            ProgressInfoWrapper.Visibility = String.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
            ProgressInfo.Text = message;

            Utilities.GUI.WPFDoEvents.RepaintUIElement(ProgressInfoWrapper);
        }

        private void StatusManager_OnStatusEntryUpdate(StatusManager.StatusEntry status_entry)
        {
            string msg = status_entry.LastStatusMessageWithProgressPercentage;

            WPFDoEvents.InvokeInUIThread(() => UpdateStatusMessage(msg));
        }

        private void LoginWindow_Closed(object sender, EventArgs e)
        {
            StatusManager.Instance.OnStatusEntryUpdate -= StatusManager_OnStatusEntryUpdate;
        }

        private void ButtonBackup_Click(object sender, RoutedEventArgs e)
        {
            BackingUp.DoBackup();
        }

        private void ButtonRestore_Click(object sender, RoutedEventArgs e)
        {
            BackingUp.DoRestore();
        }

        public void ChooseLogin()
        {
            Show();
        }

        private void CloseToContinue()
        {
            have_done_config = true;

            if (!is_closing)
            {
                Close();
            }
        }

        private void ButtonGuest_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            DoGuest();
        }

        private void DoGuest()
        {
            IsEnabled = false;

            ConfigurationManager.Instance.ResetConfigurationRecordToGuest();
            CloseToContinue();
        }

        private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Escape == e.Key)
            {
                DoGuest();
                e.Handled = true;
            }
        }

        private void LoginWindow_Closing(object sender, CancelEventArgs e)
        {
            is_closing = true;

            if (!have_done_config)
            {
                DoGuest();
            }

            StartMainApplication();
        }

        private void StartMainApplication()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();
            WPFDoEvents.SetHourglassCursor();

            // Initialise the web browser
            try
            {
                StatusManager.Instance.UpdateStatus("AppStart", "Installing browser components");
                GeckoInstaller.CheckForInstall();
                StatusManager.Instance.UpdateStatus("AppStart", "Initialising browser components");
                GeckoManager.Initialise();
                GeckoManager.RegisterPDFInterceptor();
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Problem initialising GeckoFX.");
            }

            // Fire up Qiqqa!
            StatusManager.Instance.UpdateStatus("AppStart", "Starting background processes");
            SafeThreadPool.QueueUserWorkItem(o =>
            {
                StartDaemonSingletons();
            });

            StatusManager.Instance.UpdateStatus("AppStart", "Launching Qiqqa!");
            FireStartUseFeature();
            MainWindow window = new MainWindow();
            window.Show();

            Hide();
        }

        private void StartDaemonSingletons()
        {
            StatusManager.Instance.UpdateStatus("AppStart", "Starting libraries");
            WebLibraryManager.Init();
        }

        private void FireStartUseFeature()
        {
            FeatureTrackingManager.Instance.UseFeature(
                Features.App_Open,
                "CurrentVersion", ClientVersion.CurrentVersion,
                ".NET4Client", ComputerStatistics.IsNET4ClientInstalled(),
                ".NET4Full", ComputerStatistics.IsNET4FullInstalled()
                );
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // base.OnClosed() invokes this class' Closed() code, so we flipped the order of exec to reduce the number of surprises for yours truly.
            // This NULLing stuff is really the last rites of Dispose()-like so we stick it at the end here.

            //splashscreen_window = null;
        }
    }
}
