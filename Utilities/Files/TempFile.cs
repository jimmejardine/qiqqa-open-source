using System;
using System.IO;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Utilities.Files
{
    public static class TempFile
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="extension">Do not include a . in the extension</param>
        /// <returns></returns>
        public static string GenerateTempFilename(string extension)
        {
            return Path.GetFullPath(String.Format("{0}\\TempFile.{1}.{2}", TempDirectoryForQiqqa, Guid.NewGuid().ToString(), extension));
        }

        public static string GenerateNamedTempFilename(string name, string extension)
        {
            return Path.GetFullPath(String.Format("{0}\\{3}.{1}.{2}", TempDirectoryForQiqqa, Guid.NewGuid().ToString(), extension, name));
        }

        public static string TempDirectoryForQiqqa => Path.GetFullPath(Path.Combine(Path.GetTempPath(), @"Qiqqa"));
    }
}
