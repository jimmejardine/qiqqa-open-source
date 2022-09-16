using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Utilities.Shutdownable
{
    public class ShutdownableManager
    {
        public static ShutdownableManager Instance = new ShutdownableManager();
        public delegate void ShutdownDelegate();

        private List<ShutdownDelegate> shutdown_delegates = new List<ShutdownDelegate>();
        private object shutdown_delegates_lock = new object();

        private ShutdownableManager()
        {
            Logging.Info("Creating ShutdownableManager");
        }

        public void Register(ShutdownDelegate shutdown_delegate)
        {
            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (shutdown_delegates_lock)
            {
                // l1_clk.LockPerfTimerStop();
                Logging.Info("ShutdownableManager is registering {0}", shutdown_delegate.Target);
                shutdown_delegates.Add(shutdown_delegate);
            }
        }

        private bool is_being_shut_down = false;
        private object is_being_shut_down_lock = new object();

        public bool IsShuttingDown
        {
            get
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (is_being_shut_down_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    //if (System.Windows.Threading.Dispatcher.CurrentDispatcher != Application.Current?.Dispatcher)
                    //{
                    //	Logging.Error(new Exception("Unexpected results"), "woops");
                    //}

                    bool app_shuts_down = (null == Application.Current
                        || null == Application.Current.Dispatcher
                        || Application.Current.Dispatcher.HasShutdownStarted
                        || Application.Current.Dispatcher.HasShutdownFinished);

                    return is_being_shut_down || app_shuts_down;
                }
            }
            set
            {
                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (is_being_shut_down_lock)
                {
                    // l1_clk.LockPerfTimerStop();
                    is_being_shut_down = true;
                }
            }
        }

        static private string first_known_shutdown_reason = null;
        static private object first_known_shutdown_reason_lock = new object();

        public void Shutdown(string reason)
        {
            lock (first_known_shutdown_reason_lock)
            {
                if (first_known_shutdown_reason == null)
                {
                    first_known_shutdown_reason = reason;
                }
                else
                {
                    reason = $"{first_known_shutdown_reason}\n    Subsequent shutdown reason: {reason}";
                }
            }
            Logging.Info($"ShutdownableManager is shutting down all shutdownables. Reason: {reason}");

            IsShuttingDown = true;

            while (true)
            {
                ShutdownDelegate shutdown_delegate = null;
                // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (shutdown_delegates_lock)
                {
                    // l1_clk.LockPerfTimerStop();
                    if (!shutdown_delegates.Any()) break;
                    // process Shutdown registered items in LIFO order: shutdown in reverse of init sequence!
                    int idx = shutdown_delegates.Count - 1;
                    shutdown_delegate = shutdown_delegates[idx];
                    shutdown_delegates.RemoveAt(idx);
                }

                try
                {
                    Logging.Info("ShutdownableManager is shutting down {0}", shutdown_delegate.Target);
                    shutdown_delegate();
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem shutting down Shutdownable {0}", shutdown_delegate.Target);
                }
            }
        }

        /// <summary>
        /// (utility function) Put anyone to sleep, but in a way that will end sooner if the app is exiting
        /// </summary>
        /// <param name="timeout_milliseconds"></param>
        public static void Sleep(int timeout_milliseconds = 500)
        {
            // WARNING:
            //
            // this code is a near-duplicate of the Deamon.Sleep() code -- only here we don't check if the daemon is StillRunning...
            //
            int timeout_milliseconds_remaining = timeout_milliseconds;
            while (!ShutdownableManager.Instance.IsShuttingDown && timeout_milliseconds_remaining > 0)
            {
                int sleep_time = Math.Min(timeout_milliseconds_remaining, 500);
                timeout_milliseconds_remaining -= sleep_time;
                Thread.Sleep(sleep_time);
            }
        }
    }
}
