using System;
using System.IO;
using System.Text;
using System.Windows;
using icons;
using Qiqqa.Common;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Utilities.GUI;
using Utilities.Internet;
using Utilities.Misc;

namespace Qiqqa.DocumentLibrary.BundleLibrary.BundleLibraryDownloading
{
    internal class BundleLibraryUpdatedManifestChecker
    {
        internal static void Check(WebLibraryDetail web_library_detail)
        {
            WPFDoEvents.AssertThisCodeIs_NOT_RunningInTheUIThread();

            // We can operate only on bundle libs
            if (!web_library_detail.IsBundleLibrary)
            {
                return;
            }

            // Only check every hour
            if (DateTime.UtcNow.Subtract(web_library_detail.LastBundleManifestDownloadTimestampUTC ?? DateTime.MinValue).TotalMinutes < 60)
            {
                return;
            }

            // Flag that we are running this update check now
            web_library_detail.LastBundleManifestDownloadTimestampUTC = DateTime.UtcNow;
            WebLibraryManager.Instance.NotifyOfChangeToWebLibraryDetail();

            // Download the new manifest
            BundleLibraryManifest manifest_existing = BundleLibraryManifest.FromJSON(web_library_detail.BundleManifestJSON);
            string manifest_latest_url = manifest_existing.BaseUrl + @"/" + manifest_existing.Id + Common.EXT_BUNDLE_MANIFEST;
            using (MemoryStream ms = UrlDownloader.DownloadWithBlocking(manifest_latest_url))
            {
                string manifest_latest_json = Encoding.UTF8.GetString(ms.ToArray());
                BundleLibraryManifest manifest_latest = BundleLibraryManifest.FromJSON(manifest_latest_json);

                // It is an old version or we have this version
                if (0 <= String.Compare(manifest_existing.Version, manifest_latest.Version))
                {
                    return;
                }

                // It is a version the user has chosen to ignore
                if (web_library_detail.LastBundleManifestIgnoreVersion == manifest_latest.Version)
                {
                    return;
                }

                BundleLibraryUpdateNotification blun = new BundleLibraryUpdateNotification(web_library_detail, manifest_latest);

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

    internal class BundleLibraryUpdateNotification
    {
        private WebLibraryDetail web_library_detail;
        private BundleLibraryManifest manifest_latest;

        public BundleLibraryUpdateNotification(WebLibraryDetail web_library_detail, BundleLibraryManifest manifest_latest)
        {
            this.web_library_detail = web_library_detail;
            this.manifest_latest = manifest_latest;
        }

        public void Download()
        {
            WPFDoEvents.InvokeAsyncInUIThread(() =>
            {
                MainWindowServiceDispatcher.Instance.ShowBundleLibraryJoiningControl(manifest_latest);
            }
            );
        }

        public void NoThanks()
        {
            web_library_detail.LastBundleManifestIgnoreVersion = manifest_latest.Version;
            WebLibraryManager.Instance.NotifyOfChangeToWebLibraryDetail();
        }
    }
}
