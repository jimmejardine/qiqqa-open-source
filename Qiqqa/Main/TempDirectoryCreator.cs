using System;
using Utilities;
using Utilities.Files;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.Main
{
    // Creates a temp directory before anything else runs
    public static class TempDirectoryCreator
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
