using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Utilities.Internet
{
    public class BrowserStarter
    {
        public static void OpenBrowser(string url)
        {
            // First try to rely on Windows automatically knowing what to do with a URL
            if (OpenBrowser_WINDOWS(url)) return;

            // If we get here, windows failed to understand, so try opening the browser process directly
            if (OpenBrowser_REGISTRY(url)) return;

            // If we get here, try iexplore, chrome, firefox directly
            if (OpenBrowser_APPLICATION("iexplore", url)) return;
            if (OpenBrowser_APPLICATION("chrome", url)) return;
            if (OpenBrowser_APPLICATION("firefox", url)) return;

            throw new Exception(String.Format("Unable to launch browser to url '{0}'", url));
        }

        private static bool OpenBrowser_WINDOWS(string url)
        {
            try
            {
                Process.Start(url);
                return true;
            }
            catch (Exception ex1)
            {
                Logging.Warn(ex1, "There was a problem spawning the web browser directly using url '{0}'", url);
                return false;
            }
        }

        private static bool OpenBrowser_REGISTRY(string url)
        {
            string browser_path = "";
            try
            {
                browser_path = GetDefaultBrowserPath();

                using (Process p = new Process())
                {
                    p.StartInfo.FileName = browser_path;
                    p.StartInfo.Arguments = url;
                    p.Start();
                }
                return true;
            }
            catch (Exception ex2)
            {
                Logging.Warn(ex2, "There was a problem spawning the web browser using the registry browser path '{0}'", browser_path);
                return false;
            }
        }

        private static bool OpenBrowser_APPLICATION(string browser, string url)
        {
            try
            {
                Process.Start(browser, url);
                return true;
            }
            catch (Exception ex2)
            {
                Logging.Warn(ex2, "There was a problem spawning the web browser using the application '{0}'", browser);
                return false;
            }
        }



        private static string GetDefaultBrowserPath()
        {
            string key = @"HTTP\shell\open\command";
            using (RegistryKey registrykey = Registry.ClassesRoot.OpenSubKey(key, false))
            {
                return ((string)registrykey.GetValue(null, null)).Split('"')[1];
            }
        }
    }
}
