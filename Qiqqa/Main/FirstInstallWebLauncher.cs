using System;
using System.Diagnostics;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Utilities;

namespace Qiqqa.Main
{
    internal class FirstInstallWebLauncher
    {
        public static void Check()
        {
            string fin = RegistrySettings.Instance.Read(RegistrySettings.FirstInstallNotification);
            if (String.IsNullOrEmpty(fin))
            {
                string version = "" + ClientVersion.CurrentVersion;
                RegistrySettings.Instance.Write(RegistrySettings.FirstInstallNotification, version);

                if (!RegistrySettings.Instance.GetPortableApplicationMode())
                {
                    string url = WebsiteAccess.GetOurUrl(WebsiteAccess.OurSiteLinkKind.Welcome) + "?version=" + version;
                    Process.Start(url);
                }
            }
        }
    }
}
