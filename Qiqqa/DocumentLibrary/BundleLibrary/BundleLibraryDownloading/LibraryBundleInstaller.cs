using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities;
using Utilities.GUI;
using File = Alphaleonis.Win32.Filesystem.File;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Qiqqa.DocumentLibrary.BundleLibrary.BundleLibraryDownloading
{
    class LibraryBundleInstaller
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
            Process zip_process = Process.Start(ConfigurationManager.Instance.Program7ZIP, parameters);
            zip_process.WaitForExit(10000);
            
            // Reflect this new bundle
            WebLibraryDetail new_web_library_detail = WebLibraryManager.Instance.UpdateKnownWebLibraryFromBundleLibraryManifest(manifest);

            Application.Current.Dispatcher.Invoke(((Action)(() =>
                MainWindowServiceDispatcher.Instance.OpenLibrary(new_web_library_detail.library)
            )));
        }
    }
}
