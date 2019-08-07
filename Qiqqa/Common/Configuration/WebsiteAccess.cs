using System;
using System.Deployment.Application;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Qiqqa.Common.WebcastStuff;
using Utilities;
using Utilities.Files;
using Utilities.Internet;

namespace Qiqqa.Common.Configuration
{
    public class WebsiteAccess // : Utilities.Internet.WebsiteAccess
    {
        static WebsiteAccess()
        {
            ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
        }

        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            //if (cert.Subject.ToUpper().Contains("YourServerName"))
            return true;
        }

        public static readonly string Url_AmazonS3 = @"http://aws.amazon.com/s3/";
        public static readonly string Url_Webcasts = @"http://www.youtube.com/user/QiqqaTips";
        public static readonly string Url_ZoteroCSLRepository = @"https://www.zotero.org/styles/";
        public static readonly string Url_CSLManual = @"http://citationstyles.org/downloads/specification.html";
        public static readonly string Url_CSLAbout = @"http://editor.citationstyles.org/about/";

        public static readonly string Url_Forums = @"http://getsatisfaction.com/qiqqa";

        public static readonly string Url_Datacopia = @"https://www.youtube.com/watch?v=NnNm_aqYUrQ";
        public static readonly string Url_Omnipatents = @"http://www.omnipatents.com";
        public static readonly string Url_PosterPack = @"http://www.qiqqa.com/Redir/Download/posterpack";

        public static readonly string Url_AlternativeTo = @"http://alternativeto.net/software/qiqqa/";

        public static readonly string Url_FreeDigitalPhotos = @"http://www.freedigitalphotos.net";
        public static readonly string Url_IconsTango = @"http://commons.wikimedia.org/wiki/User:Inductiveload/Tango";
        public static readonly string Url_IconsBuuf = @"http://mattahan.deviantart.com/";
        public static readonly string Url_Sorax = @"http://www.soraxsoft.com/index.html";
        public static readonly string Url_Redgate = @"http://www.red-gate.com/";
        public static readonly string Url_Tesseract = @"http://sourceforge.net/projects/tesseract-ocr/";
        public static readonly string Url_WpfToolkit = @"http://wpf.codeplex.com/";
        public static readonly string Url_WiseWanderer = @"http://wisewanderer.deviantart.com/";
        public static readonly string Url_CiteProc = @"https://bitbucket.org/fbennett/citeproc-js/wiki/Home/";
        public static readonly string Url_CSLProject = @"http://citationstyles.org/";
        public static readonly string Url_AvalonEdit = @"http://wiki.sharpdevelop.net/AvalonEdit.ashx";
        public static readonly string Url_CSLGithub = @"https://github.com/citation-style-language/styles";
        public static readonly string Url_IconsVisualPharm = @"http://www.visualpharm.com/";
        public static readonly string Url_Glyphicons = @"http://glyphicons.com/";
        public static readonly string Url_BlankWebsite = @"http://www.blankwebsite.com/";
        public static readonly string Url_Gecko = @"http://code.google.com/p/geckofx/";
        public static readonly string Url_XULRunner = @"https://developer.mozilla.org/en/XULRunner";
        public static readonly string Url_AdobeAcrobatDownload = @"http://get.adobe.com/reader/";
        public static readonly string Url_LuceneQuerySyntax = @"http://lucene.apache.org/core/old_versioned_docs/versions/2_9_1/queryparsersyntax.html";
        public static readonly string Url_GoogleScholarBibTeXPreferences = @"http://scholar.google.com/scholar_setprefs?scis=yes&scisf=4";

        public static readonly string Url_ChatAPI4Qiqqa = @"http://chatapi.qiqqa.com:8080";

        public static readonly string Url_QiqqaLibraryExportTrackReference = @"http://www.qiqqa.com/?ref=EXPHTML";
        public static readonly string Url_GoogleAnalyticsTracker4Qiqqa = @"http://www.google-analytics.com/collect";
        // dictionary tracker search URL has 1 parameter: the word you look up:
        public static readonly string Url_ThesaurusSearch = @"http://dictionary.reference.com/browse/{0}";
        // TODO: replace URL with https://www.dictionary.com/browse/{0} (which the above one maps to anyway)

