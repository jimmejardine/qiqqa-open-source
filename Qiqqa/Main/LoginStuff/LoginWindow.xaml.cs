using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Ookii.Dialogs.Wpf;
using Qiqqa.Backups;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.UpgradePaths;
using Qiqqa.UtilisationTracking;
#if XULRUNNER_GECKO_ANTIQUE
using Qiqqa.WebBrowsing.GeckoStuff;
#endif
using Utilities;
using Utilities.GUI;
using Utilities.Misc;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

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
            //Theme.Initialize(); -- already done in StandardWindow base class

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
            ObjQiqqaDatabaseLocation.ToolTip = ConfigurationManager.Instance.BaseDirectoryForQiqqa;

            ButtonChangeBasePath.Click += ButtonChangeBasePath_Click;

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
            KeyDown += LoginWindow_KeyDown;
        }

        private void ButtonChangeBasePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();

            dialog.Description = "Change Qiqqa data Base Path";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.

            string default_folder = ConfigurationManager.Instance.BaseDirectoryForQiqqa;
            if (default_folder != null)
            {
                dialog.SelectedPath = default_folder;
            }

            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBoxes.Warn("Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            }

            if ((bool)dialog.ShowDialog())
            {
                ConfigurationManager.Instance.BaseDirectoryForQiqqa = dialog.SelectedPath;
                ObjQiqqaDatabaseLocation.Text = ConfigurationManager.Instance.BaseDirectoryForQiqqa;
                ObjQiqqaDatabaseLocation.ToolTip = ConfigurationManager.Instance.BaseDirectoryForQiqqa;

                Logging.Info("The user changed the Qiqqa Base directory to folder: {0}", dialog.SelectedPath);
            }
        }

        private void UpdateStatusMessage(string message)
        {
            ProgressInfoWrapper.Visibility = String.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
            ProgressInfo.Text = message;

            WPFDoEvents.RepaintUIElement(ProgressInfoWrapper);
        }

        private void StatusManager_OnStatusEntryUpdate(StatusManager.StatusEntry status_entry)
        {
            string msg = status_entry.LastStatusMessageWithProgressPercentage;

            WPFDoEvents.InvokeAsyncInUIThread(() => UpdateStatusMessage(msg));
        }

        private void LoginWindow_Closed(object sender, EventArgs e)
        {
            StatusManager.Instance.OnStatusEntryUpdate -= StatusManager_OnStatusEntryUpdate;
        }

        private void ButtonBackup_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.Instance.BaseDirectoryForQiqqaIsFixedFromNowOn = true;
            BackingUp.DoBackup();
        }

        private void ButtonRestore_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.Instance.BaseDirectoryForQiqqaIsFixedFromNowOn = true;
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

            ConfigurationManager.Instance.BaseDirectoryForQiqqaIsFixedFromNowOn = true;
            ConfigurationManager.Instance.ResetConfigurationRecord();

            // Create the base directory in case it doesn't exist
            Directory.CreateDirectory(ConfigurationManager.Instance.BaseDirectoryForUser);

            CloseToContinue();
        }

        private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Escape == e.Key || Key.Enter == e.Key || Key.Return == e.Key)
            {
                DoGuest();
                e.Handled = true;
            }
        }

        private void LoginWindow_Closing(object sender, CancelEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                is_closing = true;

                if (have_done_config)
                {
                    StartMainApplication();
                }
            });
        }

        private void StartMainApplication()
        {
            WPFDoEvents.AssertThisCodeIsRunningInTheUIThread();

            // prevent invocation loop via close() call at the end of this function body:
            if (StandardWindowFactory.Has(nameof(MainWindow)))
            {
                return;
            }

            WPFDoEvents.SetHourglassCursor();

            ConfigurationManager.Instance.BaseDirectoryForQiqqaIsFixedFromNowOn = true;

#if XULRUNNER_GECKO_ANTIQUE
            // Initialise the web browser
            try
            {
                StatusManager.Instance.UpdateStatus("AppStart", "Installing browser components");
                GeckoInstaller.CheckForInstall();
                StatusManager.Instance.UpdateStatus("AppStart", "Initialising browser components");
                //GeckoManager.Initialise();
                //GeckoManager.RegisterPDFInterceptor();
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Problem initialising GeckoFX.");
            }
#endif

            Logging.Info("Log the config+stats again now that we are sure to have loaded the working configuration:");
            ComputerStatistics.LogCommonStatistics(ConfigurationManager.GetCurrentConfigInfos());

            // Fire up Qiqqa!
            SafeThreadPool.QueueUserWorkItem(() =>
            {
                try
                {
                    // Perform any upgrade paths that we must
                    StatusManager.Instance.UpdateStatus("AppStart", "Upgrading old libraries");
                    UpgradeManager.RunUpgrades();

#if false
                    Thread.Sleep(15000);
#endif

                    StatusManager.Instance.UpdateStatus("AppStart", "Starting background processes");
                    WebLibraryManager.Instance.Kick();
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Problem while starting up the Qiqqa core.");
                }
            });

            StatusManager.Instance.UpdateStatus("AppStart", "Launching Qiqqa!");
            FireStartUseFeature();

            StandardWindowFactory.Create(nameof(MainWindow), () =>
            {
                MainWindow window = new MainWindow();

                window.Show();

                return window;
            });

            Hide();
            Close();
        }

        private void FireStartUseFeature()
        {
            FeatureTrackingManager.Instance.UseFeature(
                Features.App_Open,
                "CurrentVersion", ClientVersion.CurrentVersion,
                ".NETInfo", ComputerStatistics.GetCLRInfo()
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
