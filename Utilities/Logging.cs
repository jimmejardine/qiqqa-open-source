using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using log4net.Appender;
using log4net.Config;
using Utilities.Misc;
using LogLog = log4net.Util.LogLog;
using LogManager = log4net.LogManager;

namespace Utilities
{
    public class Logging
    {
        private static ILog __log;

        internal struct LogBufEntry
        {
            internal Exception ex;
            internal string message;
        }

        private static List<LogBufEntry> init_ex_list;

        static Logging()
        {
            TriggerInit();
        }

        private static bool log4net_loaded;
        private static bool log4net_init_pending;
        private static bool log4net_has_shutdown;

        // The following members MUST be accessed only within the critical section guarded by this lock:
        //
        // __log
        // init_ex_list
        // log4net_loaded
        // log4net_init_pending
        // log4net_has_shutdown
        //
        private static object log4net_loaded_lock = new object();

        private static ILog log
        {
            get
            {
                ILog rv;
                bool go;
                lock (log4net_loaded_lock)
                {
                    go = (null == __log && log4net_loaded && !log4net_init_pending && !log4net_has_shutdown);
                    rv = __log;
                }
                if (go)
                {
                    Init();
                }
                return rv;
            }
        }

        /// <summary>
        /// Test whether the log4net-based log system has been loaded and set up and if so, enable the use of that logging system.
        ///
        /// Up to that moment, all Logging APIs will log to a memory buffer/queue which will written to the logging system
        /// once it is loaded, set up and active.
        /// </summary>
        public static void TriggerInit()
        {
            bool go;
            lock (log4net_loaded_lock)
            {
                go = (null == __log && !log4net_loaded && !log4net_init_pending && !log4net_has_shutdown);
                if (go)
                {
                    log4net_init_pending = true; // block simultaneous execution of the rest of the code in TriggerInit()
                }
            }

            if (go)
            {
                bool we_are_ready_for_the_next_phase = false;
                try
                {
                    // test if log4net assembly is loaded and ready:
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    bool active = false;

                    foreach (Assembly assembly in assemblies)
                    {
                        if (assembly.FullName.StartsWith("log4net"))
                        {
                            active = true;
                            break;
                        }
                    }

                    if (active)
                    {
                        // as per https://stackoverflow.com/questions/28723920/check-whether-log4net-xmlconfigurator-succeeded
                        //
                        // log4net must have picked up the configuration or it will not function anyhow
                        if (!LogManager.GetRepository().Configured)
                        {
                            // log4net not configured
                            foreach (var message in LogManager.GetRepository().ConfigurationMessages)
                            {
                                // evaluate configuration message
                                LogLog logInitMsg = message as LogLog;
                                BufferMessage(String.Format("log config: {0}", logInitMsg?.Message ?? message));
                            }
                        }

                        we_are_ready_for_the_next_phase = true;
                    }
                }
                finally
                {
                    // make sure the pending flag is reset whenever we leave this block
                    lock (log4net_loaded_lock)
                    {
                        log4net_loaded = we_are_ready_for_the_next_phase;

                        log4net_init_pending = false; // de-block
                    }
                }
            }
        }

