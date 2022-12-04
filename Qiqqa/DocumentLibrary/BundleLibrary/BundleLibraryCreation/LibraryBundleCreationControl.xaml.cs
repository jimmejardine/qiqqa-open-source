using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using icons;
using Ookii.Dialogs.Wpf;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.Documents.PDF.CitationManagerStuff;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;
using Utilities.ProcessTools;
using Utilities.Shutdownable;
using UserControl = System.Windows.Controls.UserControl;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Qiqqa.DocumentLibrary.BundleLibrary.LibraryBundleCreation
{
    /// <summary>
    /// Interaction logic for LibraryBundleCreationControl.xaml
    /// </summary>
    public partial class LibraryBundleCreationControl : UserControl
    {
        public static readonly string TITLE = "Bundle Library Builder";
        private WebLibraryDetail web_library_detail = null;
        private BundleLibraryManifest manifest = null;

        public LibraryBundleCreationControl()
        {
            InitializeComponent();

            CmdOCRAndIndex.Caption = "Force OCR and Indexing";
            CmdOCRAndIndex.Click += CmdOCRAndIndex_Click;

            CmdCrossReference.Caption = "Discover Cross-References";
            CmdCrossReference.Click += CmdCrossReference_Click;

            CmdAutoTags.Caption = "Generate AutoTags";
            CmdAutoTags.Click += CmdAutoTags_Click;

            CmdThemes.Caption = "Find Themes";
            CmdThemes.Click += CmdThemes_Click;

            CmdCreateBundle.Caption = "Create Bundle Library";
            CmdCreateBundle.Icon = Icons.GetAppIcon(Icons.BuildBundleLibrary);
            CmdCreateBundle.CaptionDock = Dock.Bottom;

            CmdCreateBundle.Click += CmdCreateBundle_Click;
        }

        private void CmdCreateBundle_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();

            dialog.Description = "Please select the folder into which the two Library Bundle files should be placed.";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.

            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                MessageBoxes.Warn("Because you are not using Windows Vista or later, the regular folder browser dialog will be used. Please use Windows Vista to see the new dialog.", "Sample folder browser dialog");
            }

            if ((bool)dialog.ShowDialog())
            {
                MessageBoxes.Info($"The selected folder was:{Environment.NewLine}{dialog.SelectedPath}", "Sample folder browser dialog");
                CreateBundle(dialog.SelectedPath);
            }
            else
            {
                MessageBoxes.Warn("Your Library Bundle creation has been canceled.");
            }
        }

        private void CreateBundle(string target_directory)
        {
            string target_filename_bundle_manifest = Path.GetFullPath(Path.Combine(target_directory, manifest.Id + Common.EXT_BUNDLE_MANIFEST));
            string target_filename_bundle = Path.GetFullPath(Path.Combine(target_directory, manifest.Id + Common.EXT_BUNDLE));

            // Check that the details of the manifest are reasonable
            try
            {
                new Uri(manifest.BaseUrl);
            }
            catch (Exception)
            {
                MessageBoxes.Warn("Your base URL of '{0}' is invalid.  Please correct it and try again.", manifest.BaseUrl);
                return;
            }

            // Smash out the manifest
            string json = manifest.ToJSON();
            File.WriteAllText(target_filename_bundle_manifest, json);

            // Smash out the bundle
            string source_directory = Path.GetFullPath(Path.Combine(web_library_detail.LIBRARY_BASE_PATH, @"*"));
            string directory_exclusion_parameter = (manifest.IncludesPDFs ? "" : "-xr!documents");
            string parameters = String.Format("a -tzip -mm=Deflate -mmt=on -mx9 \"{0}\" \"{1}\" {2}", target_filename_bundle, source_directory, directory_exclusion_parameter);

            // Watch the zipper
            SafeThreadPool.QueueUserWorkItem(() => TailZIPProcess(manifest, parameters));
        }

        private static void TailZIPProcess(BundleLibraryManifest manifest, string parameters)
        {
            using (Process zip_process = Process.Start(ConfigurationManager.Instance.Program7ZIP, parameters))
            {
                using (ProcessOutputReader process_output_reader = new ProcessOutputReader(zip_process))
                {
                    string STATUS_TOKEN = "Bundle-" + manifest.Version;

                    StatusManager.Instance.ClearCancelled(STATUS_TOKEN);

                    int iteration = 0;
                    while (true)
                    {
                        ++iteration;

                        if (ShutdownableManager.Instance.IsShuttingDown)
                        {
                            Logging.Error("Canceling creation of Bundle Library due to signaled application shutdown");
                            StatusManager.Instance.SetCancelled(STATUS_TOKEN);
                        }

                        if (StatusManager.Instance.IsCancelled(STATUS_TOKEN))
                        {
                            zip_process.Kill();
                            zip_process.WaitForExit(5000);

                            Logging.Error("Canceled creation of Bundle Library:\n--- Parameters: {0}\n{1}", parameters, process_output_reader.GetOutputsDumpStrings());

                            StatusManager.Instance.UpdateStatus(STATUS_TOKEN, "Canceled creation of Bundle Library.");
                            return;
                        }

                        if (zip_process.HasExited)
                        {
                            Logging.Info("Completed creation of Bundle Library:\n--- Parameters: {0}\n{1}", parameters, process_output_reader.GetOutputsDumpStrings());

                            StatusManager.Instance.UpdateStatus(STATUS_TOKEN, "Completed creation of Bundle Library.");
                            return;
                        }

                        StatusManager.Instance.UpdateStatus(STATUS_TOKEN, "Creating Bundle Library...", cancellable: true);

                        ShutdownableManager.Sleep(3000);
                    }
                }
            }
        }

        private void CmdThemes_Click(object sender, RoutedEventArgs e)
        {
            SafeThreadPool.QueueSafeExecUserWorkItem(() =>
            {
                    web_library_detail.Xlibrary.ExpeditionManager.RebuildExpedition(web_library_detail.Xlibrary.ExpeditionManager.RecommendedThemeCount, true, true, null);
            });
        }

        private void CmdAutoTags_Click(object sender, RoutedEventArgs e)
        {
            SafeThreadPool.QueueSafeExecUserWorkItem(() => web_library_detail.Xlibrary.AITagManager.Regenerate());
        }

        private void CmdCrossReference_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_GenerateReferences);
            SafeThreadPool.QueueSafeExecUserWorkItem(() => CitationFinder.FindCitations(web_library_detail));
        }

        private void CmdOCRAndIndex_Click(object sender, RoutedEventArgs e)
        {
            foreach (var pdf_document in web_library_detail.Xlibrary.PDFDocuments)
            {
                pdf_document.LibraryRef.Xlibrary.LibraryIndex.ReIndexDocument(pdf_document);
            }
        }

        public void ReflectLibrary(WebLibraryDetail library_)
        {
            web_library_detail = library_;
            manifest = new BundleLibraryManifest();

            string bundle_title = web_library_detail.Title + " Bundle Library";
            bundle_title = bundle_title.Replace("Library Bundle Library", "Bundle Library");

            // Set the manifest
            manifest.Id = "BUNDLE_" + web_library_detail.Id;
            manifest.Version = DateTime.UtcNow.ToString("yyyyMMdd.HHmmss");

            manifest.Title = bundle_title;
            manifest.Description = web_library_detail.Description;

            // GUI updates
            DataContext = manifest;
            ObjRunLibraryName.Text = web_library_detail.Title;

            ResetProgress();
        }

        private void ResetProgress()
        {
        }
    }
}
