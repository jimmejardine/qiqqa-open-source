using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using icons;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Qiqqa.DocumentLibrary;
using Qiqqa.DocumentLibrary.WebLibraryStuff;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;
using Utilities.Internet;
using Utilities.Misc;

namespace Qiqqa.WebBrowsing
{
    /// <summary>
    /// Interaction logic for WebBrowserHostControl.xaml
    /// </summary>
    public partial class WebBrowserHostControl : UserControl, IDisposable
    {
        public delegate void BrowserEventDelegate();
        public event BrowserEventDelegate TabChanged;
        public event BrowserEventDelegate PageLoaded;

        public delegate void BrowserNavigatingDelegate(Uri uri);
        public event BrowserNavigatingDelegate Navigating;

        static readonly string TAB_PREFERENCES = "Preferences";
        static readonly string TAB_BROWSING = "Browsing";

        class WebSearcherEntry
        {
            public WebSearcher web_searcher;
            public WebBrowserControl browser_control;
        }

        WebBrowserControl active_wbc = null;

        WebSearcherPreferenceControl web_searcher_preference_control;
        WebBrowserControl wbc_browsing;
        List<WebSearcherEntry> web_searcher_entries = new List<WebSearcherEntry>();

        public WebBrowserHostControl()
        {
            Logging.Info("+WebBrowserHostControl()");

            InitializeComponent();

            bool ADVANCED_MENUS = ConfigurationManager.Instance.ConfigurationRecord.GUI_AdvancedMenus;

            ButtonNewBrowser.Icon = Icons.GetAppIcon(Icons.New);
            if (!ADVANCED_MENUS) ButtonNewBrowser.Caption = "New";
            ButtonNewBrowser.ToolTip = "Open a new browsing window.";
            ButtonNewBrowser.Click += ButtonNewBrowser_Click;

            ButtonBack.Icon = Icons.GetAppIcon(Icons.Back);
            if (!ADVANCED_MENUS) ButtonBack.Caption = "Back";
            ButtonBack.ToolTip = "Browse back";
            ButtonBack.Click += ButtonBack_Click;

            ButtonForward.Icon = Icons.GetAppIcon(Icons.Forward);
            if (!ADVANCED_MENUS) ButtonForward.Caption = "Forward";
            ButtonForward.ToolTip = "Browse forward";
            ButtonForward.Click += ButtonForward_Click;

            TextBoxUrl.ToolTip = "Enter the web address here and press <ENTER>";
            TextBoxUrl.EmptyTextPrompt = "Enter web address";
            TextBoxUrl.OnHardSearch += TextBoxUrl_OnHardSearch;

            TextBoxGoogleScholar.ToolTip = "Enter your search keywords here and press <ENTER>";
            TextBoxGoogleScholar.OnHardSearch += TextBoxGoogleScholar_OnHardSearch;

            ButtonEZProxy.Icon = Icons.GetAppIcon(Icons.WebEZProxy);
            if (!ADVANCED_MENUS) ButtonEZProxy.Caption = "EZProxy";
            ButtonEZProxy.ToolTip = "View this web page using EZProxy.";
            ButtonEZProxy.Click += ButtonEZProxy_Click;

            ButtonPrint.Icon = Icons.GetAppIcon(Icons.Printer);
            if (!ADVANCED_MENUS) ButtonPrint.Caption = "Print";
            ButtonPrint.ToolTip = "Print this web page.";
            ButtonPrint.Click += ButtonPrint_Click;

            ButtonGrabWebPage.Icon = Icons.GetAppIcon(Icons.WebGrabWebPage);
            if (!ADVANCED_MENUS) ButtonGrabWebPage.Caption = "Convert\nto PDF";
            ButtonGrabWebPage.ToolTip = "Convert the current page into a PDF and add it to your library.";
            ButtonGrabWebPage.Click += ButtonGrabWebPage_Click;

            ButtonGrabPDFs.Icon = Icons.GetAppIcon(Icons.WebGrabPDFs);
            if (!ADVANCED_MENUS) ButtonGrabPDFs.Caption = "Grab all\nPDFs";
            ButtonGrabPDFs.ToolTip = "Downloads all the PDFs that are accessible from this web page.";
            ButtonGrabPDFs.Click += ButtonGrabPDFs_Click;
            

            /*
            ButtonAddToLibrary.Icon = Icons.GetAppIcon(Icons.WebAddToLibrary);
            if (!ADVANCED_MENUS) ButtonAddToLibrary.Caption = "Add PDF to\nLibrary";
            ButtonAddToLibrary.ToolTip = "Add the currently displayed page to your library (must be a PDF).";
            ButtonAddToLibrary.Click += new RoutedEventHandler(ButtonAddToLibrary_Click);
            */

            web_searcher_preference_control = new WebSearcherPreferenceControl(this);
            wbc_browsing = new WebBrowserControl(this);

            TabWebBrowserControls.OnActiveItemChanged += TabWebBrowserControls_OnActiveItemChanged;

            RebuildSearchers();

            Logging.Info("-WebBrowserHostControl()");
        }

