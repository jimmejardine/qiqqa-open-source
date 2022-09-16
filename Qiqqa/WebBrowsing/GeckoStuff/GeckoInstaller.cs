using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Qiqqa.Common.Configuration;
using Utilities;
using Utilities.ProcessTools;
using Utilities.Strings;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.WebBrowsing.GeckoStuff
{
    public static class GeckoInstaller
    {
        private static readonly Lazy<string> XULPackageFilename =  new Lazy<string>(() => Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"xulrunner-33.1.1.en-US.win32.zip")));
        private static readonly Lazy<string> UnpackDirectoryDirectory =  new Lazy<string>(() => Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.BaseDirectoryForQiqqa, @"xulrunner-33")));

        internal static void CheckForInstall()
        {
            bool should_install = false;

            if (!Directory.Exists(InstallationDirectory))
            {
                Logging.Info("XULRunner directory {0} does not exist, so installing it.", InstallationDirectory);
                should_install = true;
            }
            else
            {
                IEnumerable<string> directory_contents = Directory.EnumerateFiles(InstallationDirectory, "*.*", SearchOption.AllDirectories);
                int directory_contents_count = directory_contents.Count();
                if (46 != directory_contents_count)
                {
                    string directory_contents_string = StringTools.ConcatenateStrings(directory_contents, "\n\t");
                    Logging.Warn("XULRunner directory {0} does not contain all necessary files (only {2} files), so reinstalling it.  The contents were:\n\t{1}", InstallationDirectory, directory_contents_string, directory_contents_count);
                    should_install = true;
                }
            }

            if (should_install)
            {
                Logging.Info("Installing XULRunner into {0}.", InstallationDirectory);
                Directory.CreateDirectory(InstallationDirectory);

                // STDOUT/STDERR
                string process_parameters = String.Format("x -y \"{0}\" -o\"{1}\"", XULPackageFilename.Value, UnpackDirectoryDirectory.Value);
                using (Process process = ProcessSpawning.SpawnChildProcess(ConfigurationManager.Instance.Program7ZIP, process_parameters, ProcessPriorityClass.Normal))
                {
                    using (ProcessOutputReader process_output_reader = new ProcessOutputReader(process))
                    {
                        process.WaitForExit();

                        Logging.Info("XULRunner installer:\n{0}", process_output_reader.GetOutputsDumpString());
                    }
                }

                Logging.Info("XULRunner installed.");
            }
        }

        private static readonly Lazy<string> __InstallationDirectory = new Lazy<string>(() => Path.GetFullPath(Path.Combine(UnpackDirectoryDirectory.Value, @"xulrunner")));
        public static string InstallationDirectory => __InstallationDirectory.Value;
    }
}
