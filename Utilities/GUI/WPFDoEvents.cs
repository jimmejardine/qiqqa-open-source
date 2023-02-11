using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Utilities.Misc;
using Utilities.Shutdownable;

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
#if false
        private static void DoEvents()
        {
            if (!ShutdownableManager.Instance.IsShuttingDown
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
#endif

#if false
        internal static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }
#endif

#if false
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
                    if (!ShutdownableManager.Instance.IsShuttingDown
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
#endif

        // Forced Repaint of UI

        // as per: https://stackoverflow.com/questions/2886532/in-c-how-do-you-send-a-refresh-repaint-message-to-a-wpf-grid-or-canvas

        private static Action EmptyDelegate = delegate () { };

        public static void RepaintUIElement(this UIElement uiElement, Action repaint_done = null)
        {
            if (null == repaint_done)
            {
                repaint_done = EmptyDelegate;
            }
            else
            {
                Action wrapper_action = () =>
                {
                    WPFDoEvents.SafeExec(repaint_done, $"RepaintUIElement::{uiElement}");
                };
                repaint_done = wrapper_action;
            }
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, repaint_done);
        }


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
            Thread t = Thread.CurrentThread;
            ApartmentState state = t?.GetApartmentState() ?? ApartmentState.Unknown;
            bool pooled = t?.IsThreadPoolThread ?? false;
            bool bg = t?.IsBackground ?? false;
            string n = t?.Name ?? "???";
            bool nct = (Application.Current == null);
            bool isMainDispatcher = (System.Windows.Threading.Dispatcher.CurrentDispatcher == Application.Current?.Dispatcher);
            bool isUI = (!pooled && !bg && state == ApartmentState.STA);
            bool acc = Application.Current?.Dispatcher.CheckAccess() ?? false;

            if (state == ApartmentState.Unknown || (nct && !ShutdownableManager.Instance.IsShuttingDown) || (!isMainDispatcher && isUI) || acc != isUI)
            {
                // when we at the end of application lifetime, after signaling shutdown, at some point this mix shows up:
                //     [Q] WARN[Main][198.658M] Running in odd context @ STA / False / False / True / ..... / True
                //
                //            STA(state)/False(pooled)/False(bg)/True(appl.curr)/True(isShuttingdown)/False(isMainDisp)/True(isUI)/True(accNotUI)/True(mix)
#if false
                Logging.Warn($"Running in odd context @ {state}/{pooled}/{bg}/{ (Application.Current == null) }/{ ShutdownableManager.Instance.IsShuttingDown }/{ isMainDispatcher }/{ isUI }/{ (acc != isUI) }/{ !(pooled || bg || state != ApartmentState.STA) }");
#endif
            }

            if (acc)
            {
                // https://stackoverflow.com/questions/10448987/dispatcher-currentdispatcher-vs-application-current-dispatcher
                if (!isMainDispatcher || !isUI)
                {
                    Logging.Error(new Exception("Unexpected results"), $"woops @ {state}/{pooled}/{bg}/{ (Application.Current == null) }/{ ShutdownableManager.Instance.IsShuttingDown }/{ isMainDispatcher }/{ isUI }/{ (acc != isUI) }/{ !(pooled || bg || state != ApartmentState.STA) }");
                    return false;
                }
                return true;
            }
            return isUI;
        }

        public static void InvokeInUIThread(Action action, Dispatcher override_dispatcher = null, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            try
            {
                if (override_dispatcher != null)
                {
                    if (!override_dispatcher.CheckAccess())
                    {
                        Action wrapper_action = () =>
                        {
                            WPFDoEvents.SafeExec(action, "InvokeInUIThread::override_dispatcher");
                        };
                        override_dispatcher.Invoke(wrapper_action, priority);
                    }
                    else
                    {
                        action();
                    }
                }
                else if (!CurrentThreadIsUIThread())
                {
                    if (Application.Current != null)
                    {
                        Action wrapper_action = () =>
                        {
                            WPFDoEvents.SafeExec(action, "InvokeInUIThread");
                        };
                        Application.Current.Dispatcher.Invoke(wrapper_action, priority);
                    }
                    else
                    {
                        // Pray to the Big Kahuna; we're (probably) shutting down and don't know / cannot know any more if we're in UI thread or other.
                        //
                        // Fire off and pray...
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex, "InvokeInUIThread::finalFallback: Error occurred while there's no current known UI thread.");
                        }
                    }
                }
                else
                {
                    action();
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
                // wrap invoked code in exception catcher/handler so we don't get spurious unhandled exceptions in the UI.
                Action wrapper_action = () =>
                {
                    WPFDoEvents.SafeExec(action, "InvokeAsyncInUIThread");
                };
                Application.Current.Dispatcher.BeginInvoke(wrapper_action, priority);
            }
            else
            {
                ASSERT.Test(ShutdownableManager.Instance.IsShuttingDown);
                try
                {
                    throw new Exception("Ignoring async UI invocation during shutdown.");
                }
                catch (Exception ex)
                {
                    Logging.Warn(ex);
                }
            }
        }

        [Conditional("DEBUG")]
        public static void AssertThisCodeIs_NOT_RunningInTheUIThread()
        {
            // This assertion check is important, but not severe enough to barf a hairball when it fails: dont_throw=true
            // Besides, the basic test would fail when we are shutting down the application.
            bool state = !CurrentThreadIsUIThread() || ShutdownableManager.Instance.IsShuttingDown;
            if (!state)
            {
                ASSERT.Test(state, "This code MUST NOT execute in the Main UI Thread.", dont_throw: true);
            }
        }

        [Conditional("DEBUG")]
        public static void AssertThisCodeIsRunningInTheUIThread()
        {
            // This assertion check is important, but not severe enough to barf a hairball when it fails: dont_throw=true
            bool state = CurrentThreadIsUIThread() || ShutdownableManager.Instance.IsShuttingDown;
            if (!state)
            {
                ASSERT.Test(false, "This code MUST execute in the Main UI Thread.", dont_throw: true);
            }
        }

        public static string StackTrace
        {
            get
            {
                return LogAssist.AppendStackTrace(null, "SafeExec");
            }
        }

        public static void SafeExec(Action f, string trace = null, [CallerMemberName] string callerName = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = 0)
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
                    var func_call_str = GetInvokingFunctionName(callerName, sourceFilePath, sourceLineNumber);
                    if (!String.IsNullOrEmpty(trace))
                    {
                        trace = $" -- Invoker call trace:\n{func_call_str}\n{trace}";
                    }
                    else
                    {
                        trace = $" -- Invoker call trace:\n{func_call_str}";
                    }
                    Logging.Error(ex, $"Failed safe-exec.{trace}");
                }
            }
        }

