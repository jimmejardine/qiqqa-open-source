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
            log.Info(msg);
        }

        public static string Info(string msg, params object[] args)
        {
            string message = String.Format(msg, args);
            log.Info(message);
            return message;
        }

        public static string Warn(string msg, params object[] args)
        {
            return Warn(true, msg, args);
        }

        public static string Warn(bool show_stack, string msg, params object[] args)
        {
            string message = String.Format(msg, args);
            log.Warn(message);
            return message;
        }

        public static string Error(string msg, params object[] args)
        {
            string message = String.Format(msg, args);
            log.Error(message);
            return message;
        }

        public static string Warn(Exception ex, string msg, params object[] args)
        {
            return Warn(true, ex, msg, args);
        }
        
        public static string Warn(bool show_stack, Exception ex, string msg, params object[] args)
        {            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(msg, args);
            if (show_stack)
            {
                sb.AppendLine();
                sb.Append(ex.ToString());
            }

            string message = sb.ToString();
            log.Warn(message);
            return message;
        }

        public static string Error(Exception ex)
        {
            string message = ex.ToString();
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
