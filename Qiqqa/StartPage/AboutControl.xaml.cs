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
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (false) { }

            else if (sender == lnkIconsFreeDigitalPhotos)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("http://www.freedigitalphotos.net");
            }
            else if (sender == lnkIconsTango)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("http://commons.wikimedia.org/wiki/User:Inductiveload/Tango");
            }
            else if (sender == lnkIconsBuuf)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("http://mattahan.deviantart.com/");
            }
            else if (sender == lnkSorax)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("http://www.soraxsoft.com/index.html");
            }
            else if (sender == lnkRedgate)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("http://www.red-gate.com/");
            }
            else if (sender == lnkTesseract)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("http://sourceforge.net/projects/tesseract-ocr/");
            }
            else if (sender == lnkWpfToolkit)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("http://wpf.codeplex.com/");
            }
            else if (sender == lnkSplashScreen)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("http://wisewanderer.deviantart.com/");
            }
            else if (sender == lnkCiteProc)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("https://bitbucket.org/fbennett/citeproc-js/wiki/Home/");
            }
            else if (sender == lnkCSL)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("http://citationstyles.org/");
            }
            else if (sender == lnkAvalonEdit)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("http://wiki.sharpdevelop.net/AvalonEdit.ashx");
            }
            else if (sender == lnkZoteroCSL)
            {
                MainWindowServiceDispatcher.Instance.OpenUrlInBrowser("http://www.zotero.org/styles/");
            }
            else if (sender == lnkQiqqaWebsite)
            {
                WebsiteAccess.OpenWebsite(WebsiteAccess.OurSiteLinkKind.Home);
            }
            else if (sender == lnkIconsVisualPharm)
            {
                WebsiteAccess.OpenWebsite("http://www.visualpharm.com/");
            }
            else if (sender == lnkIconsGlyphicons)
            {
                WebsiteAccess.OpenWebsite("http://glyphicons.com/");
            }
            else if (sender == lnkGecko)
            {
                WebsiteAccess.OpenWebsite("http://code.google.com/p/geckofx/");
            }
            else if (sender == lnkXULRunner)
            {
                WebsiteAccess.OpenWebsite("https://developer.mozilla.org/en/XULRunner");
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
