using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace Utilities
{
    public class ComputerStatistics
    {
        public class NETVersionInfo
        {
            public string Version;
            public string InstalledVersion;
            public string ServicePack;
            public string Type;
            public int ReleaseKey;

            public override string ToString()
            {
                string typeStr = "";
                if (!String.IsNullOrEmpty(Type))
                {
                    typeStr = $" ({Type})";
                }
                string spStr = "";
                if (!String.IsNullOrEmpty(ServicePack))
                {
                    spStr = $"  SP{ServicePack}";
                }
                return $"{Version}{typeStr}  {InstalledVersion}{spStr}";
            }

            // overload > < >= <=
            public static bool operator <(NETVersionInfo a, NETVersionInfo b)
            {
                return Compare(a, b) > 0;
            }
            public static bool operator >(NETVersionInfo a, NETVersionInfo b)
            {
                return Compare(a, b) < 0;
            }
            public static bool operator <=(NETVersionInfo a, NETVersionInfo b)
            {
                return Compare(a, b) >= 0;
            }
            public static bool operator >=(NETVersionInfo a, NETVersionInfo b)
            {
                return Compare(a, b) <= 0;
            }
            public static bool operator ==(NETVersionInfo a, NETVersionInfo b)
            {
                return a.InstalledVersion == b.InstalledVersion;
            }
            public static bool operator !=(NETVersionInfo a, NETVersionInfo b)
            {
                return a.InstalledVersion != b.InstalledVersion;
            }
            public static int Compare(NETVersionInfo a, NETVersionInfo b)
            {
                var ar = a.InstalledVersion.Split('.');
                var br = b.InstalledVersion.Split('.');
                for (int i = 0, l = Math.Min(ar.Length, br.Length); i < l; i++)
                {
                    int ai;
                    int bi;
                    if (!int.TryParse(ar[i], out ai))
                    {
                        ai = 0;
                    }
                    if (!int.TryParse(br[i], out bi))
                    {
                        bi = 0;
                    }
                    if (ai != bi)
                    {
                        return bi - ai;
                    }
                }
                int d = br.Length - ar.Length;
                if (d != 0)
                {
                    return d;
                }
                if (b.Type == "Full" && a.Type != "Full")
                {
                    return 1;
                }
                if (a.Type == "Full" && b.Type != "Full")
                {
                    return -1;
                }
                return 0;
            }
        }

        public class CLRInfo
        {
            public NETVersionInfo Version;
            public bool IsNET4ClientInstalled = false;
            public bool IsNET4FullInstalled = false;
            public List<string> NET4TypesInstalled;
            public List<NETVersionInfo> NETVersionsInstalled;
            public string RawClrVersion;

            public CLRInfo()
            {
                NET4TypesInstalled = new List<string>();
                NETVersionsInstalled = new List<NETVersionInfo>();
            }
        }

        public static string GetCommonStatistics()
        {
            StringBuilder sb = new StringBuilder();
            CLRInfo inf = GetCLRInfo();

            sb.AppendFormat("COMMON STATISTICS:\r\n");

            sb.AppendFormat("Current version:       {0}\r\n", ClientVersion.CurrentVersion);
            sb.AppendFormat("Current build:         {0}\r\n", ClientVersion.CurrentBuild);
            sb.AppendFormat("Current path:          {0}\r\n", Environment.CurrentDirectory);
            sb.AppendFormat("System path:           {0}\r\n", Environment.SystemDirectory);
            sb.AppendFormat("Command line:          {0}\r\n", Environment.CommandLine);
            sb.AppendFormat("OS version:            {0}\r\n", Environment.OSVersion);
            sb.AppendFormat("CPU count:             {0}\r\n", Environment.ProcessorCount);
            sb.AppendFormat("Machine name:          {0}\r\n", Environment.MachineName);
            sb.AppendFormat("CLR version:           {0} ({1})\r\n", inf.Version, inf.RawClrVersion);
            sb.AppendFormat("Working set:           {0}Mb\r\n", Environment.WorkingSet / 1024 / 1024);
            sb.AppendFormat("Application data:      {0}\r\n", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            sb.AppendFormat("Temp path:             {0}\r\n", Path.GetTempPath());
            sb.AppendFormat("CurrentCulture:        {0}\r\n", CultureInfo.CurrentCulture);
            sb.AppendFormat("CurrentUICulture:      {0}\r\n", CultureInfo.CurrentUICulture);
            sb.AppendFormat("CurrentUICulture.Name: {0}\r\n", CultureInfo.CurrentUICulture.Name);
            sb.AppendFormat("InstalledUICulture:    {0}\r\n", CultureInfo.InstalledUICulture);
            sb.AppendFormat("InvariantCulture:      {0}\r\n", CultureInfo.InvariantCulture);
            sb.AppendFormat("UTC offset:            {0}\r\n", (DateTime.Now - DateTime.UtcNow).TotalHours);
            sb.AppendFormat(".NET4 client installed {0}\r\n", inf.IsNET4ClientInstalled);
            sb.AppendFormat(".NET4 full installed   {0}\r\n", inf.IsNET4FullInstalled);
            sb.AppendFormat(".NET CLR versions installed:\r\n");
            foreach (NETVersionInfo v in inf.NETVersionsInstalled)
            {
                sb.AppendFormat("- .NET version:         {0}\r\n", v);
            }

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                sb.AppendFormat("Application is network deployed:\r\n");
                sb.AppendFormat(" - ActivationUri                {0}\r\n", ApplicationDeployment.CurrentDeployment.ActivationUri);
                sb.AppendFormat(" - CurrentVersion               {0}\r\n", ApplicationDeployment.CurrentDeployment.CurrentVersion);
                sb.AppendFormat(" - DataDirectory                {0}\r\n", ApplicationDeployment.CurrentDeployment.DataDirectory);
                sb.AppendFormat(" - IsFirstRun                   {0}\r\n", ApplicationDeployment.CurrentDeployment.IsFirstRun);
                sb.AppendFormat(" - TimeOfLastUpdateCheck        {0}\r\n", ApplicationDeployment.CurrentDeployment.TimeOfLastUpdateCheck);
                sb.AppendFormat(" - UpdatedApplicationFullName   {0}\r\n", ApplicationDeployment.CurrentDeployment.UpdatedApplicationFullName);
                sb.AppendFormat(" - UpdatedVersion               {0}\r\n", ApplicationDeployment.CurrentDeployment.UpdatedVersion);
                sb.AppendFormat(" - UpdateLocation               {0}\r\n", ApplicationDeployment.CurrentDeployment.UpdateLocation);
            }
            else
            {
                sb.AppendFormat("Application is not network deployed.\r\n");
            }

            return sb.ToString();
        }

        public static void LogCommonStatistics()
        {
            Logging.Info(GetCommonStatistics());
        }

        // Taken from https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
        // and adapted to our needs.

        // Checking the version using >= enables forward compatibility.
        private static string CheckFor45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 528040)
                return "4.8 or later";
            if (releaseKey >= 461808)
                return "4.7.2";
            if (releaseKey >= 461308)
                return "4.7.1";
            if (releaseKey >= 460798)
                return "4.7";
            if (releaseKey >= 394802)
                return "4.6.2";
            if (releaseKey >= 394254)
                return "4.6.1";
            if (releaseKey >= 393295)
                return "4.6";
            if (releaseKey >= 379893)
                return "4.5.2";
            if (releaseKey >= 378675)
                return "4.5.1";
            if (releaseKey >= 378389)
                return "4.5";
            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }

        // The example displays output similar to the following:
        //        v2.0.50727  2.0.50727.4927  SP2
        //        v3.0  3.0.30729.4926  SP2
        //        v3.5  3.5.30729.4926  SP1
        //        v4.0
        //        Client  4.0.0.0
        public static CLRInfo GetCLRInfo()
        {
            CLRInfo rv = new CLRInfo();
            NETVersionInfo highestInstalledNETVersion = new NETVersionInfo()
            {
                InstalledVersion = ""
            };

            RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

            rv.RawClrVersion = Environment.Version.ToString();

            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\";

            // Opens the registry key for the .NET Framework entry.
            using (var ndpKey = baseKey.OpenSubKey(subkey))
            {
                foreach (var versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        //rv.NETVersionsInstalled.Add(versionKeyName.Substring(1));

                        using (RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName))
                        {
                            // Get the .NET Framework version value.
                            var name = (string)versionKey.GetValue("Version", "");
                            // Get the service pack (SP) number.
                            var sp = versionKey.GetValue("SP", "").ToString();
                            var releaseVersion = versionKey.GetValue("Release");
                            int releaseKey = releaseVersion as int? ?? 0;

                            // Get the installation flag, or an empty string if there is none.
                            var install = versionKey.GetValue("Install", "").ToString();
                            if (string.IsNullOrEmpty(install)) // No install info; it must be in a child subkey.
                            {
                                //Console.WriteLine($"{versionKeyName}  {name}");
                            }
                            else if (install == "1")
                            {
                                NETVersionInfo rec = new NETVersionInfo()
                                {
                                    Version = versionKeyName,
                                    InstalledVersion = name,
                                    ServicePack = sp,
                                    ReleaseKey = releaseKey
                                };
                                if (highestInstalledNETVersion < rec)
                                {
                                    highestInstalledNETVersion = rec;
                                }
                                if (!rv.NETVersionsInstalled.Contains(rec))
                                {
                                    rv.NETVersionsInstalled.Add(rec);
                                }
                            }
                            if (!string.IsNullOrEmpty(name))
                            {
                                continue;
                            }

                            foreach (var subKeyName in versionKey.GetSubKeyNames())
                            {
                                using (RegistryKey subKey = versionKey.OpenSubKey(subKeyName))
                                {
                                    name = (string)subKey.GetValue("Version", "");
                                    if (!string.IsNullOrEmpty(name))
                                    {
                                        sp = subKey.GetValue("SP", "").ToString();
                                        releaseVersion = subKey.GetValue("Release");
                                        releaseKey = releaseVersion as int? ?? 0;

                                        string v = versionKeyName;
                                        if (releaseKey > 0)
                                        {
                                            v = CheckFor45PlusVersion(releaseKey);
                                        }

                                        install = subKey.GetValue("Install", "").ToString();
                                        if (install == "1")
                                        {
                                            if (versionKeyName == "v4")
                                            {
                                                rv.NET4TypesInstalled.Add(subKeyName);

                                                if (subKeyName == "Client")
                                                {
                                                    rv.IsNET4ClientInstalled = true;
                                                }
                                                if (subKeyName == "Full")
                                                {
                                                    rv.IsNET4FullInstalled = true;
                                                }
                                            }

                                            NETVersionInfo rec = new NETVersionInfo()
                                            {
                                                Version = v,
                                                Type = subKeyName,
                                                InstalledVersion = name,
                                                ServicePack = sp,
                                                ReleaseKey = releaseKey
                                            };
                                            if (highestInstalledNETVersion < rec)
                                            {
                                                highestInstalledNETVersion = rec;
                                            }
                                            if (!rv.NETVersionsInstalled.Contains(rec))
                                            {
                                                rv.NETVersionsInstalled.Add(rec);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (String.IsNullOrEmpty(highestInstalledNETVersion.InstalledVersion))
            {
                rv.Version = null;
            }
            else
            {
                rv.Version = highestInstalledNETVersion;
            }

            return rv;
        }
    }
}
