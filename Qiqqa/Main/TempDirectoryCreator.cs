using System;
using System.IO;
using Utilities;
using Utilities.Files;
using File = Alphaleonis.Win32.Filesystem.File;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Qiqqa.Main
{
    // Creates a temp directory before anything else runs
    public class TempDirectoryCreator
    {        
        static TempDirectoryCreator()
        {
            try
            {
                if (!Directory.Exists(TempFile.TempDirectory))
                {
                    Directory.CreateDirectory(TempFile.TempDirectory);
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
        }

        public static bool CheckTempExists()
        {
            return Directory.Exists(TempFile.TempDirectory);
        }
    }
}
