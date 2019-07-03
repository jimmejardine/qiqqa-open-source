using System;
using System.IO;

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
            return String.Format("{0}\\TempFile.{1}.{2}", TempDirectory, Guid.NewGuid().ToString(), extension);
        }

        public static string GenerateNamedTempFilename(string name, string extension)
        {
            return String.Format("{0}\\{3}.{1}.{2}", TempDirectory, Guid.NewGuid().ToString(), extension, name);
        }

        public static string TempDirectory
        {
            get
            {
                return Path.GetTempPath();
            }
        }

    }
}