        // search URL has 1 parameter:
        public static readonly string Url_DatacopiaSearch = @"https://datacopia.com?data={0}";

        public static readonly string Url_GithubRepo4Qiqqa = @"https://github.com/jimmejardine/qiqqa-open-source";

        // bibtexsearch.com search URL has 1 parameter: the server ID/number (1..4):
        public static readonly string Url_BibTeXSearchServerN = @"http://search{0}.bibtexsearch.com:80";
        public static readonly string Url_BibTeXSearch_Submit = @"http://submit.bibtexsearch.com:80/submit";

        // Twitter Posting URL: takes 1 parameter (the message text):
        public static readonly string Url_TwitterTweetSubmit = @"http://twitter.com/intent/tweet?text={0}";

        public static readonly string Url_OliVideo = Webcasts.PLAY.url;
        public static readonly string Url_WebCastMcKillop = Webcasts.EXTERNAL_BASICS.url;
        
        public enum OurSiteLinkKind
        {
            Home,
            Welcome,
            Feedback,
            Tutorial, //Full website version
            Help,
            Faq,
            Team,
            Champion,
            Premium,
            PremiumForFree,
            PremiumTopUp,
            Datacopia,
            Omnipatents,
            PosterPack
        }

        public enum OurSiteFileKind
        {
            ClientVersion,
            ClientSetup
        }

        public static void OpenWebsite(string url)
        {
            BrowserStarter.OpenBrowser(url);
        }

        public static void OpenWebsite(OurSiteLinkKind linkType)
        {
            OpenWebsite(GetOurUrl(linkType));
        }

        public static void OpenOffsiteUrl(string finalUrl)
        {
            OpenWebsite(GetOffsiteUrl(finalUrl));
        }

        /// <summary>
        /// Downloads the given type of file from our server and returns the path to the temp file.
        /// Please clean up the temp file once done.
        /// </summary>
        public static string DownloadFile(OurSiteFileKind fileType)
        {
            using (WebClient web_client = new WebClient())
            {
                web_client.Proxy = ConfigurationManager.Instance.Proxy;
                string temp_file = TempFile.GenerateTempFilename("tmp");
                web_client.DownloadFile(GetOurFileUrl(fileType), temp_file);
                return temp_file;
            }
        }

        /// <summary>
        /// Gets the url for the api
        /// </summary>
        /// <returns></returns>
        public static string GetApiUrl(bool disable_ssl)
        {
            string protocol = disable_ssl ? "http://" : "https://";
            return protocol + "api." + GetWebRoot() + "/api.asmx";
        }

        public static void OpenWebLibrary(string web_library_short_id)
        {
            string url = GetOurUrl(OurSiteLinkKind.Home) + String.Format("Library/{0}/Documents", web_library_short_id);
            OpenWebsite(url);
        }

        public static void EditOrDeleteLibrary(string web_library_short_id)
        {
            string url = GetOurUrl(OurSiteLinkKind.Home) + String.Format("Library/{0}/Settings", web_library_short_id);
            OpenWebsite(url);
        }

        public static void ChangeLibraryPublicStatus(string web_library_short_id)
        {
            string url = GetOurUrl(OurSiteLinkKind.Home) + String.Format("Library/{0}/Settings", web_library_short_id);
            OpenWebsite(url);
        }

        public static void InviteFriendsToWebLibrary(string web_library_short_id)
        {
            string url = GetOurUrl(OurSiteLinkKind.Home) + String.Format("Library/{0}/Members", web_library_short_id);
            OpenWebsite(url);
        }

        public static void TopUpWebLibrary(string web_library_short_id)
        {
            string url = GetOurUrl(OurSiteLinkKind.Home) + String.Format("Library/{0}/Storage", web_library_short_id);
            OpenWebsite(url);
        }

