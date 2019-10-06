using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace Utilities
{
    public class LockPerfTimer
    {
        internal Stopwatch clk;
        internal Timer t;
        internal string stackTrace;
        internal object obj_lock;
        internal ElapsedEventHandler handler_ref;

        public void LockPerfTimerStop()
        {
            double ms;

            lock (this.obj_lock)
            {
                ms = this.clk.ElapsedTicks * 1.0E3 / Stopwatch.Frequency;
                this.t.Elapsed -= this.handler_ref;
                this.t.Stop();
                this.t.Close();
                this.t = null;
                //                    this.clk.Stop();
                //                    this.clk = null;
            }

            if (ms > 250.0)
            {
                Logging.Warn("lock took {0} msec @ {1}", ms, this.stackTrace);
            }
        }
    }

    public class LockPerfChecker
    {
        public static LockPerfTimer Start()
        {
            LockPerfTimer rv = new LockPerfTimer
            {
                clk = Stopwatch.StartNew(),
                t = new Timer(),
                stackTrace = LogAssist.AppendStackTrace(null, "LockPerfTimerStart").Trim(),
                obj_lock = new object(),
            };
            rv.handler_ref = new ElapsedEventHandler((object source, ElapsedEventArgs e) =>
            {
                Timer t = (source as Timer);

                lock (rv.obj_lock)
                {
                    // when the event fired while the mainline code flow STOPPED the timer inside its critical section,
                    // we should ignore this (queued) event: this sort of thing can easily occur while debugging the
                    // application.
                    if (rv.t == null)
                    {
                        t.Stop();
                        t.Close();
                        //                        return;
                    }
                    else if (t != rv.t)
                    {
                        throw new Exception("internal failure");
                    }
                }
                OnTimedEvent(rv, 1);

            });
            // Hook up the Elapsed event for the timer. 
            rv.t.Elapsed += rv.handler_ref;

            rv.t.AutoReset = false;
            rv.t.Interval = 5 * 1000;
            rv.t.Enabled = true;
            return rv;
        }

        // Handle the Elapsed event.
        private static void OnTimedEvent(LockPerfTimer pt, int i)
        {
            Logging.Debug特("+Lock TIMEOUT: {0} msec @ {1}", pt.clk.ElapsedMilliseconds, pt.stackTrace);
        }
    }

    /// <summary>
    /// Because it turns out that the `Logging` class does somehow automagickally threadlock
    /// every method that's defined in there.  >:-(
    /// </summary>
    public class LogAssist
    {
        public static string AppendStackTrace(string message, string marker = null)
        {
            // Do not append a StackTrace when there's already one:
            if (message?.Contains("  at ") ?? false)
            {
                return message;
            }
            // Do not append stacktrace to every log line: only specific
            // log messages should be augmented with a stacktrace:
            if (String.IsNullOrEmpty(message)
                || message.Contains("Object reference not set to an instance of an object"))
            {
                string t = Environment.StackTrace;
                int pos = t.IndexOf(marker ?? "AppendStackTrace");
                pos = t.IndexOf("\n", pos + 1);
                t = t.Substring(pos + 1);

                return message + "\n  Stacktrace:\n    " + t.Replace("\n", "\n    ");
            }
            return message;
        }

        public static string PrefixMemUsage(string message)
        {
            return String.Format("[{0:0.000}M] {1}", ((double)GC.GetTotalMemory(false)) / 1E6, message);
        }

#if false
        private static string CheckToBreakIntoDebugger(string msg)
        {
            if (msg.Contains("Browser") 
                || msg.Contains("Active browser control changed")
                || msg.Contains("Error while processing pipe connection")
                || msg.Contains("ObjectDisposedException")
                || msg.Contains("ArgumentOutOfRangeException")
                || msg.Contains("IndexOutOfRangeException")
            )
            {
                // break!
                msg = "BRK!" + msg;
            }
            return msg;
        }
#else
        public static string CheckToBreakIntoDebugger(string msg)
        {
            // this function is a no-op
            return msg;
        }
#endif
    }
}
