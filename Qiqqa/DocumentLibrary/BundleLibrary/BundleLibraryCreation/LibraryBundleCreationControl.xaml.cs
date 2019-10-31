using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.Documents.PDF.CitationManagerStuff;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;
using Utilities.Misc;
using UserControl = System.Windows.Controls.UserControl;

namespace Qiqqa.DocumentLibrary.BundleLibrary.LibraryBundleCreation
{
    /// <summary>
    /// Interaction logic for LibraryBundleCreationControl.xaml
    /// </summary>
    public partial class LibraryBundleCreationControl : UserControl
    {
        public static readonly string TITLE = "Bundle Library Builder";

        Library library = null;
        BundleLibraryManifest manifest = null;

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

        void CmdCreateBundle_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Please select the folder into which the two Library Bundle files should be placed.";
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    CreateBundle(dialog.SelectedPath.ToString());
                }
                else
                {
                    MessageBoxes.Warn("Your Library Bundle creation has been cancelled.");
                }
            }
        }

        void CreateBundle(string target_directory)
        {
            string target_filename_bundle_manifest = Path.GetFullPath(Path.Combine(target_directory, manifest.Id + Common.EXT_BUNDLE_MANIFEST));
            string target_filename_bundle = Path.GetFullPath(Path.Combine(target_directory, manifest.Id + Common.EXT_BUNDLE));

            // Check that the details of the manifest are reasonable
                try
                {
                    new Uri(this.manifest.BaseUrl);
                }
                catch (Exception)
                {
                    MessageBoxes.Warn("Your base URL of '{0}' is invalid.  Please correct it and try again.", this.manifest.BaseUrl);
                    return;
                }

            // Smash out the manifest
            string json = this.manifest.ToJSON();
            File.WriteAllText(target_filename_bundle_manifest, json);

            // Smash out the bundle
            string source_directory = Path.GetFullPath(Path.Combine(library.LIBRARY_BASE_PATH, @"*"));
            string directory_exclusion_parameter = (manifest.IncludesPDFs ? "" : "-xr!documents");
            string parameters = String.Format("a -tzip -mm=Deflate -mmt=on -mx9 \"{0}\" \"{1}\" {2}", target_filename_bundle, source_directory, directory_exclusion_parameter);
            Process zip_process = Process.Start(ConfigurationManager.Instance.Program7ZIP, parameters);

            // Watch the zipper
            SafeThreadPool.QueueUserWorkItem(o => TailZIPProcess(manifest, zip_process));
        }

        private static void TailZIPProcess(BundleLibraryManifest manifest, Process zip_process)
        {
            string STATUS_TOKEN = "Bundle-" + manifest.Version;

            StatusManager.Instance.ClearCancelled(STATUS_TOKEN);

            int iteration = 0;
            while (true)
            {
                ++iteration;

                if (StatusManager.Instance.IsCancelled(STATUS_TOKEN))
                {
                    zip_process.Kill();
                    StatusManager.Instance.UpdateStatus(STATUS_TOKEN, "Cancelled creation of Bundle Library.");
                    return;
                }

                if (zip_process.HasExited)
                {
                    StatusManager.Instance.UpdateStatus(STATUS_TOKEN, "Completed creation of Bundle Library.");
                    return;
                }

                StatusManager.Instance.UpdateStatusBusy(STATUS_TOKEN, "Creating Bundle Library...", iteration, iteration + 1, true);
                
                Thread.Sleep(1000);
            }
        }

        void CmdThemes_Click(object sender, RoutedEventArgs e)
        {
            SafeThreadPool.QueueUserWorkItem(o => library.ExpeditionManager.RebuildExpedition(library.ExpeditionManager.RecommendedThemeCount, true, true, null));
        }

        void CmdAutoTags_Click(object sender, RoutedEventArgs e)
        {
            SafeThreadPool.QueueUserWorkItem(o => this.library.AITagManager.Regenerate(null));
        }

        void CmdCrossReference_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Library_GenerateReferences);
            SafeThreadPool.QueueUserWorkItem(o => CitationFinder.FindCitations(this.library));

        }

        void CmdOCRAndIndex_Click(object sender, RoutedEventArgs e)
        {
            foreach (var pdf_document in library.PDFDocuments)
            {
                pdf_document.Library.LibraryIndex.ReIndexDocument(pdf_document);
            }
        }

        public void ReflectLibrary(Library library_)
        {
            this.library = library_;
            this.manifest = new BundleLibraryManifest();
            
            string bundle_title = library_.WebLibraryDetail.Title + " Bundle Library";
            bundle_title = bundle_title.Replace("Library Bundle Library", "Bundle Library");

            // Set the manifest
            this.manifest.Id = "BUNDLE_" + library_.WebLibraryDetail.Id;
            this.manifest.Version = DateTime.UtcNow.ToString("yyyyMMdd.HHmmss");

            this.manifest.Title = bundle_title;
            this.manifest.Description = library_.WebLibraryDetail.Description;
            
            // GUI updates
            this.DataContext = this.manifest;
            ObjRunLibraryName.Text = library_.WebLibraryDetail.Title;

            ResetProgress();
        }

        private void ResetProgress()
        {
        }
    }
}