        void ButtonEZProxy_Click(object sender, RoutedEventArgs e)
        {
            string current_url = active_wbc.CurrentUri.ToString();

            string ezproxy = ConfigurationManager.Instance.ConfigurationRecord.Proxy_EZProxy;
            if (!String.IsNullOrEmpty(ezproxy))
            {
                string new_url = ezproxy.Replace("$@", Uri.EscapeUriString(current_url));
                active_wbc.Navigate(new_url);
            }
            else
            {
                if (MessageBoxes.AskQuestion("Do you not have an EZProxy set.  Do you want to open the Config Screen to set one?"))
                {
                    MainWindowServiceDispatcher.Instance.OpenControlPanel();
                }
            }

            e.Handled = true;
        }

        void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            active_wbc.Print();

            e.Handled = true;
        }

        void ButtonNewBrowser_Click(object sender, RoutedEventArgs e)
        {
            this.OpenNewWindow("http://www.qiqqa.com/Account/Edit");
        }

        public void ForceAdvancedMenus()
        {
            ButtonNewBrowser.Caption = null;
            ButtonBack.Caption = null;
            ButtonForward.Caption = null;
            ButtonGrabWebPage.Caption = null;
            ButtonPrint.Caption = null;
            ButtonEZProxy.Caption = null;
            ButtonGrabPDFs.Caption = null;

            TxtWebAddress.Visibility = Visibility.Collapsed;
            TxtSearchTheWeb.Visibility = Visibility.Collapsed;            
        }

        internal void ForceSnifferSearchers()        
        {
            HashSet<string> once_off_requested_web_searchers = new HashSet<string>();
            once_off_requested_web_searchers.Add(WebSearchers.SCHOLAR_KEY);
            once_off_requested_web_searchers.Add(WebSearchers.PUBMEDXML_KEY);
            RebuildSearchers(once_off_requested_web_searchers);
        }

        private string default_web_searcher_key = null;
        public string DefaultWebSearcherKey
        {
            set
            {
                default_web_searcher_key = value;
            }
        }

        private void DeleteSearchers()
        {
            foreach (var web_searcher_entry in web_searcher_entries)
            {
                TabWebBrowserControls.CloseContent(web_searcher_entry.browser_control);
            }

            web_searcher_entries.Clear();
        }

        internal void RebuildSearchers()
        {
            RebuildSearchers(null);
        }


        internal void RebuildSearchers(HashSet<string> once_off_requested_web_searchers)
        {
            // Clear down the old searchers
            DeleteSearchers();

            // Which are the entries we deffo want?
            HashSet<string> requested_web_searchers = once_off_requested_web_searchers ?? WebSearcherPreferenceManager.Instance.LoadPreferences();

            // Create the new searchers
            foreach (string requested_web_searcher in requested_web_searchers)
            {
                WebSearcher web_searcher = WebSearchers.FindWebSearcher(requested_web_searcher);
                if (null != web_searcher)
                {
                    WebSearcherEntry web_searcher_entry = new WebSearcherEntry();
                    web_searcher_entry.web_searcher = web_searcher;
                    web_searcher_entry.browser_control = new WebBrowserControl(this);
                    web_searcher_entries.Add(web_searcher_entry);
                }
                else
                {
                    Logging.Warn("Unable to find a WebSearcher for key {0}", requested_web_searcher);
                }
            }

            // Add the new searchers
            if (!TabWebBrowserControls.Contains(TAB_PREFERENCES))
            {
                TabWebBrowserControls.AddContent(TAB_PREFERENCES, "", Icons.GetAppIcon(Icons.WebPreferences), false, false, web_searcher_preference_control);
            }

            if (!TabWebBrowserControls.Contains(TAB_BROWSING))
            {
                TabWebBrowserControls.AddContent(TAB_BROWSING, TAB_BROWSING, Icons.GetAppIcon(Icons.ModuleWebBrowser), false, false, wbc_browsing);
            }

            foreach (var web_searcher_entry in web_searcher_entries)
            {
                TabWebBrowserControls.AddContent(web_searcher_entry.web_searcher.key, web_searcher_entry.web_searcher.title, Icons.GetAppIcon(Icons.WebSearch), false, false, web_searcher_entry.browser_control);                
            }

            // Select default
            TabWebBrowserControls.MakeActive(TAB_BROWSING);
        }

