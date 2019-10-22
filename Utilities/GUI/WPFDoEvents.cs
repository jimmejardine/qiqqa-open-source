using System.Windows.Threading;
using System.Windows;
using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;

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
				&& null != Dispatcher.CurrentDispatcher 
				&& !Dispatcher.CurrentDispatcher.HasShutdownStarted 
				&& !Dispatcher.CurrentDispatcher.HasShutdownFinished
			)
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
            }
            else
            {
                Thread.Sleep(50);
            }
        }

        public static object ExitFrame(object f)
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
            if (Application.Current == null || Application.Current.Dispatcher.CheckAccess())
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
                    //if (null != Application.Current)
                    {
                        if (!Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown 
                            && null != Application.Current.Dispatcher 
                            && !Application.Current.Dispatcher.HasShutdownStarted 
                            && !Application.Current.Dispatcher.HasShutdownFinished
						)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                Logging.Debug(":::WaitForUIThreadActivityDone InvokeAsync started");
                                DoEvents();
                                Logging.Debug(":::WaitForUIThreadActivityDone InvokeAsync finished");
                            }));
                        }
                        else
                        {
                            Thread.Sleep(50);
                        }
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
            if (Application.Current != null)
            {
                // Set the cursor to Hourglass as per: http://www.csharp411.com/the-proper-way-to-show-the-wait-cursor/ 
                // --> https://stackoverflow.com/questions/11021422/how-do-i-display-wait-cursor-during-a-wpf-applications-startup
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                });
            }
        }

        public static void ResetHourglassCursor()
        {
            if (Application.Current != null)
            {
                // revert the forced hourglass cursor, if any:
                //
                // RESET the cursor to Hourglass as per: http://www.csharp411.com/the-proper-way-to-show-the-wait-cursor/ 
                // --> https://stackoverflow.com/questions/11021422/how-do-i-display-wait-cursor-during-a-wpf-applications-startup
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Mouse.OverrideCursor = null;
                });
            }
        }
    }
}

