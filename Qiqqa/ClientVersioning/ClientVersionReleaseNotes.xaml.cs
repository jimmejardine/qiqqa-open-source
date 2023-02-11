using System.Deployment.Application;
using System.Windows;
using icons;
using Utilities;

namespace Qiqqa.ClientVersioning
{
    /// <summary>
    /// Displays the release notes we have loaded from the server.
    /// </summary>
    public partial class ClientVersionReleaseNotes
    {
        public ClientVersionReleaseNotes(string release_notes)
        {
            InitializeComponent();

            Header.Img = Icons.GetAppIcon(Icons.Upgrade);
            TextBlock.Text = release_notes;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                DownloadButton.Visibility = Visibility.Collapsed;
            }

            DownloadButton.Icon = Icons.GetAppIcon(Icons.Upgrade);
            DownloadButton.Caption = "Download";

            CloseButton.Icon = Icons.GetAppIcon(Icons.Cancel);
            CloseButton.Caption = "Cancel";
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Logging.Info("User clicked to download from the release notes window");
            ClientUpdater.Instance.DownloadNewClientVersion();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