        private static bool Init()
        {
            bool go;
            lock (log4net_loaded_lock)
            {
                go = (null == __log && log4net_loaded && !log4net_init_pending && !log4net_has_shutdown);
                if (go)
                {
                    log4net_init_pending = true; // block simultaneous execution of the rest of the code in Init()
                }
            }

            if (go)
            {
                try
                {
                    BasicConfigurator.Configure();
                    XmlConfigurator.Configure();
                    ILog l = LogManager.GetLogger(typeof(Logging));
                    lock (log4net_loaded_lock)
                    {
                        __log = l;
                    }
                    // as per https://stackoverflow.com/questions/28723920/check-whether-log4net-xmlconfigurator-succeeded
                    //
                    // log4net must have picked up the configuration or it will not function anyhow
                    //if (!LogManager.GetRepository().Configured)
                    {
                        // log4net not configured
                        foreach (var message in LogManager.GetRepository().ConfigurationMessages)
                        {
                            // evaluate configuration message
                            LogLog logInitMsg = message as LogLog;
                            BufferMessage(String.Format("log4net config: {0}", logInitMsg?.Message ?? message));
                        }
                    }
                }
                catch (Exception ex)
                {
                    BufferException(ex);
                }
            }

            bool rv;
            lock (log4net_loaded_lock)
            {
                if (go)
                {
                    log4net_init_pending = false; // de-block
                }
                // return value: TRUE when success
                rv = (null != __log);
            }

            // only log/yak about initialization success when it actually did happen just above:
            if (rv)
            {
                Debug("Logging initialised at {0}", LogAssist.AppendStackTrace(null, "get_log"));
                Info("Logging initialised.");

                // thread safety: move and reset the pending message buffer/list
                List<LogBufEntry> lst = new List<LogBufEntry>();
                lock (log4net_loaded_lock)
                {
                    if (init_ex_list != null && init_ex_list.Count > 0)
                    {
                        // move list
                        lst = init_ex_list;
                        init_ex_list = null;
                    }
                }
                if (lst.Count > 0)
                {
                    Error("--- Logging early bird log messages: ---");
                    foreach (LogBufEntry ex in lst)
                    {
                        if (ex.ex != null)
                        {
                            if (ex.message != null)
                            {
                                Error(ex.ex, "{0}", ex.message);
                            }
                            else
                            {
                                Error(ex.ex, "Logger init failure?");
                            }
                        }
                        else
                        {
                            Info("{0}", ex.message);
                        }
                    }
                    lst.Clear();
                    Error("-- Logging early bird log messages done. ---");
                }
            }

            return rv;
        }

        public static void ShutDown()
        {
            Debug("Application + Logging ShutDown");
            lock (log4net_loaded_lock)
            {
                log4net_has_shutdown = true;
                __log = null;
            }
            LogManager.Flush(5000);
            LogManager.Shutdown();
        }

        public static bool HasShutDown
        {
            get
            {
                lock (log4net_loaded_lock)
                {
                    return log4net_has_shutdown;
                }
            }
        }

        private static void BufferException(Exception _ex, string msg = null)
        {
            lock (log4net_loaded_lock)
            {
                if (log4net_has_shutdown)
                {
                    return;
                }
                if (init_ex_list == null)
                {
                    init_ex_list = new List<LogBufEntry>();
                }
                init_ex_list.Add(new LogBufEntry
                {
                    ex = _ex,
                    message = msg
                });
            }
        }
        private static void BufferMessage(string msg)
        {
            lock (log4net_loaded_lock)
            {
                if (log4net_has_shutdown)
                {
                    return;
                }
                if (init_ex_list == null)
                {
                    init_ex_list = new List<LogBufEntry>();
                }
                init_ex_list.Add(new LogBufEntry
                {
                    message = msg
                });
            }
        }

        private static void LogDebug(string msg)
        {
            msg = LogAssist.CheckToBreakIntoDebugger(LogAssist.PrefixMemUsage(LogAssist.AppendStackTrace(msg)));
            ILog l = log;
            if (l != null)
            {
                l.Debug(msg);
            }
            else
            {
                BufferMessage(msg);
            }
        }
        private static void LogInfo(string msg)
        {
            msg = LogAssist.CheckToBreakIntoDebugger(LogAssist.PrefixMemUsage(LogAssist.AppendStackTrace(msg)));
            ILog l = log;
            if (l != null)
            {
                l.Info(msg);
            }
            else
            {
                BufferMessage(msg);
            }
        }
        private static void LogWarn(string msg)
        {
            msg = LogAssist.CheckToBreakIntoDebugger(LogAssist.PrefixMemUsage(LogAssist.AppendStackTrace(msg)));
            ILog l = log;
            if (l != null)
            {
                l.Warn(msg);
            }
            else
            {
                BufferMessage(msg);
            }
        }
        private static void LogError(string msg)
        {
            msg = LogAssist.CheckToBreakIntoDebugger(LogAssist.PrefixMemUsage(LogAssist.AppendStackTrace(msg)));
            ILog l = log;
            if (l != null)
            {
                l.Error(msg);
            }
            else
            {
                BufferMessage(msg);
            }

#if TEST
            object l1 = new object();
            object l2 = new object();

            // Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (l1)
            {
                // l1_clk.LockPerfTimerStop();
                msg += "x";
                // Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                    Thread.Sleep(10 * 1000);
                    // l2_clk.LockPerfTimerStop();
                    msg += "x";
            }
#endif
        }

