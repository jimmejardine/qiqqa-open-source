using System;
using System.IO;
using System.Threading;
using System.Windows;
using icons;
using Microsoft.Win32;
using Qiqqa.Common.Configuration;
using Qiqqa.Common.GUI;
using Qiqqa.DocumentLibrary.BundleLibrary.BundleLibraryDownloading;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;
using Utilities.Internet;
using Utilities.Misc;

namespace Qiqqa.DocumentLibrary.BundleLibrary.LibraryBundleDownloading
{
    /// <summary>
    /// Interaction logic for BundleLibraryJoiningControl.xaml
    /// </summary>
    public partial class BundleLibraryJoiningControl : StandardWindow
    {
        public BundleLibraryJoiningControl()
        {
            InitializeComponent();
            
            this.Title = 
            this.Header.Caption =  "Download Bundle Library";

            this.Header.SubCaption = "Please confirm the details of the Bundle Library you wish to download.";
            this.Header.Img = Icons.GetAppIcon(Icons.WebLibrary_BundleLibrary);

            this.ButtonCancel.Caption = "Cancel";
            this.ButtonCancel.Icon = Icons.GetAppIcon(Icons.Cancel);
            this.ButtonCancel.Click += ButtonCancel_Click;

            this.ButtonDownload.Caption = "Download";
            this.ButtonDownload.Icon = Icons.GetAppIcon(Icons.WebLibrary_BundleLibrary);
            this.ButtonDownload.Click += ButtonDownload_Click;

            ObjButtonManifestFilenameChoose.Click += ObjButtonManifestFilenameChoose_Click;
        }

        void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            FeatureTrackingManager.Instance.UseFeature(Features.StartPage_JoinBundleLibrary);

            BundleLibraryManifest manifest = this.DataContext as BundleLibraryManifest ;
            
            if (null == manifest)
            {
                MessageBoxes.Error("Please select a Bundle Library manifest file (*.qiqqa_bundle_manifest).");
                return;
            }

            // Kick off the downloader
            SafeThreadPool.QueueUserWorkItem(o => ManageDownload(manifest));

            this.Close();
        }

        private void ManageDownload(BundleLibraryManifest manifest)
        {
            string url = manifest.BaseUrl + "/" + manifest.Id + Common.EXT_BUNDLE;
            UrlDownloader.DownloadAsyncTracker download_async_tracker = UrlDownloader.DownloadWithNonBlocking(ConfigurationManager.Instance.Proxy, url);

            string STATUS_TOKEN = "BundleDownload-" + manifest.Version;

            StatusManager.Instance.ClearCancelled(STATUS_TOKEN);
            while (!download_async_tracker.DownloadComplete)
            {
                if (StatusManager.Instance.IsCancelled(STATUS_TOKEN))
                {
                    download_async_tracker.Cancel();
                    break;
                }

                StatusManager.Instance.UpdateStatusBusy(STATUS_TOKEN, "Downloading Bundle Library...", download_async_tracker.ProgressPercentage, 100, true);
                Thread.Sleep(1000);
            }

            // Check the reason for exiting
            if (false) {}
            else if (download_async_tracker.DownloadDataCompletedEventArgs.Cancelled)
            {
                StatusManager.Instance.UpdateStatus(STATUS_TOKEN, "Cancelled download of Bundle Library.");
            }
            else if (null != download_async_tracker.DownloadDataCompletedEventArgs.Error)
            {
                MessageBoxes.Error(download_async_tracker.DownloadDataCompletedEventArgs.Error, "There was an error during the download of your Bundle Library.  Please try again later or contact {0} for more information.", manifest.SupportEmail);
                StatusManager.Instance.UpdateStatus(STATUS_TOKEN, "Error during download of Bundle Library.");
            }
            else if (null == download_async_tracker.DownloadDataCompletedEventArgs.Result)
            {
                MessageBoxes.Error(download_async_tracker.DownloadDataCompletedEventArgs.Error, "There was an error during the download of your Bundle Library.  Please try again later or contact {0} for more information.", manifest.SupportEmail);
                StatusManager.Instance.UpdateStatus(STATUS_TOKEN, "Error during download of Bundle Library.");
            }
            else
            {
                StatusManager.Instance.UpdateStatus(STATUS_TOKEN, "Completed download of Bundle Library.");
                if (MessageBoxes.AskQuestion("The Bundle Library named '{0}' has been downloaded.  Do you want to install it now?", manifest.Title))
                {
                    LibraryBundleInstaller.Install(manifest, download_async_tracker.DownloadDataCompletedEventArgs.Result);
                }
                else
                {
                    MessageBoxes.Warn("Not installing Bundle Library.");
                }
            }
        }

        void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void ObjButtonManifestFilenameChoose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Bundle Library Manifest files|*.qiqqa_bundle_manifest" + "|" + "All files|*.*";
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;
                if (true == dialog.ShowDialog())
                {
                    string filename = dialog.FileName;
                    FocusOnManifestFilename(filename);
                }
            }

            catch (Exception ex)
            {
                MessageBoxes.Error(ex, "There was a problem with that Bundle Library Manifest file.");
            }
        }

        internal void FocusOnManifestFilename(string filename)
        {
            string json = File.ReadAllText(filename);
            BundleLibraryManifest manifest = BundleLibraryManifest.FromJSON(json);
            FocusOnManifest(manifest, filename);
        }

        internal void FocusOnManifest(BundleLibraryManifest manifest, string filename_prompt)
        {
            TxtManifestFilename.Text = filename_prompt;
            this.DataContext = manifest;
        }
    }
}