        public static bool IsTestEnvironment
        {
            get
            {
                return GetWebRoot().Contains("test.qiqqa.com");
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the url of the root of the main website. 
        /// </summary>
        /// <returns></returns>
        private static string GetWebsiteUrl()
        {
            return "http://www." + GetWebRoot();
        }
        private static string GetDownloadWebsiteUrl()
        {
            return "http://download." + GetWebRoot();
        }

        private static string GetWebRoot()
        {
            // First check if there is anything in the registry that overrides this source
            string local_web_root_override = RegistrySettings.Instance.Read(RegistrySettings.UrlWebRoot);
            if (!String.IsNullOrEmpty(local_web_root_override))
            {
                return local_web_root_override;
            }

            // Otherwise try to get it from the deployment descriptor
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                string update_location = ApplicationDeployment.CurrentDeployment.UpdateLocation.GetLeftPart(UriPartial.Authority);
                //Logging.Info("The ApplicationDeployment.CurrentDeployment.UpdateLocation is {0}", update_location);

                int firstDot = update_location.IndexOf('.');
                if (firstDot < 0)
                {
                    Logging.Warn("We were expecting a . in the update location, but there was none!");
                    return update_location; // Should never happen
                }

                return update_location.Substring(firstDot + 1);
            }
            
            return "qiqqa.com";
        }

        public static string GetOurUrl(OurSiteLinkKind linkType)
        {
            switch (linkType)
            {
                case OurSiteLinkKind.Home:
                    return GetWebsiteUrl() + "/";

                case OurSiteLinkKind.Welcome:
                    return GetWebsiteUrl() + "/Home/Welcome";

                case OurSiteLinkKind.Feedback:
                    return GetWebsiteUrl() + "/Feedback";

                case OurSiteLinkKind.Tutorial:
                    return GetWebsiteUrl() + "/Help/Tutorial";

                case OurSiteLinkKind.Help:
                    return GetWebsiteUrl() + "/Help";

                case OurSiteLinkKind.Faq:
                    return GetWebsiteUrl() + "/Faq";

                case OurSiteLinkKind.Team:
                    return GetWebsiteUrl() + "/About/Team";

                case OurSiteLinkKind.Champion:
                    return GetWebsiteUrl() + "/Champion";

                case OurSiteLinkKind.Datacopia:
                    return Url_Datacopia;
                case OurSiteLinkKind.Omnipatents:
                    return Url_Omnipatents;
                case OurSiteLinkKind.PosterPack:
                    return Url_PosterPack;
                case OurSiteLinkKind.Premium:
                    return GetWebsiteUrl() + "/About/PremiumMembership";

                case OurSiteLinkKind.PremiumForFree:
                    return GetWebsiteUrl() + "/About/PremiumForFree";

                case OurSiteLinkKind.PremiumTopUp:
                    return GetWebsiteUrl() + "/Account/PremiumTopUp";

                default:
                    throw new NotImplementedException();
            }
        }

        public static string GetPremiumUrl(string token)
        {
            return GetWebsiteUrl() + "/About/PremiumMembership/" + token;
        }

        public static string GetPremiumPlusUrl(string token)
        {
            return GetWebsiteUrl() + "/About/PremiumPlus/" + token;
        }

        /// <summary>
        /// So we can track directly in google analytics
        /// </summary>
        public static string GetOffsiteUrl(string finalUrl)
        {
            return GetWebsiteUrl() + "/Redir/Go/?src=app&url=" + Uri.EscapeDataString(finalUrl);
        }


        public static string GetOurFileUrl(OurSiteFileKind fileType)
        {
            switch (fileType)
            {
                case OurSiteFileKind.ClientVersion:
                    return GetDownloadWebsiteUrl() + "/Content/Client/ClientVersion.xml";
                case OurSiteFileKind.ClientSetup:
                    return GetDownloadWebsiteUrl() + "/Content/Client/setup.exe";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
