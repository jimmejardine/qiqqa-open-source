using System;
using Qiqqa.Common.Configuration;
using Qiqqa.UpgradePaths.V037To038;
using Utilities;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.UpgradePaths.V043To044
{
    internal class MoveOCRDirs
    {
        internal static void RunUpgrade()
        {
            try
            {
                string OLD = Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.BaseDirectoryForQiqqa, @"Temp"));
                string NEW = Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.BaseDirectoryForQiqqa, @"ocr"));

                if (!Directory.Exists(NEW))
                {
                    Logging.Info("The NEW OCR directory does not exist.");
                    if (Directory.Exists(OLD))
                    {
                        Logging.Info("The OLD OCR directory does exist.  So moving it!");
                        Directory.Move(OLD, NEW);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "There was a problem upgrading OCR directory.");
            }
        }
    }
}
