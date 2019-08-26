using System;
using System.Deployment.Application;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Utilities;

namespace Qiqqa.StartPage
{
    public partial class AboutControl : UserControl
    {
        public AboutControl()
        {
            InitializeComponent();
            lblCopyrightYear.Text = DateTime.Now.Year.ToString();
            
            string post_version_type = ApplicationDeployment.IsNetworkDeployed ? "o" : "s";
            lblVersion.Text = "v." + ClientVersion.CurrentVersion + post_version_type;
            lblFullVersion.Text = "v." + ClientVersion.CurrentBuild + post_version_type;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (sender == lnkIconsFreeDigitalPhotos)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_FreeDigitalPhotos);
            }
            else if (sender == lnkIconsTango)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_IconsTango);
            }
            else if (sender == lnkIconsBuuf)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_IconsBuuf);
            }
            else if (sender == lnkSorax)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_Sorax);
            }
            else if (sender == lnkRedgate)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_Redgate);
            }
            else if (sender == lnkTesseract)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_Tesseract);
            }
            else if (sender == lnkWpfToolkit)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_WpfToolkit);
            }
            else if (sender == lnkSplashScreen)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_WiseWanderer);
            }
            else if (sender == lnkCiteProc)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_CiteProc);
            }
            else if (sender == lnkCSL || sender == lnkCSLProject)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_CSLProject);
            }
            else if (sender == lnkAvalonEdit)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_AvalonEdit);
            }
            else if (sender == lnkZoteroCSL)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_ZoteroCSLRepository);
            }
            else if (sender == lnkCSLGithub)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser(WebsiteAccess.Url_CSLGithub);
            }
            else if (sender == lnkQiqqaWebsite)
            {
                WebsiteAccess.OpenWebsite(WebsiteAccess.OurSiteLinkKind.Home);
            }
            else if (sender == lnkIconsVisualPharm)
            {
                WebsiteAccess.OpenWebsite(WebsiteAccess.Url_IconsVisualPharm);
            }
            else if (sender == lnkIconsGlyphicons)
            {
                WebsiteAccess.OpenWebsite(WebsiteAccess.Url_Glyphicons);
            }
            else if (sender == lnkGecko)
            {
                WebsiteAccess.OpenWebsite(WebsiteAccess.Url_Gecko);
            }
            else if (sender == lnkXULRunner)
            {
                WebsiteAccess.OpenWebsite(WebsiteAccess.Url_XULRunner);
            }
            else if (sender == lnkLicenses)
            {
                MainWindowServiceDispatcher.Instance.OpenLicensesDirectory();
            }
            else if (sender == lnkForums)
            {
                WebsiteAccess.OpenWebsite(WebsiteAccess.Url_Forums);
            }
            else
            {
                Logging.Error("Unknown about screen hyperlink " + sender);
            }
        }

        private void Feedback_Click(object sender, RoutedEventArgs e)
        {
            WebsiteAccess.OpenWebsite(WebsiteAccess.OurSiteLinkKind.Feedback);
        }
    }
}