#if DEBUG
#if false
        public static void TestAsyncErrorHandling()
        {
            Action bad_f = () =>
            {
                throw new Exception("boom!");
            };

            SafeExec(bad_f);
            WPFDoEvents.InvokeInUIThread(bad_f);
            WPFDoEvents.InvokeInUIThread(() =>
            {
                SafeExec(bad_f, "TestAsyncErrorHandling::InvokeInUIThread");
            });
            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.InvokeInUIThread(bad_f);
                WPFDoEvents.InvokeAsyncInUIThread(bad_f);
            });
            WPFDoEvents.InvokeAsyncInUIThread(bad_f);
            WPFDoEvents.InvokeAsyncInUIThread(() =>
            {
                WPFDoEvents.InvokeInUIThread(bad_f);
                WPFDoEvents.InvokeAsyncInUIThread(bad_f);
            });

            // this is the only one which is expected to invoke the Unhandled Exception Handler code
#if false
            SafeThreadPool.QueueUserWorkItem(() =>
            {
                bad_f();
            });
#endif

            SafeThreadPool.QueueUserWorkItem(() =>
            {
                SafeExec(bad_f, "TestAsyncErrorHandling::QueueUserWorkItem");
            });
            SafeThreadPool.QueueUserWorkItem(() =>
            {
                WPFDoEvents.InvokeInUIThread(bad_f);
                WPFDoEvents.InvokeAsyncInUIThread(bad_f);
            });
        }
#else
        public static void TestAsyncErrorHandling()
        {
        }
#endif
#endif

            // Get us the name of the function which calls this member. As such it serves to provide
            // us with the "currently executing function".
            //
            // Derived from https://stackoverflow.com/a/15310053/1635910
            // via https://stackoverflow.com/questions/44153/can-you-use-reflection-to-find-the-name-of-the-currently-executing-method/15310053#15310053
            // and https://stackoverflow.com/questions/2652460/how-to-get-the-name-of-the-current-method-from-code
            public static string GetInvokingFunctionName([CallerMemberName] string callerName = null, [CallerFilePath] string sourceFilePath = null, [CallerLineNumber] int sourceLineNumber = 0)
        {
            string[] arr = sourceFilePath.Split('\\');
            string[] arr2 = new string[4];
            Array.Copy(arr, Math.Max(0, arr.Length - 4), arr2, 0, 4);
            return $"{callerName ?? "???"}::...{String.Join("/", arr2)}@{sourceLineNumber}";
        }
    }
}
