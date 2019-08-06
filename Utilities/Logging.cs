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

        public static void Debug(string msg)
        {
            log.Debug(msg);
        }

        public static string Debug(string msg, params object[] args)
        {
            string message = String.Format(msg, args);
            log.Debug(message);
            return message;
        }

        public static void Info(string msg)
        {
            msg = String.Format("[{0}] {1}", GC.GetTotalMemory(false), msg);
            log.Info(msg);
        }

        public static string Info(string msg, params object[] args)
        {
            string message = String.Format(msg, args);
            message = String.Format("[{0}] {1}", GC.GetTotalMemory(false), message);
            log.Info(message);
            return message;
        }

        public static string Warn(string msg, params object[] args)
        {
            string message = String.Format(msg, args);
            message = String.Format("[{0}] {1}", GC.GetTotalMemory(false), message);
            log.Warn(message);
            return message;
        }

        public static string Warn(Exception ex, string msg, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(msg, args);
            sb.AppendLine();
            sb.Append(ex.ToString());
            string message = sb.ToString();
            message = String.Format("[{0}] {1}", GC.GetTotalMemory(false), message);
            log.Warn(message);
            return message;
        }

        public static string Error(string msg, params object[] args)
        {
            string message = String.Format(msg, args);
            message = String.Format("[{0}] {1}", GC.GetTotalMemory(false), message);
            log.Error(message);
            return message;
        }

        public static string Error(Exception ex)
        {
            string message = ex.ToString();
            message = String.Format("[{0}] {1}", GC.GetTotalMemory(false), message);
            log.Error(message);
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
            message = String.Format("[{0}] {1}", GC.GetTotalMemory(false), message);
            log.Error(message);
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
