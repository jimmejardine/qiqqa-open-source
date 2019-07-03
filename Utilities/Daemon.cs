using System;
using System.Threading;

namespace Utilities
{
    public class Daemon
    {
        string daemon_name;
        Thread thread;
        bool still_running;

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

        public void Stop()
        {
            still_running = false;
        }

        public void Join()
        {
            thread.Join();            
        }

        public bool Join(int timeout_milliseconds)
        {
            return thread.Join(timeout_milliseconds);
        }

        public bool StillRunning
        {
            get
            {
                return still_running;
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
            while (StillRunning && timeout_milliseconds_remaining > 0)
            {
                int sleep_time = Math.Min(timeout_milliseconds_remaining, 1000);
                timeout_milliseconds_remaining -= sleep_time;                
                Thread.Sleep(sleep_time);
            }            
        }
    }
}
