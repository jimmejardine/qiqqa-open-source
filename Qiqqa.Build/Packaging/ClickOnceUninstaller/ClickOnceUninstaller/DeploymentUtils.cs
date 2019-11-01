//This was adapted from the following web page:
//http://www.jamesharte.com/blog/?p=11

using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;


namespace DeploymentUtilities
{
    public class DeploymentUtils
    {

        #region "Install/Uninstall"

        /// <summary>
        /// Uninstall the current version of the application.
        /// </summary>
        public static void UninstallMe()
        {
            // Find Uninstall string in registry    
            string DisplayName = null;
            string uninstallString = GetUninstallString(out DisplayName);
            if (uninstallString.Length <= 0)
            {
                return;
            }

            //uninstallString example: "rundll32.exe dfshim.dll,ShArpMaintain GMStudio.application, Culture=neutral, PublicKeyToken=e84b302e57430172, processorArchitecture=msil";
            string runDLL32 = uninstallString.Substring(0, 12);
            string args = uninstallString.Substring(13);

            if (MessageBox.Show(
                "You have an old ClickOnce version of Qiqqa installed.  To remove the previous version, a popup will appear - please select the 'Remove the application from this computer' option from the popup and then click OK.\n\nDo you want to proceed with the uninstall?\n\n(Note: your existing data will NOT be affected by the uninstall, and will be waiting for you after the new installation is complete.)",
                "Uninstall Previous Qiqqa",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information) == DialogResult.OK)
            {
                //start the uninstall; this will bring up the uninstall dialog 
                //  asking if it's ok
                Process uninstallProcess = Process.Start(runDLL32, args);
                uninstallProcess.WaitForExit();
            }

            //push the OK button
            //PushUninstallOKButton(DisplayName);
        }

        #endregion "Install/Uninstall"

        #region "GetTheUninstallInfo"
        /// <summary>
        /// Gets the uninstall string for the current ClickOnce app from the Windows 
        /// Registry.
        /// </summary>
        /// <param name="PublicKeyToken">The public key token of the app.</param>
        /// <returns>The command line to execute that will uninstall the app.</returns>
        public static string GetUninstallString(out string DisplayName)
        {
            string uninstallString = null;

            //open the registry key and get the subkey names
            using (RegistryKey uninstallKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"))
            {
                if (uninstallKey == null)
                {
                    DisplayName = string.Empty;
                    return string.Empty;
                }
                string[] appKeyNames = uninstallKey.GetSubKeyNames();

                DisplayName = null;
                bool found = false;

                //search through the list for one with a match 
                foreach (string appKeyName in appKeyNames)
                {
                    using (RegistryKey appKey = uninstallKey.OpenSubKey(appKeyName))
                    {
                        uninstallString = (string)appKey.GetValue("UninstallString");
                        DisplayName = (string)appKey.GetValue("DisplayName");
                        if (uninstallString.Contains("Qiqqa.application") && (DisplayName == "Qiqqa" || DisplayName == "QiqqaTest"))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    return uninstallString;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        #endregion "GetTheUninstallInfo"

    }
}
