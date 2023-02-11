#if XULRUNNER_GECKO_ANTIQUE

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Qiqqa.Common.Configuration;
using Qiqqa.UtilisationTracking;
using Utilities;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


#if false
namespace Qiqqa.WebBrowsing.GeckoStuff
{
    public static class GeckoManager
    {
#region --- Some external DLLs that we will need ------------------------------------------------

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetEnvironmentVariable(string lpName, string lpValue);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr LoadLibraryEx(string dllFilePath, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool FreeLibrary(IntPtr dllPointer);

        //static uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        //static uint LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040;
        private static uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;

#endregion ------------------------------------------------------------------------------------

#region --- Our dependency DLLs ---------------------------------------------------------------

        private static List<string> DEPENDENCY_DLLS = new List<string>
        {
            
            // Order is IMPORTANT - they are loaded in REVERSE dependency so that Windows never invokes its own dependency resolution code
            "msvcr100",
            "msvcp100",
            "mozglue",
            //"breakpadinjector",   //  - causes a crash on jimme's machines...
            "icudt52",
            "icuuc52",
            "icuin52",
            "sandboxbroker",
            //"nspr4",
            "mozjs",
            //"plc4",
            //"plds4",
            //"nssutil3",            
            "nss3",
            //"smime3",
            //"ssl3",
            //"mozsqlite3",
            "mozalloc",
            "gkmedias",
            "xul",
            "softokn3",
            "nssdbm3",
            "nssckbi",
            //"d3dx9_43",
            "D3DCompiler_43",
            "D3DCompiler_46",
            "libGLESv2",
            "libEGL",
            "IA2Marshal",
            "gkmedias",
            "freebl3",
            "AccessibleMarshal",
        };

#endregion ------------------------------------------------------------------------------------


        private static bool have_initialised = false;

        public static void Initialise()
        {
            try
            {
                Logging.Info("+Initialising Gecko DLL search path");

                // Work out the paths
                string installation_directory = GeckoInstaller.InstallationDirectory;

                // Set the application PATH to try load the DLL
                {
                    string path_value = ConfigurationManager.Instance.StartupDirectoryForQiqqa + ";" + GeckoInstaller.InstallationDirectory;
                    if (!SetEnvironmentVariable("PATH", path_value))
                    {
                        Logging.Error("There was a problem setting the PATH for Qiqqa to {0}", path_value);
                    }
                }

                // Set the DLL loader installation directory
                {
                    if (!SetDllDirectory(installation_directory))
                    {
                        Logging.Error("There was a problem setting the Gecko DLL search path to {0}", installation_directory);
                    }
                }

                // Preload each of the dependency DLLS
                {
                    Logging.Info("+Preloading GeckoFX DLLs");
                    foreach (string dependency_dll in DEPENDENCY_DLLS)
                    {
                        try
                        {
                            Logging.Info("Preloading {0}", dependency_dll);
                            string dll_path = Path.GetFullPath(Path.Combine(GeckoInstaller.InstallationDirectory, dependency_dll + ".dll"));
                            IntPtr module_handle = LoadLibraryEx(dll_path, IntPtr.Zero, LOAD_WITH_ALTERED_SEARCH_PATH);
                            if (module_handle == IntPtr.Zero)
                            {
                                int error_code = Marshal.GetLastWin32Error();
                                throw new Exception(string.Format("LoadLibraryEx returned NULL while loading '{0}', error_code={1}", dll_path, error_code));
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(ex, "Error preloading {0}", dependency_dll);
                            FeatureTrackingManager.Instance.UseFeature(Features.Exception_GeckoPreload, "dll", dependency_dll);
                        }
                    }
                    Logging.Info("-Preloading GeckoFX DLLs");
                }

                Logging.Info("+Initialising GeckoFX - profile directory is at {0}", Xpcom.ProfileDirectory);
                Xpcom.Initialize(installation_directory);
                Logging.Info("-Initialising GeckoFX");

                have_initialised = true;

                SetupProxyAndUserAgent(true);
            }
            catch (Exception ex)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Exception_GeckoPreferences);
                Logging.Error(ex, "There was a problem initialising Gecko.");
            }
        }

        public static void SetupUserAgent()
        {
            if (!have_initialised)
            {
                Logging.Warn("GeckoFX is not setting user agent until initialised.");
                return;
            }

            try
            {
            }
            catch (Exception ex)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Exception_GeckoPreferences);
                Logging.Error(ex, "There was a problem setting the Gecko user agent.");
            }

            Logging.Info("-Setting user agent");
        }

        public static void SetupProxyAndUserAgent(bool first_setting_of_proxy)
        {
            if (!have_initialised)
            {
                Logging.Warn("GeckoFX is not setting proxy and user agent until initialised.");
                return;
            }

            try
            {
                ConfigurationRecord configuration_record = ConfigurationManager.Instance.ConfigurationRecord;

                {
                    Logging.Info("+Setting user agent");

                    string user_agent = configuration_record.GetWebUserAgent();

                    GeckoPreferences.User["general.useragent.override"] = user_agent;
                    Logging.Info("-Setting user agent");
                }

                if (configuration_record.Proxy_UseProxy)
                {
                    Logging.Warn("Setting PROXY for GeckoFX");
                    GeckoPreferences.User["network.proxy.type"] = 1;
                    GeckoPreferences.User["network.proxy.user"] = configuration_record.Proxy_Username ?? "";
                    GeckoPreferences.User["network.proxy.password"] = configuration_record.Proxy_Password ?? "";
                    GeckoPreferences.User["network.proxy.http"] = configuration_record.Proxy_Hostname ?? "";
                    GeckoPreferences.User["network.proxy.http_port"] = configuration_record.Proxy_Port;
                    GeckoPreferences.User["network.proxy.socks"] = configuration_record.Proxy_Hostname ?? "";
                    GeckoPreferences.User["network.proxy.socks_port"] = configuration_record.Proxy_Port;
                    GeckoPreferences.User["network.proxy.socks_remote_dns"] = true;
                    GeckoPreferences.User["network.proxy.ssl"] = configuration_record.Proxy_Hostname ?? "";
                    GeckoPreferences.User["network.proxy.ssl_port"] = configuration_record.Proxy_Port;

                    //0 – Direct connection, no proxy. (Default)
                    //1 – Manual proxy configuration.
                    //2 – Proxy auto-configuration (PAC).
                    //4 – Auto-detect proxy settings.
                    //5 – Use system proxy settings (Default in Linux).   
                }
                else
                {
                    // Only try to turn off the proxy if there is a chance that we turned it on previously
                    if (!first_setting_of_proxy)
                    {
                        Logging.Info("Setting DEFAULT for GeckoFX");
                        GeckoPreferences.User["network.proxy.type"] = 5;
                    }
                    else
                    {
                        Logging.Info("Leaving DEFAULT for GeckoFX");
                    }
                }
            }

            catch (Exception ex)
            {
                FeatureTrackingManager.Instance.UseFeature(Features.Exception_GeckoProxy);
                Logging.Error(ex, "There was a problem setting the Gecko proxy and user agent.");
            }
        }

        public static void RegisterPDFInterceptor()
        {
            ObserverService.AddObserver(PDFInterceptor.Instance);
        }
    }
}

#endif
