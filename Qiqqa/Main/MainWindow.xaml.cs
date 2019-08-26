using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using icons;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Main.IPC;
using Qiqqa.Main.LogoutStuff;
using Qiqqa.StartPage;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using Qiqqa.Common.BackgroundWorkerDaemonStuff;

namespace Qiqqa.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : StandardWindow
    {
        internal StartPageControl ObjStartPage = new StartPageControl();

        public static readonly string TITLE_START_PAGE = "Home (F1)";

        KeyboardHook keyboard_hook;
        IPCServer ipc_server;

        public MainWindow()
        {
            MainWindowServiceDispatcher.Instance.MainWindow = this;

            Theme.Initialize();

            InitializeComponent();

            this.DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;

            // Set a DEV title if necessary
            if (WebsiteAccess.IsTestEnvironment)
            {
                Title = "Qiqqa (TEST ENVIRONMENT)";
            }
            else
            {
                Title = "Qiqqa";
            }

            // Check that we actually are fitting on the user's screen
            if (this.Left > SystemParameters.VirtualScreenWidth || this.Width > SystemParameters.FullPrimaryScreenWidth)
            {
                this.Left = 0;
                this.Width = SystemParameters.FullPrimaryScreenWidth;
            }
            if (this.Top > SystemParameters.VirtualScreenHeight || this.Height > SystemParameters.FullPrimaryScreenHeight)
            {
                this.Top = 0;
                this.Height = SystemParameters.FullPrimaryScreenHeight;                
            }            

            DockingManager.WindowIcon = Icons.GetAppIconICO(Icons.Qiqqa);
            DockingManager.OwnerWindow = this;

            DockingManager.AddContent(TITLE_START_PAGE, TITLE_START_PAGE, Icons.GetAppIcon(Icons.ModuleStartPage), false, true, ObjStartPage);
            DockingManager.MakeActive(TITLE_START_PAGE);

            ObjStatusBar.Background = ThemeColours.Background_Brush_Blue_LightToDark;
            ObjTabBackground.Background = ThemeColours.Background_Brush_Blue_VeryDark;

            this.SizeChanged += MainWindow_SizeChanged;
            this.KeyUp += MainWindow_KeyUp;

            this.Closing += MainWindow_Closing;
            this.Closed += MainWindow_Closed;

            ObjTabWelcome.GetGoing += ObjTabWelcome_GetGoing;

            // Put this in a background thread
            Dispatcher.BeginInvoke(((Action)(() => PostStartupWork())), DispatcherPriority.Normal);
        }

        void keyboard_hook_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Z == e.KeyCode && KeyboardTools.IsWinDown())
            {
                Logging.Info("Qiqqa is being activated by WIN-Z");
                MainWindowServiceDispatcher.Instance.OpenPopupInCite();
                this.Activate();
                e.Handled = true;
            }
        }

        void PostStartupWork()
        {
            if (ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreWindowsAtStartup)
            {
                RestoreDesktopManager.RestoreDesktop();
            }
            else
            {
                // Open the most recently accessed web library
                List<WebLibraryDetail> web_libary_details = WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibrariesWithoutGuest;
                WebLibraryManager.Instance.SortWebLibraryDetailsByLastAccessed(web_libary_details);
                if (0 < web_libary_details.Count)
                {
                    MainWindowServiceDispatcher.Instance.OpenLibrary(web_libary_details[0].library);
                }

                // Also open guest under some circumstances
                    bool should_open_guest = false;

                        // No web libraries
                        if (0 == web_libary_details.Count)
                        {
                            should_open_guest = true;
                        }
                        // Web library is small compared to guest library
                        if (0 < web_libary_details.Count && WebLibraryManager.Instance.Library_Guest.PDFDocuments_IncludingDeleted_Count > 2 * web_libary_details[0].library.PDFDocuments_IncludingDeleted_Count)
                        {
                            should_open_guest = true;
                        }

                    if (should_open_guest)
                    {
                        MainWindowServiceDispatcher.Instance.OpenLibrary(WebLibraryManager.Instance.Library_Guest);
                    }

                // Make sure the start page is selected
                MainWindowServiceDispatcher.Instance.OpenStartPage();
            }

            if (ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreLocationAtStartup)
            {
                this.SetupConfiguredDimensions();
            }
            else
            {
                if (!RegistrySettings.Instance.IsSet(RegistrySettings.StartNotMaximized))
                {
                    this.WindowState = WindowState.Maximized;
                }
            }

            // Install the global keyboard and mouse hooks
            if (!RegistrySettings.Instance.IsSet(RegistrySettings.DisableGlobalKeyHook))
            {
                keyboard_hook = new KeyboardHook();
                keyboard_hook.KeyDown += keyboard_hook_KeyDown;
                keyboard_hook.Start();
            }
            else
            {
                Logging.Warn("DisableGlobalKeyHook is set!");
            }

            if (true)
            {
                // Start listening for other apps
                ipc_server = new IPCServer();
                ipc_server.IPCServerMessage += ipc_server_IPCServerMessage;

                ipc_server.Start();
            }

            if (ConfigurationManager.Instance.ConfigurationRecord.TermsAndConditionsAccepted)
            {
                StartBackgroundWorkerDaemon();
            }
        }

        void ipc_server_IPCServerMessage(string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindowServiceDispatcher.Instance.ProcessCommandLineFile(message);
            }
            ));
        }

        static bool already_exiting = false;
        public bool suppress_exit_warning = false;        
        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!already_exiting && !suppress_exit_warning)
            {
                if (ConfigurationManager.Instance.ConfigurationRecord.GUI_AskOnExit)
                {
                    LogoutWindow logout_window = new LogoutWindow();
                    if (false == logout_window.ShowDialog())
                    {
                        e.Cancel = true;
                        Logging.Info("User has requested not to quit Qiqqa on window close");
                        return;
                    }
                }
            }

            if (ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreWindowsAtStartup)
            {
                RestoreDesktopManager.SaveDesktop();
            }

            // Close all windows
            this.DockingManager.CloseAllContent();

            // If we get this far, they want out
            already_exiting = true;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            Logging.Info("+Explicitly shutting down application");

            ipc_server.Stop();
            
            FeatureTrackingManager.Instance.UseFeature(Features.App_Close);
            
            Application.Current.Shutdown();
            Logging.Info("-Explicitly shutting down application");
        }

        void MainWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!KeyboardTools.IsCTRLDown())
            {
                switch (e.Key)
                {
                    case Key.F1:
                        if (KeyboardTools.IsShiftDown())
                        {
                            MainWindowServiceDispatcher.Instance.OpenControlPanel();
                        }
                        else
                        {
                            MainWindowServiceDispatcher.Instance.OpenStartPage();
                        }
                        e.Handled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Logging.Info("Size is now {0}", e.NewSize.ToString());
        }

        private void StartBackgroundWorkerDaemon()
        {
            BackgroundWorkerDaemon d = BackgroundWorkerDaemon.Instance;
        }

        void ObjTabWelcome_GetGoing()
        {
            StartBackgroundWorkerDaemon();

            // Start the Wizard if necessary
            if (!ConfigurationManager.Instance.ConfigurationRecord.Wizard_HasSeenIntroWizard)
            {
                ConfigurationManager.Instance.ConfigurationRecord.Wizard_HasSeenIntroWizard = true;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(() => ConfigurationManager.Instance.ConfigurationRecord.Wizard_HasSeenIntroWizard);
                MainWindowServiceDispatcher.Instance.OpenWelcomeWizard();
            }
        }
    }
}
