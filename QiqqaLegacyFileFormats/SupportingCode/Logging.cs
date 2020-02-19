using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace QiqqaLegacyFileFormats          // namespace QiqqaLegacyFileFormats
{
    public class Logging
    {
        internal struct LogBufEntry
        {
            internal Exception ex;
            internal string message;
        }

        private static List<LogBufEntry> init_ex_list = null;

        static Logging()
        {
            //ILog dummy_fetch_to_init_logger = log;
            TriggerInit();
        }

        // The following members MUST be accessed only within the critical section guarded by this lock:
        //
        // __log
        // init_ex_list
        //
        private static object log4net_loaded_lock = new object();

        /// <summary>
        /// Test whether the log4net-based log system has been loaded and set up and if so, enable the use of that logging system.
        /// 
        /// Up to that moment, all Logging APIs will log to a memory buffer/queue which will written to the logging system
        /// once it is loaded, set up and active.
        /// </summary>
        public static void TriggerInit()
        {
        }

        private static void Init()
        {
            // only log/yak about initialization success when it actually did happen just above:
            Info("Logging initialised.");
        }

        private static void BufferException(Exception _ex, string msg = null)
        {
            lock (log4net_loaded_lock)
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
        }
        private static void BufferMessage(string msg)
        {
            lock (log4net_loaded_lock)
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
        }

        private static void LogDebug(string msg)
        {
            BufferMessage(msg);
        }
        private static void LogInfo(string msg)
        {
            BufferMessage(msg);
        }
        private static void LogWarn(string msg)
        {
            BufferMessage(msg);
        }
        private static void LogError(string msg)
        {
            BufferMessage(msg);
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
    }
}
