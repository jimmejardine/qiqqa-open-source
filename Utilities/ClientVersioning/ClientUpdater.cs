using System;
using System.Net;
using System.Text;
using Utilities.Files;
using Utilities.GUI;
using Utilities.Internet;
using Utilities.Misc;
using Qiqqa.Common.Configuration;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace Utilities.ClientVersioning
{
    /// <summary>
    /// Talks to the server to figure out what the server says the latest version is, and suggests an update if required.
    /// </summary>
    public class ClientUpdater
    {
        public static ClientUpdater Instance;

        private ClientVersionInformation _latestClientVersionInformation;
        private readonly string _appName;
        private readonly string _downloadLogo;
        private readonly string _clientVersionUrl;
        private readonly string _clientSetupUrl;
        private readonly bool _isTestEnvironment;
        private readonly Action<string> _showReleaseNotesDelegate;

        public static void Init(string appName, string downloadLogo, string clientVersionUrl, string clientSetupUrl, bool isTestEnvironment, Action<string> showReleaseNotesDelegate)
        {
            Instance = new ClientUpdater(appName, downloadLogo, clientVersionUrl, clientSetupUrl, isTestEnvironment, showReleaseNotesDelegate);
        }

        private ClientUpdater(string appName, string downloadLogo, string clientVersionUrl, string clientSetupUrl, bool isTestEnvironment, Action<string> showReleaseNotesDelegate)
        {
            _appName = appName;
            _downloadLogo = downloadLogo;
            _clientVersionUrl = clientVersionUrl;
            _clientSetupUrl = clientSetupUrl;
            _isTestEnvironment = isTestEnvironment;
            _showReleaseNotesDelegate = showReleaseNotesDelegate;
        }

        public void CheckForNewClientVersion(IWebProxy proxy)
        {
            try
            {
                Logging.Info("About to check for new client version at server: {0}", _clientVersionUrl);
                string temp_file = null;
                try
                {
                    temp_file = DownloadFile(proxy, _clientVersionUrl);

                    //  deserialize into memory
                    ClientVersionInformation client_version_information = XmlSerializeFile.Deserialize<ClientVersionInformation>(temp_file);
                    if (client_version_information != null)
                    {
                        Logging.Info("Received latest client version information from server: {0}", client_version_information);

                        ProcessClientVersionInformation(client_version_information);
                    }
                    else
                    {
                        Logging.Info("Unable to deserialize client version information from server, nothing to do.");
                    }
                }
                finally
                {
                    //  clean up the client version xml file
                    if (!string.IsNullOrEmpty(temp_file))
                    {
                        //  this method doesn't throw an exception
                        FileTools.Delete(temp_file);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Problem checking for new client version at URL: {0}", _clientVersionUrl);
            }
        }

        private void ProcessClientVersionInformation(ClientVersionInformation client_version_information)
        {
            _latestClientVersionInformation = client_version_information;

            int current_executing_version = ClientVersion.CurrentVersion;

            string notification_bar_text;
            string notification_bar_tooltip;
            NotificationManager.NotificationType notification_type;

            //  are we on the latest version? whohooooo
            if (current_executing_version == client_version_information.LatestVersion)
            {
                Logging.Info("We are on the latest version: {0}", current_executing_version);
                //  nothing left to do
                return;
            }

            //  is this version ok - based on the compliant version flag, in which case nothing to do except log
            if (client_version_information.CompliantFromVersion.HasValue && current_executing_version >= client_version_information.CompliantFromVersion)
            {
                Logging.Info("User is running an old version (v{0}), but it is still >= the compliant version (v{1}), so not telling them to update.", current_executing_version, client_version_information.CompliantFromVersion);
                //return;
            }

            //  are we ahead of the latest version (in the event we have rolled back, let the user know)
            if (current_executing_version > client_version_information.LatestVersion)
            {
                Logging.Warn("Current executing version (v{0}) is ahead of server version (v{1})??? Have we rolled back a version?", current_executing_version, client_version_information.LatestVersion);
                notification_bar_text = "We have reverted to an old version of " + _appName + ", please download the previous version.";
                notification_bar_tooltip = "It appears that we have reverted to an old version of " + _appName + ", most likely due to some issues with the latest version.  Please download LexLens again and reinstall (the installer will warn you that you are installing an old version which you can safely ignore).";
                notification_type = NotificationManager.NotificationType.Warning;
            }
            //  is this version obsolete - let them know with a strongly worded message
            else if (client_version_information.ObsoleteToVersion.HasValue && current_executing_version <= client_version_information.ObsoleteToVersion)
            {
                notification_bar_text = "Your " + _appName + " version is out-of-date, please download the latest version.";
                notification_bar_tooltip = "You are using an old version of " + _appName + " which is not currently supported, please download the latest version.";
                notification_type = NotificationManager.NotificationType.Warning;
            }
            //  just a bog standard update message
            else
            {
                notification_bar_text = "A new version of " + _appName + " is available for download.";
                notification_bar_tooltip = null;
                notification_type = NotificationManager.NotificationType.Warning;
            }

            //  should we notify them about a new version available - only do if their version isn't compliant
            if (!client_version_information.CompliantFromVersion.HasValue || current_executing_version < client_version_information.CompliantFromVersion)
            {
                WPFDoEvents.ResetHourglassCursor();

                NotificationManager.Instance.AddPendingNotification(
                    new NotificationManager.Notification(
                        notification_bar_text,
                        notification_bar_tooltip,
                        notification_type,
                        _downloadLogo, //Icons.lexlens_logo,
                        "See Changes",
                        ViewChanges,
                        "Download",
                        DownloadNewClientVersion
                        ));
            }
        }

        /// <summary>
        /// This is invoked when the user clicks on the notification bar "Download" button.
        /// </summary>
        public void DownloadNewClientVersion(object obj)
        {
            try
            {
                if (_latestClientVersionInformation == null)
                {
                    Logging.Warn("No latest client information to work with???  Cannot download.");
                    return;
                }

                //  for the moment, just use the first download location
                string download_source = _latestClientVersionInformation.DownloadLocations[0].Trim();
                Logging.Info("Download source in update file is {0}", download_source);
                if (0 == download_source.CompareTo("@WEB_BASE@"))
                {
                    download_source = _clientSetupUrl;
                }
                Logging.Info("Download source that we will use is {0}", download_source);


                if (_isTestEnvironment)
                {
                    download_source = download_source.Replace("www.", "www.test.");
                    Logging.Warn("Modified download source to use test.lexlens.com: " + download_source);
                }

                Logging.Info("Kicking off browser to download the latest client version from: {0}", download_source);
                BrowserStarter.OpenBrowser(download_source);
                Logging.Info("Finished starting browser to download latest client.");
            }
            catch (Exception e)
            {
                Logging.Error(e, "Problem kicking off download for new client version");
            }
        }

        /// <summary>
        /// Shows a simple modal list of release notes.
        /// </summary>
        public void ViewChanges(object dummy = null)
        {
            try
            {
                string release_notes;

                if (_latestClientVersionInformation != null)
                {
                    release_notes = _latestClientVersionInformation.ReleaseNotes;
                    if (string.IsNullOrEmpty(release_notes))
                    {
                        Logging.Warn("No release notes from server, nothing to do");
                        return;
                    }
                }
                else
                {
                    const string ChangelogFilename = Path.GetFullPath(Path.Combine(ConfigurationManager.Instance.StartupDirectoryForQiqqa, @"CHANGELOG.md"));

                    release_notes = File.ReadAllText(ChangelogFilename, Encoding.UTF8);
                }
                release_notes = release_notes.Trim();

                //  switch to the gui thread
                WPFDoEvents.InvokeInUIThread(() => _showReleaseNotesDelegate(release_notes));
            }
            catch (Exception e)
            {
                Logging.Error(e, "Problem viewing changes for new client version");
            }
        }

        /// <summary>
        /// Downloads the given type of file from our server and returns the path to the temp file.
        /// Please clean up the temp file once done.
        /// </summary>
        public static string DownloadFile(IWebProxy proxy, string url)
        {
            using (WebClient web_client = new WebClient())
            {
                web_client.Proxy = proxy;
                string temp_file = TempFile.GenerateTempFilename("tmp");
                web_client.DownloadFile(url, temp_file);
                return temp_file;
            }
        }

    }
}
