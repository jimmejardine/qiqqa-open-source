using System.Diagnostics;

namespace Utilities
{
    public class WPFTrace : TraceListener
    {
        public override void Write(string message)
        {
            Logging.Warn(message);
        }

        public override void WriteLine(string message)
        {
            Logging.Warn(message);
        }
    }
}