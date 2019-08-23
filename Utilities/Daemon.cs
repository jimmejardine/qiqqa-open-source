using System;
using System.Threading;

namespace Utilities
{
    public class Daemon
    {
        string daemon_name;
        Thread thread;
        bool still_running = false;
        object still_running_lock = new object();

        public Daemon(string daemon_name)
        {
            this.daemon_name = daemon_name;
        }

        public void Start(ParameterizedThreadStart thread_start, object param)
        {
            thread = new Thread(thread_start);
            thread.Name = "Daemon." + daemon_name;
            still_running = true;
            thread.Start(param);
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
            lock (still_running_lock)
            {
                still_running = false;
            }
        }

        public void Join()
        {
            // when user code hasn't called Stop() yt, we do it for them to signal 
            // any running code in the thread that time is up:
            Stop();

            thread.Join();
        }

        public bool Join(int timeout_milliseconds)
        {
            // when user code hasn't called Stop() yt, we do it for them to signal 
            // any running code in the thread that time is up:
            Stop();

            return thread.Join(timeout_milliseconds);
        }

        public bool StillRunning
        {
            get
            {
                lock (still_running_lock)
                {
                    return still_running;
                }
            }
        }

        public void Sleep()
        {
            Sleep(500);
        }

        public ThreadPriority Priority
        {
            set
            {
                thread.Priority = value;
            }
        }

        /// <summary>
        /// Put the daemon to sleep, but in a way that will end sooner if the app is exiting
        /// </summary>
        /// <param name="timeout_milliseconds"></param>
        public void Sleep(int timeout_milliseconds)
        {
            int timeout_milliseconds_remaining = timeout_milliseconds;
            while (StillRunning && !Utilities.Shutdownable.ShutdownableManager.Instance.IsShuttingDown && timeout_milliseconds_remaining > 0)
            {
                int sleep_time = Math.Min(timeout_milliseconds_remaining, 500);
                timeout_milliseconds_remaining -= sleep_time;
                Thread.Sleep(sleep_time);
            }
        }
    }
}
