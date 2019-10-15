using System;
using System.IO;
using Utilities;
using File = Alphaleonis.Win32.Filesystem.File;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Qiqqa.UpgradePaths.V012To013
{
    class Upgrade
    {
        internal static void RunUpgrade()
        {
            Logging.Info("Upgrading from 012 to 013");
            MoveQiqqaLibraryFromRoamingToLocalProfile();
        }

        private static void MoveQiqqaLibraryFromRoamingToLocalProfile()
        {
            string folder_local_qiqqa = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Quantisle/Qiqqa"));
            string folder_roaming_qiqqa = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Quantisle/Qiqqa"));

            string folder_local_quantisle = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Quantisle"));

            // First make sure that the Quantisle directory is available in the local profile directory
            if (!Directory.Exists(folder_local_quantisle))
            {
                Directory.CreateDirectory(folder_local_quantisle);            
            }

            // Output some useful stats
            if (Directory.Exists(folder_roaming_qiqqa)) Logging.Info("Roaming Qiqqa directory exists");
            if (Directory.Exists(folder_local_qiqqa)) Logging.Info("Local Qiqqa directory exists");
            
            // If a roaming Qiqqa directory exists, move it to the local directory
            if (Directory.Exists(folder_roaming_qiqqa) && !Directory.Exists(folder_local_qiqqa))
            {
                Logging.Info("Moving Qiqqa directory from Roaming to Local");
                try
                {
                    Directory.Move(folder_roaming_qiqqa, folder_local_qiqqa);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "Error moving Qiqqa directory from Roaming to Local");
                }
            }
            else
            {
                Logging.Info("NOT moving Qiqqa directory from Roaming to Local");
            }
        }
    }
}
