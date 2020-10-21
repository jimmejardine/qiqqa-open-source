using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Utilities.Misc;

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
                && (Application.Current?.Dispatcher.CheckAccess() ?? false)
                && (!Application.Current?.Dispatcher.HasShutdownStarted ?? false))
            {
                // https://stackoverflow.com/questions/4502037/where-is-the-application-doevents-in-wpf
                // https://stackoverflow.com/questions/21642381/how-to-wait-for-waithandle-while-serving-wpf-dispatcher-events
                //
                // Update (taken from last SO link and adapted to Qiqqa)
                //
                // So it seems the above method will peg the CPU.
                // Here's an alternative that uses a DispatcherTimer to check while the method pumps for messages.

#if false
                DispatcherFrame frame = new DispatcherFrame();

                // https://stackoverflow.com/questions/10448987/dispatcher-currentdispatcher-vs-application-current-dispatcher
                if (System.Windows.Threading.Dispatcher.CurrentDispatcher != Application.Current?.Dispatcher)
                {
                    Logging.Error(new Exception("Unexpected results"), "woops");
                }

                //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
#else
                DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background)
                {
                    Interval = TimeSpan.FromMilliseconds(50)
                };

                DispatcherFrame frame = new DispatcherFrame();

                timer.Tick += (o, e) =>
                {
                    timer.IsEnabled = false;
                    frame.Continue = false;
                };
                timer.IsEnabled = true;
                Dispatcher.PushFrame(frame);
#endif
            }
            else
            {
                Thread.Yield();
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

            Stopwatch clk = Stopwatch.StartNew();

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
                // to enter will wait for the UI/Dispatcher to relinquish control, while subsequent callers from
                // other background threads will wait on the lock to resolve...
                //lock (DoEvents_lock)
                {
                    if (!Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown
                        && (!Application.Current?.Dispatcher.HasShutdownStarted ?? false))
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
                        Thread.Yield();
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

        public static void InvokeInUIThread(Action action, Dispatcher override_dispatcher = null, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            try
            {
                if (override_dispatcher != null)
                {
                    if (!override_dispatcher.CheckAccess())
                    {
                        override_dispatcher.Invoke(action, priority);
                    }
                    else
                    {
                        action.Invoke();
                    }
                }
                else if (!CurrentThreadIsUIThread())
                {
                    Application.Current.Dispatcher.Invoke(action, priority);
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

        public static void InvokeAsyncInUIThread(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(action, priority);
            }
            else
            {
                throw new Exception("no known GUI thread to invoke async to...");
            }
        }

        [Conditional("DEBUG")]
        public static void AssertThisCodeIs_NOT_RunningInTheUIThread()
        {
            // This assertion check is important, but not severe enough to barf a hairball when it fails: dont_throw=true
            ASSERT.Test(!CurrentThreadIsUIThread(), "This code MUST NOT execute in the Main UI Thread.", dont_throw: true);
        }

        [Conditional("DEBUG")]
        public static void AssertThisCodeIsRunningInTheUIThread()
        {
            // This assertion check is important, but not severe enough to barf a hairball when it fails: dont_throw=true
            ASSERT.Test(CurrentThreadIsUIThread(), "This code MUST execute in the Main UI Thread.", dont_throw: true);
        }

        public static void SafeExec(Action f, Dispatcher override_dispatcher = null, bool must_exec_in_UI_thread = false)
        {
            if ((!must_exec_in_UI_thread && override_dispatcher == null) || CurrentThreadIsUIThread())
            {
                // exec in same thread:
                try
                {
                    f();
                }
                catch (Exception ex)
                {
                    // NOTE: when you set a debugger breakpoint here, it should only be hit
                    // AFTER the Logging singleton instance has shut down:
                    // it's okay when we're *that far* into the application termination phase.
                    if (!Logging.HasShutDown)
                    {
                        Logging.Error(ex, "Failed safe-exec in same thread.");
                    }
                }
            }
            else
            {
                string trace = LogAssist.AppendStackTrace(null, "SafeExec");

                WPFDoEvents.InvokeInUIThread(() =>
                {
                    try
                    {
                        f();
                    }
                    catch (Exception ex)
                    {
                        if (!Logging.HasShutDown)
                        {
                            Logging.Error(ex, "Failed safe-exec in UI thread.\n  Invoker call trace:\n{0}", trace);
                        }
                    }
                }, override_dispatcher);
            }
        }
    }
}
