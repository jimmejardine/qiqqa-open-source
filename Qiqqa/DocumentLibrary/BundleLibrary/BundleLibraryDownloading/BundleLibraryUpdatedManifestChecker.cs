using System;
using System.IO;
using System.Text;
using System.Windows;
using icons;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities.Internet;
using Utilities.Misc;

namespace Qiqqa.DocumentLibrary.BundleLibrary.BundleLibraryDownloading
{
    class BundleLibraryUpdatedManifestChecker
    {
        internal static void Check(Library library)
        {
            // We can operate only on bundle libs
            if (!library.WebLibraryDetail.IsBundleLibrary)
            {
                return;
            }

            // Only check every hour
            if (DateTime.UtcNow.Subtract(library.WebLibraryDetail.LastBundleManifestDownloadTimestampUTC ?? DateTime.MinValue).TotalMinutes < 60)
            {
                return;
            }

            // Flag that we are running this update check now
            library.WebLibraryDetail.LastBundleManifestDownloadTimestampUTC = DateTime.UtcNow;
            WebLibraryManager.Instance.NotifyOfChangeToWebLibraryDetail();

            // Download the new manifest
            BundleLibraryManifest manifest_existing = BundleLibraryManifest.FromJSON(library.WebLibraryDetail.BundleManifestJSON);
            string manifest_latest_url = manifest_existing.BaseUrl + "/" + manifest_existing.Id +".qiqqa_bundle_manifest";
            MemoryStream ms;
            UrlDownloader.DownloadWithBlocking(ConfigurationManager.Instance.Proxy, manifest_latest_url, out ms);
            string manifest_latest_json = Encoding.UTF8.GetString(ms.ToArray());
            BundleLibraryManifest manifest_latest = BundleLibraryManifest.FromJSON(manifest_latest_json);

            // It is an old version or we have this version
            if (0 <= String.Compare(manifest_existing.Version, manifest_latest.Version))
            {
                return;
            }

            // It is a version the user has chosen to ignore
            if (library.WebLibraryDetail.LastBundleManifestIgnoreVersion == manifest_latest.Version)
            {
                return;
            }

            {   
                BundleLibraryUpdateNotification blun = new BundleLibraryUpdateNotification(library, manifest_latest);

                NotificationManager.Instance.AddPendingNotification(
                    new NotificationManager.Notification(
                        String.Format("An update is available for your Bundle Library '{0}', from version {1} to {2}.", manifest_latest.Title, manifest_existing.Version, manifest_latest.Version),
                        "Bundle Library update available!",
                        NotificationManager.NotificationType.Info,
                        Icons.LibraryTypeBundle,
                        "Download!",
                        blun.Download,
                        "No thanks!",
                        blun.NoThanks
                    )
                );
            }            
        }
    }

    class BundleLibraryUpdateNotification
    {
        Library library;
        BundleLibraryManifest manifest_latest;

        public BundleLibraryUpdateNotification(Library library, BundleLibraryManifest manifest_latest)
        {
            this.library = library;
            this.manifest_latest = manifest_latest;
        }

        public void Download(object obj)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindowServiceDispatcher.Instance.ShowBundleLibraryJoiningControl(manifest_latest);
            }
            ));
        }

        public void NoThanks(object obj)
        {
            library.WebLibraryDetail.LastBundleManifestIgnoreVersion = manifest_latest.Version;
            WebLibraryManager.Instance.NotifyOfChangeToWebLibraryDetail();
        }
    }
}
