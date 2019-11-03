using System.Net;

// Hacky solution to make certain configuration items available to Utilities namespace classes/code.

namespace Utilities
{
    public static class Configuration
    {
        private static string _WebUserAgent;
        public static string WebUserAgent
        {
            get
            {
                FireOnBeingAccessed();
                return _WebUserAgent;
            }
            set => _WebUserAgent = value;
        }

        private static IWebProxy _Proxy;
        public static IWebProxy Proxy
        {
            get
            {
                FireOnBeingAccessed();
                return _Proxy;
            }
            set => _Proxy = value;
        }

        private static bool fired = false;
        private static void FireOnBeingAccessed()
        {
            if (!fired)
            {
                OnBeingAccessed();
            }
        }

        public static event ConfigAccessedHandler OnBeingAccessed;

        public delegate void ConfigAccessedHandler();
    }
}
