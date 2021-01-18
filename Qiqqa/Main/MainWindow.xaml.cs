using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using icons;
using Qiqqa.Common;
using Qiqqa.Common.BackgroundWorkerDaemonStuff;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Main.IPC;
using Qiqqa.Main.LogoutStuff;
using Qiqqa.StartPage;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.Maintainable;
using Utilities.Misc;
using Utilities.Shutdownable;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace Qiqqa.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : StandardWindow, IDisposable
    {
        internal StartPageControl ObjStartPage = new StartPageControl();

        public static readonly string TITLE_START_PAGE = "Home (F1)";
        private KeyboardHook keyboard_hook;
        private IPCServer ipc_server;
        private int HourglassState;

        public MainWindow()
        {
            MainWindowServiceDispatcher.Instance.MainWindow = this;

            Theme.Initialize();

            InitializeComponent();

            Application.Current.SessionEnding += Current_SessionEnding;
            Application.Current.Exit += Current_Exit;

            HourglassState = 2;
            WPFDoEvents.SetHourglassCursor();

            DataContext = ConfigurationManager.Instance.ConfigurationRecord_Bindable;

            // Set a DEV title if necessary
            Title = String.Format("Qiqqa v{0}", ClientVersion.CurrentBuild);
            if (WebsiteAccess.IsTestEnvironment)
            {
                Title = String.Format("Qiqqa v{0} (TEST ENVIRONMENT)", ClientVersion.CurrentBuild);
            }

            // Check that we actually are fitting on the user's screen
            if (Left > SystemParameters.VirtualScreenWidth || Width > SystemParameters.FullPrimaryScreenWidth)
            {
                Left = 0;
                Width = SystemParameters.FullPrimaryScreenWidth;
            }
            if (Top > SystemParameters.VirtualScreenHeight || Height > SystemParameters.FullPrimaryScreenHeight)
            {
                Top = 0;
                Height = SystemParameters.FullPrimaryScreenHeight;
            }

            DockingManager.WindowIcon = Icons.GetAppIconICO(Icons.Qiqqa);
            DockingManager.OwnerWindow = this;

            DockingManager.AddContent(TITLE_START_PAGE, TITLE_START_PAGE, Icons.GetAppIcon(Icons.ModuleStartPage), false, true, ObjStartPage);
            DockingManager.MakeActive(TITLE_START_PAGE);

            ObjStatusBar.Background = ThemeColours.Background_Brush_Blue_LightToDark;
            ObjTabBackground.Background = ThemeColours.Background_Brush_Blue_VeryDark;

            SizeChanged += MainWindow_SizeChanged;
            KeyUp += MainWindow_KeyUp;

            Unloaded += MainWindow_Unloaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
            Closing += MainWindow_Closing;
            Closed += MainWindow_Closed;

            // We've looked for the LAST event that triggers dependably at the start of the application:
            //   ContentRendered
            // is the last one triggered of this bunch:
            //
            //this.Activated += MainWindow_Activated;
            ContentRendered += MainWindow_ContentRendered;
            //this.Initialized += MainWindow_Initialized;
            //this.LayoutUpdated += MainWindow_LayoutUpdated;
            //this.Loaded += MainWindow_Loaded;
            //this.ManipulationCompleted += MainWindow_ManipulationCompleted;
            //this.ManipulationStarting += MainWindow_ManipulationStarting;
            //this.SourceInitialized += MainWindow_SourceInitialized;
            //this.StateChanged += MainWindow_StateChanged;

            WebLibraryManager.Instance.WebLibrariesChanged += Instance_WebLibrariesChanged;
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Logging.Info("x");

            CleanUp();
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            Logging.Info("x");

            CleanUp();
        }

        private void Current_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            Logging.Info("x");

            CleanUp();
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Logging.Info("x");

            CleanUp();
        }

        private void Instance_WebLibrariesChanged()
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            if (ShutdownableManager.Instance.IsShuttingDown)
            {
                Logging.Info("WebLibrariesChanged: Breaking out of UI update due to application termination");
                return;
            }

            // NOTE:
            // the code in PostStartupWork() depends on the completed loading of the libraries,
            // hence it is executed only now instead of earlier in MainWindow()
            PostStartupWork();

            // https://stackoverflow.com/questions/34340134/how-to-know-when-a-frameworkelement-has-been-totally-rendered
            ResetHourglassWhenAllIsDone();
        }

        private void ResetHourglassWhenAllIsDone()
        {
            WPFDoEvents.InvokeInUIThread(() => {
                HourglassState--;

                if (HourglassState == 0)
                {
                    WPFDoEvents.ResetHourglassCursor();

#if PROFILE_STARTUP_PHASE
                    Environment.Exit(-2);    // testing & profiling only
#endif
                }
            }, priority: DispatcherPriority.ContextIdle);
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            Logging.Debug特("MainWindow::ContentRendered event");

            // hold off: level 2 -> 1
            MaintainableManager.Instance.BumpHoldOffPendingLevel();

            // https://stackoverflow.com/questions/34340134/how-to-know-when-a-frameworkelement-has-been-totally-rendered
            ResetHourglassWhenAllIsDone();
        }

        private void keyboard_hook_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Z == e.KeyCode && KeyboardTools.IsWinDown())
            {
                Logging.Info("Qiqqa is being activated by WIN-Z");
                MainWindowServiceDispatcher.Instance.OpenPopupInCite();
                Activate();
                e.Handled = true;
            }
        }

        private void PostStartupWork()
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            if (ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreWindowsAtStartup)
            {
                RestoreDesktopManager.RestoreDesktop();
            }
            else
            {
                // Open the most recently accessed web library
                List<WebLibraryDetail> web_libary_details = WebLibraryManager.Instance.WebLibraryDetails_WorkingWebLibraries;
                WebLibraryManager.Instance.SortWebLibraryDetailsByLastAccessed(web_libary_details);

                // Also open guest under some circumstances
                bool should_open_guest = false;

                ASSERT.Test(web_libary_details.Count > 0);

                // Web library is small compared to guest library
                if (WebLibraryManager.Instance.Library_Guest.Xlibrary.PDFDocuments_IncludingDeleted_Count > 2 * web_libary_details[0].Xlibrary.PDFDocuments_IncludingDeleted_Count)
                {
                    should_open_guest = true;
                }

                WPFDoEvents.InvokeInUIThread(() => MainWindowServiceDispatcher.Instance.OpenLibrary(web_libary_details[0]));

                // don't open the guest library *twice* so check against `web_libary_details[0].library`
                if (should_open_guest && web_libary_details[0] != WebLibraryManager.Instance.Library_Guest)
                {
                    WPFDoEvents.InvokeInUIThread(() => MainWindowServiceDispatcher.Instance.OpenLibrary(WebLibraryManager.Instance.Library_Guest));
                }

                // Make sure the start page is selected
                WPFDoEvents.InvokeInUIThread(() => MainWindowServiceDispatcher.Instance.OpenStartPage());
            }

            WPFDoEvents.InvokeInUIThread(() =>
            {
                if (ConfigurationManager.Instance.ConfigurationRecord.GUI_RestoreLocationAtStartup)
                {
                    SetupConfiguredDimensions();
                }
                else
                {
                    if (!RegistrySettings.Instance.IsSet(RegistrySettings.StartNotMaximized))
                    {
                        WindowState = WindowState.Maximized;
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
            });

            {
                // Start listening for other apps
                ipc_server = new IPCServer();
                ipc_server.IPCServerMessage += ipc_server_IPCServerMessage;

                ipc_server.Start();
            }

            StartBackgroundWorkerDaemon();
        }

        private void ipc_server_IPCServerMessage(string message)
        {
            WPFDoEvents.InvokeAsyncInUIThread(() =>
            {
                MainWindowServiceDispatcher.Instance.ProcessCommandLineFile(message);
            }
            );
        }

        private static bool already_exiting = false;
        public bool suppress_exit_warning = false;

        private void MainWindow_Closing(object sender, CancelEventArgs e)
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
            DockingManager.CloseAllContent();

            MainEntry.SignalShutdown("Main window CLOSING event: user explicitly shutting down application.");

            CleanUp();
        }

        private void CleanUp()
        {
            ipc_server?.Stop();
            ipc_server = null;

            FeatureTrackingManager.Instance.UseFeature(Features.App_Close);

            Application.Current.Shutdown();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Logging.Info("+Explicitly shutting down application");

            MainEntry.SignalShutdown("Main window CLOSED event: explicitly shutting down application.");

            CleanUp();

            Logging.Info("-Explicitly shutting down application");
        }

        private void MainWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Logging.Debug("MainWindow_SizeChanged :: Size is now {0}", e.NewSize.ToString());
        }

        private void StartBackgroundWorkerDaemon()
        {
            BackgroundWorkerDaemon d = BackgroundWorkerDaemon.Instance;
        }

        private void ObjTabWelcome_GetGoing()
        {
            StartBackgroundWorkerDaemon();

            // Start the Wizard if necessary
            if (!ConfigurationManager.Instance.ConfigurationRecord.Wizard_HasSeenIntroWizard)
            {
                ConfigurationManager.Instance.ConfigurationRecord.Wizard_HasSeenIntroWizard = true;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.Wizard_HasSeenIntroWizard));
                MainWindowServiceDispatcher.Instance.OpenWelcomeWizard();
            }
        }

#region --- IDisposable ------------------------------------------------------------------------

        ~MainWindow()
        {
            Logging.Debug("~MainWindow()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing MainWindow");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("MainWindow::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
                    if (dispose_count == 0)
                    {
                        ipc_server?.Stop();
                    }
                });

                WPFDoEvents.SafeExec(() =>
                {
                    ObjStartPage = null;

                    keyboard_hook = null;
                    ipc_server = null;
                });

                WPFDoEvents.SafeExec(() =>
                {
                    DataContext = null;
                });

                ++dispose_count;
            });
        }

#endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Dispose() does a few things that are also done in MainWindow_Closed(), which is invoked via base.OnClosed(),
            // so we flipped the order of exec to reduce the number of surprises for yours truly.
            Dispose();
        }
    }
}