        [Conditional("DIAG")]
        public static void Debug(string msg)
        {
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        [Conditional("DIAG")]
        public static void Debug(string msg, params object[] args)
        {
            msg = String.Format(msg, args);
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        [Conditional("DIAG")]
        public static void Debug(Exception ex)
        {
            string msg = ex.ToStringAllExceptionDetails();
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        [Conditional("DIAG")]
        public static void Debug(Exception ex, string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(msg);
            sb.AppendLine();
            sb.Append(ex.ToStringAllExceptionDetails());
            msg = sb.ToString();
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        [Conditional("DIAG")]
        public static void Debug(Exception ex, string msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(msg, args);
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(ex.ToStringAllExceptionDetails());
            msg = sb.ToString();
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        // Special (特) Debug Logging which should be available everywhere at my back&call, without unlocking The Debug Horde Of The Apocalypse (which are the Debug methods above).

        public static void Debug特(string msg)
        {
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        public static void Debug特(string msg, params object[] args)
        {
            msg = String.Format(msg, args);
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        public static void Debug特(Exception ex)
        {
            string msg = ex.ToStringAllExceptionDetails();
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        public static void Debug特(Exception ex, string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(msg);
            sb.AppendLine();
            sb.Append(ex.ToStringAllExceptionDetails());
            msg = sb.ToString();
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        public static void Debug特(Exception ex, string msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(msg, args);
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(ex.ToStringAllExceptionDetails());
            msg = sb.ToString();
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        public static string Info(string msg)
        {
            //log?.Info(msg) ?? BufferMessage(msg);
            LogInfo(msg);
            return msg;
        }

        public static string Info(string msg, params object[] args)
        {
            msg = String.Format(msg, args);
            //log?.Info(msg) ?? BufferMessage(msg);
            LogInfo(msg);
            return msg;
        }

        public static string Warn(string msg)
        {
            //log?.Warn(msg) ?? BufferMessage(msg);
            LogWarn(msg);
            return msg;
        }

        public static string Warn(string msg, params object[] args)
        {
            msg = String.Format(msg, args);
            //log?.Warn(msg) ?? BufferMessage(msg);
            LogWarn(msg);
            return msg;
        }

        public static void Warn(Exception ex)
        {
            string msg = ex.ToStringAllExceptionDetails();
            //log?.Warn(msg) ?? BufferMessage(msg);
            LogWarn(msg);
        }

        public static void Warn(Exception ex, string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(msg);
            sb.AppendLine();
            sb.Append(ex.ToStringAllExceptionDetails());
            msg = sb.ToString();
            //log?.Warn(msg) ?? BufferMessage(msg);
            LogWarn(msg);
        }

        public static void Warn(Exception ex, string msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(msg, args);
            sb.AppendLine();
            sb.Append(ex.ToStringAllExceptionDetails());
            msg = sb.ToString();
            //log?.Warn(msg) ?? BufferMessage(msg);
            LogWarn(msg);
        }

        public static string Error(string msg)
        {
            //log?.Error(msg) ?? BufferMessage(msg);
            LogError(msg);
            return msg;
        }

        public static string Error(string msg, params object[] args)
        {
            msg = String.Format(msg, args);
            //log?.Error(msg) ?? BufferMessage(msg);
            LogError(msg);
            return msg;
        }

        public static string Error(Exception ex)
        {
            string msg = ex.ToStringAllExceptionDetails();
            //log?.Error(msg) ?? BufferMessage(msg);
            LogError(msg);
            return msg;
        }

        public static string Error(Exception ex, string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(msg);
            sb.AppendLine();
            sb.Append(ex.ToStringAllExceptionDetails());
            msg = sb.ToString();
            //log?.Error(msg) ?? BufferMessage(msg);
            LogError(msg);
            return msg;
        }

        public static string Error(Exception ex, string msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(msg, args);
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(ex.ToStringAllExceptionDetails());
            msg = sb.ToString();
            //log?.Error(msg) ?? BufferMessage(msg);
            LogError(msg);
            return msg;
        }

        public static string GetLogFilename()
        {
            foreach (var logger in LogManager.GetRepository().GetAppenders())
            {
                RollingFileAppender rfa = logger as RollingFileAppender;
                if (null != rfa)
                {
                    return rfa.File;
                }
            }

            return null;
        }
    }
}
