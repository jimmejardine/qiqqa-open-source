using System;
using System.Net;

namespace Utilities.Internet
{
    public class ProxyTools
    {
        public static readonly string USERNAME_DEFAULT_CREDENTIALS = "DefaultCredentials";
        public static readonly string USERNAME_DEFAULT_NETWORK_CREDENTIALS = "DefaultNetworkCredentials";

        public static IWebProxy CreateProxy(bool Proxy_UseProxy, string Proxy_Hostname, int Proxy_Port, string Proxy_Username, string Proxy_Password)
        {
            if (Proxy_UseProxy)
            {
                WebProxy proxy = new WebProxy(Proxy_Hostname, Proxy_Port);

                if (false) {}
                else if (Proxy_Username == USERNAME_DEFAULT_CREDENTIALS)
                {
                    proxy.Credentials = CredentialCache.DefaultCredentials;
                }
                else if (Proxy_Username == USERNAME_DEFAULT_NETWORK_CREDENTIALS)
                {
                    proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                else if (!String.IsNullOrEmpty(Proxy_Username))
                {
                    proxy.Credentials = new NetworkCredential(Proxy_Username, Proxy_Password);
                }

                return proxy;
            }
            else
            {
                return WebRequest.GetSystemWebProxy();
            }
        }
    }
}
