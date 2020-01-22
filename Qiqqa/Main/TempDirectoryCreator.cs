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
            CreateDirectoryIfNonExistent();
        }

        public static bool CreateDirectoryIfNonExistent()
        {
            try
            {
                if (!Directory.Exists(TempFile.TempDirectoryForQiqqa))
                {
                    Directory.CreateDirectory(TempFile.TempDirectoryForQiqqa);

                    return CheckTempExists();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
            }
            return false;
        }

        public static bool CheckTempExists()
        {
            return Directory.Exists(TempFile.TempDirectoryForQiqqa);
        }
    }
}
