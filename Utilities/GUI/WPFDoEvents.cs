using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Utilities.GUI
{
    /// <summary>
    /// A nasty little class that does almost the equivalent of DoEvents in former lifetimes.  
    /// Try to avoid using it.  Obviously you will have a good reason to do so (e.g WPF printing blocks the GUI, grrrrrrrrrrrrrr!!!).
    /// 
    /// See also https://docs.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatcherframe
    /// </summary>
    public static class WPFDoEvents
    {
        private static void DoEvents()
        {
            if (!Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown
                && (Application.Current?.Dispatcher.CheckAccess() ?? false))
            {
                DispatcherFrame frame = new DispatcherFrame();
                // https://stackoverflow.com/questions/10448987/dispatcher-currentdispatcher-vs-application-current-dispatcher
                if (System.Windows.Threading.Dispatcher.CurrentDispatcher != Application.Current?.Dispatcher)
                {
                    Logging.Error(new Exception("Unexpected results"), "woops");
                }
                //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
            }
            else
            {
                Thread.Sleep(50);
            }
        }

        internal static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }

        //private static object DoEvents_lock = new object();

        public static void WaitForUIThreadActivityDone()
        {
            Logging.Debug("+WaitForUIThreadActivityDone start");

            Stopwatch clk = new Stopwatch();
            clk.Start();

            // if (Application.Current == null || Application.Current.Dispatcher.Thread == Thread.CurrentThread)
            // as per: https://stackoverflow.com/questions/5143599/detecting-whether-on-ui-thread-in-wpf-and-winforms#answer-14280425
            // and: https://stackoverflow.com/questions/2982498/wpf-dispatcher-the-calling-thread-cannot-access-this-object-because-a-differen/13726324#13726324
            if (Application.Current?.Dispatcher.CheckAccess() ?? false)
            {
                DoEvents();
            }
            else
            {
                // As we want the caller to WAIT until the UI has processed everything in the Windows Message Pipe,
                // i.e. kept the UI responsive, we will LOCK around this block too: the first (background) thread
                // to enter will wait for the UI/Dispatcher to relinquish control, while subseqeunt callers from
                // other background threads will wait on the lock to resolve...
                //lock (DoEvents_lock)
                {
                    if (!Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Logging.Debug(":::WaitForUIThreadActivityDone Invoke started");
                            DoEvents();
                            Logging.Debug(":::WaitForUIThreadActivityDone Invoke finished");
                        }));
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
            }
            Logging.Debug("-WaitForUIThreadActivityDone end (time spent: {0} ms)", clk.ElapsedMilliseconds);
        }

        #region Forced Repaint of UI 

        // as per: https://stackoverflow.com/questions/2886532/in-c-how-do-you-send-a-refresh-repaint-message-to-a-wpf-grid-or-canvas

        private static Action EmptyDelegate = delegate () { };

        public static void RepaintUIElement(this UIElement uiElement, Action repaint_done = null)
        {
            if (null == repaint_done)
            {
                repaint_done = EmptyDelegate;
            }
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, repaint_done);
        }

        #endregion

        public static void SetHourglassCursor()
        {
            // Set the cursor to Hourglass as per: http://www.csharp411.com/the-proper-way-to-show-the-wait-cursor/ 
            // --> https://stackoverflow.com/questions/11021422/how-do-i-display-wait-cursor-during-a-wpf-applications-startup
            InvokeInUIThread(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });
        }

        public static void ResetHourglassCursor()
        {
            // revert the forced hourglass cursor, if any:
            //
            // RESET the cursor to Hourglass as per: http://www.csharp411.com/the-proper-way-to-show-the-wait-cursor/ 
            // --> https://stackoverflow.com/questions/11021422/how-do-i-display-wait-cursor-during-a-wpf-applications-startup
            InvokeInUIThread(() =>
            {
                Mouse.OverrideCursor = null;
            });
        }

        public static bool CurrentThreadIsUIThread()
        {
            if (Application.Current?.Dispatcher.CheckAccess() ?? false)
            {
                // https://stackoverflow.com/questions/10448987/dispatcher-currentdispatcher-vs-application-current-dispatcher
                if (System.Windows.Threading.Dispatcher.CurrentDispatcher != Application.Current?.Dispatcher)
                {
                    Logging.Error(new Exception("Unexpected results"), "woops");
                    return false;
                }
                return true;
            }
            return Application.Current == null;
        }

        public static void InvokeInUIThread(Action action, Dispatcher override_dispatcher = null)
        {
            try
            {
                if (override_dispatcher != null)
                {
                    if (!override_dispatcher.CheckAccess())
                    {
                        override_dispatcher.Invoke(action, DispatcherPriority.Normal);
                    }
                    else
                    {
                        action.Invoke();
                    }
                }
                else if (!CurrentThreadIsUIThread())
                {
                    Application.Current.Dispatcher.Invoke(action, DispatcherPriority.Normal);
                }
                else
                {
                    action.Invoke();
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "InvokeInUIThread");
            }
        }

        public static void InvokeAsyncInUIThread(Action action)
        {
            if (!CurrentThreadIsUIThread())
            {
                Application.Current.Dispatcher.InvokeAsync(action, DispatcherPriority.Normal);
            }
            else
            {
                AsyncCallback cb = null;
                action.BeginInvoke(cb, null);
            }
        }

        public static void AssertThisCodeIs_NOT_RunningInTheUIThread()
        {
            if (CurrentThreadIsUIThread())
            {
                throw new ApplicationException("This code MUST NOT execute in the Main UI Thread. Report this issue at the GitHub Qiqqa project issue page https://github.com/jimmejardine/qiqqa-open-source/issues and please do provide the log file which contains this error report and accompanying stacktrace.");
            }
        }

        public static void AssertThisCodeIsRunningInTheUIThread()
        {
            if (!CurrentThreadIsUIThread())
            {
                throw new ApplicationException("This code MUST NOT execute in the Main UI Thread. Report this issue at the GitHub Qiqqa project issue page https://github.com/jimmejardine/qiqqa-open-source/issues and please do provide the log file which contains this error report and accompanying stacktrace.");
            }
        }
    }
}