        public void GoBibTeXMode()
        {
            TabWebBrowserControls.MakeActive(WebSearchers.SCHOLAR_KEY);
        }

        void ButtonGrabWebPage_Click(object sender, RoutedEventArgs e)
        {
            if (null == CurrentUri)
            {
                MessageBoxes.Error("You need to have navigated to a web page before trying to export to PDF.");
                return;
            }

            string title = CurrentTitle;
            string url = CurrentUri.ToString();

            // This was the code that does the magic locally on the client...
            SafeThreadPool.QueueUserWorkItem(o => HTMLToPDFConversion.GrabWebPage(title, url));
        }

        void TabWebBrowserControls_OnActiveItemChanged(FrameworkElement newItemContent)
        {
            WebBrowserControl wbc = newItemContent as WebBrowserControl;

            if (null != wbc)
            {
                wbc.NavigateToPendingOnceVisibleUri();

                Logging.Info("Active browser control changed");
                active_wbc = wbc;

                Uri uri = wbc.CurrentUri;
                TextBoxUrl.Text = uri == null ? String.Empty : uri.ToString();
            }

            if (null != TabChanged)
            {
                TabChanged();
            }
        }

        /*
        void ButtonAddToLibrary_Click(object sender, RoutedEventArgs e)
        {
            if (null == CurrentUri)
            {
                MessageBoxes.Error("You need to have navigated to a web page before trying to import it into your library.");
                return;
            }

            FeatureTrackingManager.Instance.UseFeature(Features.Web_AddToLibrary);

            string url = CurrentUri.ToString();

            WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary();
            if (null != web_library_detail)
            {
                ImportingIntoLibrary.AddNewDocumentToLibraryFromInternet_ASYNCHRONOUS(web_library_detail.library, url);
            }
        }
        */

        void TextBoxUrl_OnHardSearch()
        {
            DoBrowse();
        }

