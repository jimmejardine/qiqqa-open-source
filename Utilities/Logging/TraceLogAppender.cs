using System;
using System.Text;
using System.Diagnostics;
using log4net.Appender;
using log4net.Core;

//
// https://stackoverflow.com/questions/11802663/log4net-traceappender-only-logs-messages-with-level-verbose-when-using-windows
//
//
namespace Utilities.log4net
{
    public class QiqqaTraceAppender : TraceAppender
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            var level = loggingEvent.Level;
            var message = RenderLoggingEvent(loggingEvent).TrimEnd();

            // skip Qiqqa Logging init/setup log line + stacktrace:
            if (message.Contains("Logging initialised."))
            {
                return;
            }

            if (level >= Level.Error)
            {
                Trace.TraceError(message);
            }
            else if (level >= Level.Warn)
            {
                Trace.TraceWarning(message);
            }
            else if (level >= Level.Info)
            {
                Trace.TraceInformation(message);
            }
            else
            {
                Trace.WriteLine(message);
            }

            if (ImmediateFlush)
            {
                Trace.Flush();
            }
        }
    }
}
