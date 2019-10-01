using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;
using log4net;
using log4net.Appender;
using log4net.Config;

namespace Utilities
{
    public class Logging
    {
        static ILog __log;

        internal struct LogBufEntry
        {
            internal Exception ex;
            internal string message;
        }
        static List<LogBufEntry> init_ex_list;

        static Logging()
        {
            ILog dummy_fetch_to_init_logger = log;
        }

        static ILog log
        {
            get
            {
                if (null != __log) return __log;

                try
                {
                    BasicConfigurator.Configure();
                    XmlConfigurator.Configure();
                    __log = LogManager.GetLogger(typeof(Logging));
                    Info("Logging initialised.{0}", LogAssist.AppendStackTrace(null, "get_log"));

                    if (init_ex_list != null && init_ex_list.Count > 0)
                    {
                        Error("Logging init failures (due to premature init/usage?):");
                        foreach (LogBufEntry ex in init_ex_list)
                        {
                            if (ex.ex != null)
                            {
                                if (ex.message != null)
                                {
                                    Error(ex.ex, "Logger init failure. Message: {0}", ex.message);
                                }
                                else
                                {
                                    Error(ex.ex, "Logger init failure.");
                                }
                            }
                            else
                            {
                                Info(ex.message);
                            }
                        }
                        init_ex_list.Clear();
                        init_ex_list = null;
                    }
                }
                catch (Exception ex)
                {
                    BufferException(ex);
                }

                return __log;
            }
        }

        private static void BufferException(Exception _ex, string msg = null)
        {
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
        private static void BufferMessage(string msg)
        {
            if (init_ex_list == null)
            {
                init_ex_list = new List<LogBufEntry>();
            }
            init_ex_list.Add(new LogBufEntry
            {
                message = msg
            });
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

            Utilities.LockPerfTimer l1_clk = Utilities.LockPerfChecker.Start();
            lock (l1)
            {
                l1_clk.LockPerfTimerStop();
                msg += "x";
                Utilities.LockPerfTimer l2_clk = Utilities.LockPerfChecker.Start();
                    System.Threading.Thread.Sleep(10 * 1000);
                    l2_clk.LockPerfTimerStop();
                    msg += "x";
            }
#endif
        }

        public static void Debug(string msg)
        {
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        public static void Debug(string msg, params object[] args)
        {
            msg = String.Format(msg, args);
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        public static void Debug(Exception ex)
        {
            string msg = ex.ToString();
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        public static void Debug(Exception ex, string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(msg);
            sb.AppendLine();
            sb.Append(ex.ToString());
            msg = sb.ToString();
            //log?.Debug(msg) ?? BufferMessage(msg);
            LogDebug(msg);
        }

        public static void Debug(Exception ex, string msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(msg, args);
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(ex.ToString());
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
            string msg = ex.ToString();
            //log?.Warn(msg) ?? BufferMessage(msg);
            LogWarn(msg);
        }

        public static void Warn(Exception ex, string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(msg);
            sb.AppendLine();
            sb.Append(ex.ToString());
            msg = sb.ToString();
            //log?.Warn(msg) ?? BufferMessage(msg);
            LogWarn(msg);
        }

        public static void Warn(Exception ex, string msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(msg, args);
            sb.AppendLine();
            sb.Append(ex.ToString());
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
            string msg = ex.ToString();
            //log?.Error(msg) ?? BufferMessage(msg);
            LogError(msg);
            return msg;
        }

        public static string Error(Exception ex, string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(msg);
            sb.AppendLine();
            sb.Append(ex.ToString());
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
            sb.Append(ex.ToString());
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