        void DoBrowse()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Web_Browse);

            string uri = TextBoxUrl.Text;

            // If they are holding down CTRL, add www.XYZ.com to XYZ
            if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
            {
                if (!uri.EndsWith(".com")) uri = uri + ".com";
                if (!uri.StartsWith("www.") && !uri.StartsWith("http")) uri = "www." + uri;
                TextBoxUrl.Text = uri;
            }


            // If they are missing http:// or the like, then add it
            if (uri.Contains('.') && !Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                string http_uri = "http://" + uri;
                if (Uri.IsWellFormedUriString(http_uri, UriKind.Absolute))
                {
                    uri = http_uri;
                    TextBoxUrl.Text = http_uri;
                }
            }

            if (Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                CurrentWebBrowserControl.Navigate(new Uri(uri));
            }
            else
            {
                DoWebSearch(uri);
            }

            // Reselect all text so that it is easy to retype
            TextBoxUrl.SelectAll();
        }

        void TextBoxGoogleScholar_OnHardSearch()
        {
            DoWebSearch();
        }

        internal void SelectSearchTab(string ACTIVE_SEARCH_KEY)
        {
            TabWebBrowserControls.MakeActive(ACTIVE_SEARCH_KEY);
        }

        public void OpenUrl(string url)
        {
            TextBoxUrl.Text = url;
            wbc_browsing.Navigate(url);

            TextBoxUrl.SelectAll();
            TabWebBrowserControls.MakeActive(TAB_BROWSING);
        }

        public WebBrowserControl OpenNewWindow()
        {
            WebBrowserControl wbc = new WebBrowserControl(this);

            string unique_tab_name = Guid.NewGuid().ToString();
            TabWebBrowserControls.AddContent(unique_tab_name, "Browser", Icons.GetAppIcon(Icons.ModuleWebBrowser), true, true, wbc);

            return wbc;
        }

        public WebBrowserControl OpenNewWindow(string url)
        {
            WebBrowserControl wbc = OpenNewWindow();
            wbc.Navigate(url);
            return wbc;
        }

        void DoWebSearch()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Web_Search);

            DoWebSearch(TextBoxGoogleScholar.Text);
        }

        public void DoWebSearch(string search_terms)
        {
            TextBoxGoogleScholar.Text = search_terms;

            foreach (var web_searcher_entry in web_searcher_entries)
            {
                Uri uri = web_searcher_entry.web_searcher.populate_url_template(web_searcher_entry.web_searcher.url_template, search_terms);
                web_searcher_entry.browser_control.NavigateOnceVisible(uri);

                // Make sure the current page is actually going to execute
                if (TabWebBrowserControls.CurrentActiveTabItem == web_searcher_entry.browser_control)
                {
                    web_searcher_entry.browser_control.NavigateToPendingOnceVisibleUri();
                }
            }

            // Make sure we are looking at a search page...
            if (TabWebBrowserControls.CurrentActiveTabItem == wbc_browsing)
            {
                bool used_default_web_searcher = false;
                foreach (var web_searcher_entry in web_searcher_entries)
                {
                    if (default_web_searcher_key == web_searcher_entry.web_searcher.key)
                    {
                        TabWebBrowserControls.MakeActive(web_searcher_entry.web_searcher.key);
                        used_default_web_searcher = true;
                    }
                }

                if (!used_default_web_searcher)
                {
                    if (web_searcher_entries.Count > 0)
                    {
                        TabWebBrowserControls.MakeActive(web_searcher_entries[0].web_searcher.key);
                    }
                }
            }

            TextBoxGoogleScholar.SelectAll();
        }

        internal void ObjWebBrowser_Navigating(WebBrowserControl wbc, Uri uri)
        {
            lblHintHowToUse.Visibility = Visibility.Hidden;

            if (wbc == active_wbc)
            {
                TextBoxUrl.Text = uri.ToString();
                TextBoxUrl.SelectAll();

                if (null != Navigating)
                {
                    Navigating(uri);
                }
            }
        }

        internal void ObjWebBrowser_LoadCompleted(WebBrowserControl wbc)
        {
            if (wbc == active_wbc)
            {
                if (null != PageLoaded)
                {
                    PageLoaded();
                }
            }
        }

        public Uri CurrentUri
        {
            get
            {
                return active_wbc.CurrentUri;
            }
        }


        public string CurrentTitle
        {
            get
            {
                return active_wbc.Title;
            }
        }

        public string CurrentPageText
        {
            get
            {
                return active_wbc.PageText;
            }
        }

        public string CurrentPageHTML
        {
            get
            {
                return active_wbc.PageHTML;
            }
        }

        void ButtonForward_Click(object sender, RoutedEventArgs e)
        {
            CurrentWebBrowserControl.GoForward();
        }

        void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            CurrentWebBrowserControl.GoBack();
        }


        //bool is_first_grab_pdfs = true;        
        void ButtonGrabPDFs_Click(object sender, RoutedEventArgs e)
        {
            Uri current_uri = CurrentWebBrowserControl.CurrentUri;
            string html = CurrentWebBrowserControl.PageHTML;

            List<string> urls = DownloadableFileGrabber.Grab(html, "pdf");

            List<Uri> uris = new List<Uri>();
            foreach (string url in urls)
            {
                Uri uri;
                if (Uri.TryCreate(current_uri, url, out uri))
                {
                    uris.Add(uri);
                }
            }

            if (0 < uris.Count)
            {
                string msg = String.Format(
                    "Qiqqa has found {0} PDFs on this page.  Please choose the library into which you want to import them."
                    , uris.Count
                    );

                WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary(msg);
                if (null != web_library_detail)
                {
                    foreach (Uri uri in uris)
                    {
                        ImportingIntoLibrary.AddNewDocumentToLibraryFromInternet_ASYNCHRONOUS(web_library_detail.library, uri.ToString());
                    }
                }
                else
                {
                    MessageBoxes.Warn("No PDFs have been imported.");
                }
            }
            else
            {
                MessageBoxes.Info("Qiqqa could not find links to any PDFs on this page (with URLs ending in .pdf");
            }
        }


        WebBrowserControl CurrentWebBrowserControl
        {
            get
            {
                WebBrowserControl web_browser_control = (TabWebBrowserControls.CurrentActiveTabItem as WebBrowserControl) ?? wbc_browsing;
                return web_browser_control;
            }
        }

        internal void DoDictionarySearch(string query)
        {
            string search_terms = Uri.EscapeDataString(query);
            string url_browsing = String.Format("http://dictionary.reference.com/browse/{0}", search_terms);

            wbc_browsing.Navigate(url_browsing);
            TabWebBrowserControls.MakeActive(TAB_BROWSING);
        }


        ~WebBrowserHostControl()
        {
            Logging.Info("~WebBrowserHostControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Info("Disposing WebBrowserHostControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Get rid of managed resources
                DeleteSearchers();

                wbc_browsing.Dispose();
            }

            // Get rid of unmanaged resources 
        }
    }
}
