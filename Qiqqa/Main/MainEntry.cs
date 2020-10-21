using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.Common.MessageBoxControls;
using Qiqqa.Main.IPC;
using Qiqqa.Main.LoginStuff;
using Qiqqa.UpgradePaths;
using Utilities;
using Utilities.Files;
using Utilities.GUI;
using Utilities.GUI.DualTabbedLayoutStuff;
using Utilities.Misc;
using Utilities.ProcessTools;
using Utilities.Shutdownable;
using Console = Utilities.GUI.Console;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


#if CEFSHARP
using CefSharp.Wpf;
using CefSharp;
#endif


namespace Qiqqa.Main
{
    public static class MainEntry
    {
        [DllImport("kernel32.dll")]
        private static extern int SetErrorMode(int newMode);

        private static Application application;

        static MainEntry()
        {
            try
            {
                DoPreamble();
                DoApplicationCreate();

                SafeThreadPool.QueueUserWorkItem(o =>
                {
                    DoUpgrades();
#if false 									// set to true for testing the UI behaviour wile this takes a long time to 'run':
                    Thread.Sleep(15000);
#endif
                    DoPostUpgrade();
                });
            }
            catch (Exception ex)
            {
                MessageBoxes.Error(ex, "There was an exception in the top-level static main entry!");
            }
        }

        private static void DoPreamble()
        {
            // Make sure the temp directory exists
            if (!TempDirectoryCreator.CheckTempExists())
            {
                MessageBoxes.Error(@"Qiqqa needs the directory {0} to exist for it to function properly.  Please create it or set the TEMP environment variable and restart Qiqqa.", TempFile.TempDirectoryForQiqqa);
            }

            // Make sure that windir is set (goddamned font subsystem bug: http://stackoverflow.com/questions/10094197/wpf-window-throws-typeinitializationexception-at-start-up)
            {
                if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("windir")))
                {
                    Logging.Warn("Environment variable windir is empty so setting it to {0}", Environment.GetEnvironmentVariable("SystemRoot"));
                    Environment.SetEnvironmentVariable("windir", Environment.GetEnvironmentVariable("SystemRoot"));
                }
            }

            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            Thread.CurrentThread.Name = "Main";

            if (RegistrySettings.Instance.IsSet(RegistrySettings.DebugConsole))
            {
                Console.Instance.Init();
                Logging.Info("Console initialised");
            }

            // Support windows-level error reporting - helps suppressing the errors in pdfdraw.exe and QiqqaOCR.exe
            // https://msdn.microsoft.com/en-us/library/windows/desktop/ms680621%28v=vs.85%29.aspx
            try
            {
                SetErrorMode(0x0001 | 0x0002 | 0x0004 | 0x8000);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Error trying to suppress global process failure.");
            }

            // kick the number of threads in the threadpool down to a reasonable number
            SafeThreadPool.SetMaxActiveThreadCount();

            AppDomain.CurrentDomain.AssemblyLoad += delegate (object sender, AssemblyLoadEventArgs args)
            {
                Logging.Info("Loaded assembly: {0}", args.LoadedAssembly.FullName);
                Logging.TriggerInit();
            };

#if CEFSHARP

            #region CEFsharp setup

            // CEFsharp setup for AnyPC as per https://github.com/cefsharp/CefSharp/issues/1714:
            AppDomain.CurrentDomain.AssemblyResolve += CefResolver;

            InitCef();

            #endregion CEFsharp setup

#endif

            try
            {
                FirstInstallWebLauncher.Check();
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Unknown exception during FirstInstallWebLauncher.Check().");
            }

            try
            {
                FileAssociationRegistration.DoRegistration();
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Unknown exception during FileAssociationRegistration.DoRegistration().");
            }

            // Start tracing WPF events
#if DEBUG
            WPFTrace wpf_trace = new WPFTrace();
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(wpf_trace);
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
            System.Diagnostics.Trace.AutoFlush = true;
#endif

            // If we have a command line parameter and another instance is running, send it to them and exit
            string[] command_line_args = Environment.GetCommandLineArgs();
            if (1 < command_line_args.Length && !ProcessSingleton.IsProcessUnique(false))
            {
                IPCClient.SendMessage(command_line_args[1]);
                Environment.Exit(-2);
            }

            // Check that we are the only instance running
            try
            {
                if (!RegistrySettings.Instance.IsSet(RegistrySettings.AllowMultipleQiqqaInstances) && !ProcessSingleton.IsProcessUnique(bring_other_process_to_front_if_it_exists: true))
                {
                    MessageBoxes.Info("There seems to be an instance of Qiqqa already running so Qiqqa will not start again.\n\nSometimes it takes a few moments for Qiqqa to exit as it finishes up a final OCR or download.  If this problem persists, you can kill the Qiqqa.exe process in Task Manager.");
                    Logging.Info("There is another instance of Qiqqa running, so exiting.");
                    Environment.Exit(-1);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Unknown exception while checking for single app instance.  Continuing as normal so there could be several Qiqqas running.");
            }

            ComputerStatistics.LogCommonStatistics();
        }

