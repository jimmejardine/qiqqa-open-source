using System;
using System.Threading;
using Utilities.Shutdownable;

namespace Utilities
{
    public class Daemon
    {
        private string daemon_name;
        private int daemon_index;
        private Thread thread;
        private bool still_running = false;
        private object still_running_lock = new object();

        public Daemon(string daemon_name, int daemon_index = -1)
        {
            this.daemon_name = daemon_name;
            this.daemon_index = daemon_index;
        }

        public int ManagedThreadId => thread.ManagedThreadId;

        public void Start(ParameterizedThreadStart thread_start, object param = null)
        {
            thread = new Thread(thread_start);
            thread.Name = daemon_index == -1 ? $"Daemon.{daemon_name}" : $"Daemon.{daemon_name}.{daemon_index + 1}";
            still_running = true;
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start(param ?? this); // if no explicit parameter is specified, pass the threadinfo as implicit parameter
        }

        /// <summary>
        /// Signal the daemon thread that it is being terminated.
        ///
        /// Check for this signal by testing <code>daemon.StillRunning</code>.
        ///
        /// <see cref="StillRunning" />
        /// </summary>
        public void Stop()
        {
            //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (still_running_lock)
            {
                //l1_clk.LockPerfTimerStop();
                still_running = false;
            }
        }

        public void Abort()
        {
            // when user code hasn't called Stop() yet, we do it for them to signal
            // any running code in the thread that time is up:
            Stop();

            thread.Abort();
        }

        public void Join()
        {
            // when user code hasn't called Stop() yet, we do it for them to signal
            // any running code in the thread that time is up:
            Stop();

            thread.Join();
        }

        public bool Join(int timeout_milliseconds)
        {
            // when user code hasn't called Stop() yet, we do it for them to signal
            // any running code in the thread that time is up:
            Stop();

            return thread.Join(timeout_milliseconds);
        }

        public bool StillRunning
        {
            get
            {
                //Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
                lock (still_running_lock)
                {
                    //l1_clk.LockPerfTimerStop();
                    return still_running;
                }
            }
        }

        public ThreadPriority Priority
        {
            set => thread.Priority = value;
        }

        /// <summary>
        /// Put the daemon to sleep, but in a way that will end sooner if the app is exiting
        /// </summary>
        /// <param name="timeout_milliseconds"></param>
        public void Sleep(int timeout_milliseconds = 500)
        {
            int timeout_milliseconds_remaining = timeout_milliseconds;
            while (StillRunning && !ShutdownableManager.Instance.IsShuttingDown && timeout_milliseconds_remaining > 0)
            {
                int sleep_time = Math.Min(timeout_milliseconds_remaining, 500);
                timeout_milliseconds_remaining -= sleep_time;
                Thread.Sleep(sleep_time);
            }
        }
    }
}
