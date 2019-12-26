using System;
using System.Diagnostics;
using System.Windows;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;
using Utilities.GUI;
using Utilities.ProcessTools;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;


namespace Qiqqa.DocumentLibrary.BundleLibrary.BundleLibraryDownloading
{
    internal class LibraryBundleInstaller
    {
        internal static void Install(BundleLibraryManifest manifest, byte[] library_bundle_binary)
        {
            string temp_filename = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(temp_filename, library_bundle_binary);
                Install(manifest, temp_filename);
            }
            finally
            {
                try
                {
                    File.Delete(temp_filename);
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, "There was a problem deleting the temporary library bundle file.");
                }
            }
        }

        internal static void Install(BundleLibraryManifest manifest, string library_bundle_filename)
        {
            Library library = WebLibraryManager.Instance.GetLibrary(manifest.Id);
            if (null != library)
            {
                MessageBoxes.Info("You already have a version of this Bundle Library.  Please ensure you close all windows that use this library after the latest has been downloaded.");
            }

            string library_directory = Library.GetLibraryBasePathForId(manifest.Id);
            Directory.CreateDirectory(library_directory);

            // Unzip the bundle
            string parameters = String.Format("-y x \"{0}\" -o\"{1}\"", library_bundle_filename, library_directory);
            using (Process process = ProcessSpawning.SpawnChildProcess(ConfigurationManager.Instance.Program7ZIP, parameters))
            {
                using (ProcessOutputReader process_output_reader = new ProcessOutputReader(process))
                {
                    process.WaitForExit();

                    Logging.Info("7ZIP Log Bundle Install progress:\n{0}", process_output_reader.GetOutputsDumpString());
                }
            }

            // Reflect this new bundle
            WebLibraryDetail new_web_library_detail = WebLibraryManager.Instance.UpdateKnownWebLibraryFromBundleLibraryManifest(manifest, suppress_flush_to_disk: false);

            WPFDoEvents.InvokeInUIThread(() => {
                MainWindowServiceDispatcher.Instance.OpenLibrary(new_web_library_detail.library);
            });
        }
    }
}