        private static void DoApplicationCreate()
        {
            // Create the application object
            application = new Application();
            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            application.Exit += Application_Exit;
            application.Activated += Application_Activated;
            application.LoadCompleted += Application_LoadCompleted;
            application.Startup += Application_Startup;

            // All the exception handling
            application.DispatcherUnhandledException += application_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            SafeThreadPool.UnhandledException += SafeThreadPool_UnhandledException;

            // Start the FPS measurer
            { var init = WPFFrameRate.Instance; }
        }

        private static void Application_Startup(object sender, StartupEventArgs e)
        {
            Logging.Info("---Application_Startup");
        }

        private static void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            Logging.Info("---Application_LoadCompleted");
        }

        private static void Application_Activated(object sender, EventArgs e)
        {
            Logging.Info("---Application_Activated");
        }

        private static void Application_Exit(object sender, ExitEventArgs e)
        {
            Logging.Info("---Application_Exit");
        }

        private static void DoUpgrades()
        {
            // Perform any upgrade paths that we must
            UpgradeManager.RunUpgrades();
        }

        private static void DoPostUpgrade()
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            // NB NB NB NB: You CANT USE ANYTHING IN THE USER CONFIG AT THIS POINT - it is not yet decided until LOGIN has completed...

            WPFDoEvents.InvokeInUIThread(() =>
            {
                StatusManager.Instance.UpdateStatus("AppStart", "Loading themes");
                Theme.Initialize();
                DualTabbedLayout.GetWindowOverride = delegate () { return new StandardWindow(); };

                // Force tooltips to stay open
                ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(3600000));
            });

            // Make sure the data directories exist...
            if (!Directory.Exists(ConfigurationManager.Instance.BaseDirectoryForUser))
            {
                Directory.CreateDirectory(ConfigurationManager.Instance.BaseDirectoryForUser);
            }

            // and kick off the Login Dialog to start the application proper:
            WPFDoEvents.InvokeAsyncInUIThread(() => ShowLoginDialog());

            // NB NB NB NB: You CANT USE ANYTHING IN THE USER CONFIG AT THIS POINT - it is not yet decided until LOGIN has completed...
        }

        public static void SignalShutdown()
        {
            ShutdownableManager.Instance.Shutdown();
        }

        public static void ShowLoginDialog()
        {
            LoginWindow login_window = new LoginWindow();
            login_window.ChooseLogin();
        }

        [STAThread]
        private static void Main()
        {
            Logging.Info("+static Main()");

            try
            {
                StatusManager.Instance.UpdateStatus("AppStart", "Logging in");

                // NOTE: the initial Login Dialog will be shown by code at the end
                // of the (background) DoPostUpgrade() process which is already running
                // by the time we arrive at this location.
                //
                // This ensures all process parts, which are expected to be done by
                // the time to login Dialog is visible (and usable by the user), are
                // indeed ready.

                try
                {
                    application.Run();
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Exception caught at Main() application.Run().  Disaster.");
                }

                SignalShutdown();
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Exception caught at Main().  Disaster.");
            }

            Logging.Info("-static Main()");

            // This must be the last line the application executes, EVAR!
            Logging.ShutDown();
        }

        private static void SafeThreadPool_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            RemarkOnException(e.ExceptionObject as Exception, false);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            RemarkOnException(e.ExceptionObject as Exception, true);
        }

        private static void application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            RemarkOnException(e.Exception, true);
            e.Handled = true;
        }

        private static void RemarkOnException(Exception ex, bool potentially_fatal)
        {
            Logging.Error(ex, "RemarkOnException.....");
            if (null != Application.Current)
            {
                WPFDoEvents.InvokeInUIThread(() =>
                {
                    RemarkOnException_GUI_THREAD(ex, potentially_fatal);
                }
                );
            }
        }

        private static void RemarkOnException_GUI_THREAD(Exception ex, bool potentially_fatal)
        {
            try
            {
                Logging.Error(ex, "RemarkOnException_GUI_THREAD...");
                UnhandledExceptionMessageBox.DisplayException(ex);
                const int EACCESS = unchecked((int)0x80004005);
                if (ex.Message.Contains("A generic error occurred in GDI+") || ex.HResult == EACCESS)
                {
                    potentially_fatal = false;
                }
            }
            catch (Exception ex2)
            {
                Logging.Error(ex2, "Exception thrown in top level error handler!!");
            }

            if (potentially_fatal)
            {
                // signal the application to shutdown as an unhandled exception is a grave issue and nothing will be guaranteed afterwards.
                Utilities.Shutdownable.ShutdownableManager.Instance.Shutdown();

                // and terminate the Windows Message Loop if it hasn't already (in my tests, Qiqqa was stuck in there without a window to receive messages from at this point...)
                MainWindowServiceDispatcher.Instance.ShutdownQiqqa(true);
            }
        }

#if CEFSHARP

        #region CEFsharp setup helpers

        // CEFsharp setup code as per https://github.com/cefsharp/CefSharp/issues/1714:

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitCef()
        {
            var settings = new CefSettings();

            // Set BrowserSubProcessPath based on app bitness at runtime
            settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                   Environment.Is64BitProcess ? "x64" : "x86",
                                                   "CefSharp.BrowserSubprocess.exe");

            // Make sure you set performDependencyCheck false
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

#if false
            var browser = new BrowserForm();
            Application.Run(browser);
#endif
        }

        // Will attempt to load missing assembly from either x86 or x64 subdir
        private static Assembly CefResolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }

        #endregion CEFsharp setup helpers

#endif

    }
}
