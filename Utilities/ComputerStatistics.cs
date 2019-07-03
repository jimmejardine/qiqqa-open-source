using System;
using System.Deployment.Application;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace Utilities
{
    public class ComputerStatistics
    {
        public static string GetCommonStatistics()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("COMMON STATISTICS:\r\n");

            sb.AppendFormat("Current version:       {0}\r\n", ClientVersion.CurrentVersion);
            sb.AppendFormat("Current path:          {0}\r\n", Environment.CurrentDirectory);
            sb.AppendFormat("System path:           {0}\r\n", Environment.SystemDirectory);
            sb.AppendFormat("Command line:          {0}\r\n", Environment.CommandLine);
            sb.AppendFormat("OS version:            {0}\r\n", Environment.OSVersion);
            sb.AppendFormat("CPU count:             {0}\r\n", Environment.ProcessorCount);
            sb.AppendFormat("Machine name:          {0}\r\n", Environment.MachineName);
            sb.AppendFormat("CLR version:           {0}\r\n", Environment.Version);
            sb.AppendFormat("Working set:           {0}Mb\r\n", Environment.WorkingSet / 1024 / 1024);
            sb.AppendFormat("Application data:      {0}\r\n", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            sb.AppendFormat("Temp path:             {0}\r\n", Path.GetTempPath());
            sb.AppendFormat("CurrentCulture:        {0}\r\n", CultureInfo.CurrentCulture);
            sb.AppendFormat("CurrentUICulture:      {0}\r\n", CultureInfo.CurrentUICulture);
            sb.AppendFormat("CurrentUICulture.Name: {0}\r\n", CultureInfo.CurrentUICulture.Name);
            sb.AppendFormat("InstalledUICulture:    {0}\r\n", CultureInfo.InstalledUICulture);
            sb.AppendFormat("InvariantCulture:      {0}\r\n", CultureInfo.InvariantCulture);
            sb.AppendFormat("UTC offset:            {0}\r\n", (DateTime.Now - DateTime.UtcNow).TotalHours);
            sb.AppendFormat(".NET4 client installed {0}\r\n", IsNET4ClientInstalled());
            sb.AppendFormat(".NET4 full installed   {0}\r\n", IsNET4FullInstalled());
            

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

        public static bool IsNET4ClientInstalled()
        {
            return IsNET4XXXInstalled(@"Software\Microsoft\NET Framework Setup\NDP\v4\Client");
        }

        public static bool IsNET4FullInstalled()
        {
            return IsNET4XXXInstalled(@"Software\Microsoft\NET Framework Setup\NDP\v4\Full");
        }

        private static bool IsNET4XXXInstalled(string reg_key)
        {
            try
            {
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(reg_key))
                {
                    if (null != rk)
                    {
                        return null != rk.GetValue(@"Install", null, RegistryValueOptions.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Unable to check for .NET4 version.");
            }

            return false;
        }


        public static bool IsNET4Installed()
        {
            return false
                || IsNET4ClientInstalled()
                || IsNET4FullInstalled()
                ;
        }
    }
}
