using System;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Config;

namespace Utilities
{
    public class Logging
    {
        static ILog log;

        static Logging()
        {
            BasicConfigurator.Configure();
            XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(Logging));
            Info("Logging initialised");
        }

        private static string AppendStackTrace(string message)
        {
            // Do not append a StackTrace when there's already one:
            if (message.Contains("  at "))
            {
                return message;
            }
            // Do not append stacktrace to every log line: only specific
            // log messages should be augmented with a stacktrace:
            if (message.Contains("Object reference not set to an instance of an object") 
                || message.Contains("Logging initialised"))
            {
                string t = Environment.StackTrace;
                return message + "\n  Stacktrace:\n    " + t.Replace("\n", "\n    ");
            }
            return message;
        }

        private static string PrefixMemUsage(string message)
        {
            return String.Format("[{0}] {1}", ((double)GC.GetTotalMemory(false)) / 1E6, message);
        }

        public static void Debug(string msg)
        {
            log.Debug(PrefixMemUsage(AppendStackTrace(msg)));
        }

        public static string Debug(string msg, params object[] args)
        {
            string message = String.Format(msg, args);
            log.Debug(PrefixMemUsage(AppendStackTrace(message)));
            return message;
        }

        public static void Info(string msg)
        {
            log.Info(PrefixMemUsage(AppendStackTrace(msg)));
        }

        public static string Info(string msg, params object[] args)
        {
            string message = String.Format(msg, args);
            log.Info(PrefixMemUsage(AppendStackTrace(message)));
            return message;
        }

        public static string Warn(string msg, params object[] args)
        {
            string message = String.Format(msg, args);
            log.Warn(PrefixMemUsage(AppendStackTrace(message)));
            return message;
        }

        public static string Warn(Exception ex, string msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(msg, args);
            sb.AppendLine();
            sb.Append(ex.ToString());
            string message = sb.ToString();
            log.Warn(PrefixMemUsage(AppendStackTrace(message)));
            return message;
        }

        public static string Error(string msg, params object[] args)
        {
            string message = String.Format(msg, args);
            log.Error(PrefixMemUsage(AppendStackTrace(message)));
            return message;
        }

        public static string Error(Exception ex)
        {
            string message = ex.ToString();
            log.Error(PrefixMemUsage(AppendStackTrace(message)));
            return message;
        }

        public static string Error(Exception ex, string msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(msg, args);
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(ex.ToString());
            string message = sb.ToString();
            log.Error(PrefixMemUsage(AppendStackTrace(message)));
            return message;
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
