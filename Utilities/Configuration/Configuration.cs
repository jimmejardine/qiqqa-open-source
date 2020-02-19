using System.Net;

// Hacky solution to make certain configuration items available to Utilities namespace classes/code.

namespace Utilities
{
    public static class Configuration
    {
        public delegate string GetWebUserAgentCB();
        public delegate IWebProxy GetProxyCB();

        public static GetWebUserAgentCB GetWebUserAgent = null;
        public static GetProxyCB GetProxy = null;
    }
}
