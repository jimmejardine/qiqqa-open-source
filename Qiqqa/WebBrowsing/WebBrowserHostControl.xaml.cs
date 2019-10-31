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
        WebBrowserControl wbc_browsing = null;
        List<WebSearcherEntry> web_searcher_entries = new List<WebSearcherEntry>();

        Library current_library = null;
        public Library CurrentLibrary
        {
            get
            {
                Library lib = current_library;
                if (null == lib)
                {
                    WebLibraryDetail web_library_detail = WebLibraryPicker.PickWebLibrary();
                    if (null != web_library_detail)
                    {
                        lib = web_library_detail.library;
                    }
                }
                return lib;
            }
            set
            {
                current_library = value;
            }
        }

        public WebBrowserHostControl()
        {
            Logging.Debug("+WebBrowserHostControl()");

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

            ButtonAddToLibrary.Icon = Icons.GetAppIcon(Icons.WebAddToLibrary);
            if (!ADVANCED_MENUS) ButtonAddToLibrary.Caption = "Add PDF to\nLibrary";
            ButtonAddToLibrary.ToolTip = "Add the currently displayed page to your library (must be a PDF).";
            ButtonAddToLibrary.Click += new RoutedEventHandler(ButtonAddToLibrary_Click);

            if (ADVANCED_MENUS) ForceAdvancedMenus();

            web_searcher_preference_control = new WebSearcherPreferenceControl(this);
            wbc_browsing = new WebBrowserControl(this);

            TabWebBrowserControls.OnActiveItemChanged += TabWebBrowserControls_OnActiveItemChanged;

            RebuildSearchers();

            Logging.Debug("-WebBrowserHostControl()");
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
            // This URL is slow/timeout:
            // this.OpenNewWindow("http://www.qiqqa.com/Account/Edit");

            //this.OpenNewWindow(WebsiteAccess.Url_GithubRepo4Qiqqa);
            this.OpenNewWindow(WebsiteAccess.Url_BlankWebsite);
        }

        internal void ForceAdvancedMenus()
        {
            ButtonNewBrowser.Caption = null;
            ButtonBack.Caption = null;
            ButtonForward.Caption = null;
            ButtonGrabWebPage.Caption = null;
            ButtonPrint.Caption = null;
            ButtonEZProxy.Caption = null;
            ButtonGrabPDFs.Caption = null;
            ButtonAddToLibrary.Caption = null;

            TxtWebAddress.Visibility = Visibility.Collapsed;
            TxtSearchTheWeb.Visibility = Visibility.Collapsed;
        }

        internal void SetupSnifferSearchers()
        {
            HashSet<string> once_off_requested_web_searchers = new HashSet<string>();
            once_off_requested_web_searchers.Add(WebSearchers.SCHOLAR_KEY);
            once_off_requested_web_searchers.Add(WebSearchers.PUBMEDXML_KEY);
            RebuildSearchers(once_off_requested_web_searchers);
        }

        private string default_web_searcher_key = null;
        public string DefaultWebSearcherKey
        {
            get
            {
                return default_web_searcher_key;
            }
            set
            {
                default_web_searcher_key = value;
            }
        }

        // the .CloseContent() call inside DeleteSearchers() can trigger additional
        // cleanup elsewhere, which will invoke this class' Dispose(true) method,
        // which - yes, otherwise intentionally - invokes DeleteSearches(),
        // resulting in a bit of havoc.
        //
        // The quick & dirty way out of this conundrum (while maximizing GC) is to
        // track whether we're entering here as part of an 'outer' call to 
        // DeleteSearches(); we use a counter instead of a boolean for debugging/analysis
        // purposes.
        private int executing_DeleteSearchers = 0;

        private void DeleteSearchers()
        {
            ++executing_DeleteSearchers;
            Logging.Debug("ENTER DeleteSearchers() nesting level {0}", executing_DeleteSearchers);
            try
            {
                if (executing_DeleteSearchers == 1)
                {
                    foreach (var web_searcher_entry in web_searcher_entries)
                    {
                        TabWebBrowserControls.CloseContent(web_searcher_entry.browser_control);

                        web_searcher_entry.web_searcher = null;
                        web_searcher_entry.browser_control = null;
                    }

                    web_searcher_entries.Clear();
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "DeleteSearchers() UNEXPECTED FAILURE");
                throw;
            }
            finally
            {
                Logging.Debug("LEAVE DeleteSearchers() nesting level {0}", executing_DeleteSearchers);
                --executing_DeleteSearchers;
            }
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
            HashSet<string> requested_web_searchers = WebSearcherPreferenceManager.Instance.LoadPreferences(once_off_requested_web_searchers);

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

                Logging.Debug特("Active browser control changed");
                active_wbc = wbc;

                Uri uri = wbc.CurrentUri;
                TextBoxUrl.Text = uri == null ? String.Empty : uri.ToString();
            }

            TabChanged?.Invoke();
        }

        // TODO: make it work akin to the <embed> handling to prevent confusion: 
        // when the browser shows a single PDF, it MAY be an <embed> web page and 
        // we should account for that!
        void ButtonAddToLibrary_Click(object sender, RoutedEventArgs e)
        {
            if (null == CurrentUri)
            {
                MessageBoxes.Error("You need to have navigated to a web page before trying to import it into your library.");
                return;
            }

            FeatureTrackingManager.Instance.UseFeature(Features.Web_AddToLibrary);

            string url = CurrentUri.AbsoluteUri;

            Library lib = CurrentLibrary;
            if (null != lib)
            {
                ImportingIntoLibrary.AddNewDocumentToLibraryFromInternet_ASYNCHRONOUS(lib, url);
            }
        }

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

        internal void SelectSearchTab(string active_search_key)
        {
            TabWebBrowserControls.MakeActive(active_search_key);
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
            WebBrowserControl wbc = new WebBrowserControl(this);   // <-- must be Dispose()d by caller

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
            // sanitize search text: TABS to spaces
            search_terms = Utilities.Strings.StringTools.Sanitize(search_terms);

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

            // Make sure we are looking at a search page... **UNLESS** the user has explicitly selected 
            // the current browser tab.
            if (CurrentWebBrowserControl == wbc_browsing)
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

                Navigating?.Invoke(uri);
            }
        }

        internal void ObjWebBrowser_LoadCompleted(WebBrowserControl wbc)
        {
            if (wbc == active_wbc)
            {
                PageLoaded?.Invoke();
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

        void ButtonGrabPDFs_Click(object sender, RoutedEventArgs e)
        {
            Uri current_uri = CurrentWebBrowserControl.CurrentUri;
            string html = CurrentWebBrowserControl.PageHTML;

            List<string> urls = DownloadableFileGrabber.Grab(current_uri, html, "pdf");

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

                Library lib = CurrentLibrary;
                if (null != lib)
                {
                    foreach (Uri uri in uris)
                    {
                        ImportingIntoLibrary.AddNewDocumentToLibraryFromInternet_ASYNCHRONOUS(lib, uri.AbsoluteUri);
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
            string url_browsing = String.Format(WebsiteAccess.Url_ThesaurusSearch, search_terms);

            wbc_browsing.Navigate(url_browsing);
            TabWebBrowserControls.MakeActive(TAB_BROWSING);
        }

        #region --- Disposal ----------------------------------------------------------------------------------------

        ~WebBrowserHostControl()
        {
            Logging.Debug("~WebBrowserHostControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing WebBrowserHostControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("WebBrowserHostControl::Dispose({0}) @{1}", disposing, dispose_count);

            // Prevent recursive run-away of the code via the chain:
            //
            // *** 	Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserControl.Dispose(bool disposing)
            // **   Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserControl.Dispose() 
            // 	    Utilities.dll!Utilities.GUI.DualTabbedLayoutStuff.DualTabbedLayout.WantsClose(Utilities.GUI.DualTabbedLayoutStuff.DualTabbedLayoutItem item) 
            //      Utilities.dll!Utilities.GUI.DualTabbedLayoutStuff.DualTabbedLayout.CloseContent(System.Windows.FrameworkElement fe) 
            //      Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserHostControl.DeleteSearchers() 
            //      Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserHostControl.Dispose(bool disposing) 
            //      Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserHostControl.Dispose() 
            // ***  Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserControl.Dispose(bool disposing) 
            // **   Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserControl.Dispose() 
            //
            // and prevent partial/broken cleanup due to chains like this one, resulting in
            // a dispose_count == 2:
            //
            // =2 * Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserHostControl.Dispose(bool disposing) 
            //      Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserHostControl.Dispose() 
            // =2 * Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserControl.Dispose(bool disposing) 
            //      Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserControl.Dispose() 
            // =1   Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserHostControl.Dispose(bool disposing) 
            //      Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserHostControl.Dispose() 
            // =1   Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserControl.Dispose(bool disposing) 
            //      Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserControl.Dispose() 
            //      Utilities.dll!Utilities.GUI.DualTabbedLayoutStuff.DualTabbedLayout.WantsClose(Utilities.GUI.DualTabbedLayoutStuff.DualTabbedLayoutItem item) 
            //      Utilities.dll!Utilities.GUI.DualTabbedLayoutStuff.DualTabbedLayout.CloseContent(System.Windows.FrameworkElement fe) 
            // *    Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserHostControl.DeleteSearchers() 
            //      Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserHostControl.RebuildSearchers(System.Collections.Generic.HashSet<string> once_off_requested_web_searchers) 
            //      Qiqqa.exe!Qiqqa.WebBrowsing.WebBrowserHostControl.ForceSnifferSearchers() 
            //
            if (dispose_count == 0)
            {
                // Get rid of managed resources
                DeleteSearchers();

                wbc_browsing?.Dispose();

                active_wbc?.Dispose();

                //TabChanged -= ;
                //PageLoaded -= ;
                //Navigating -= ;

                TextBoxUrl.OnHardSearch -= TextBoxUrl_OnHardSearch;
                TextBoxGoogleScholar.OnHardSearch -= TextBoxGoogleScholar_OnHardSearch;

                TabWebBrowserControls.OnActiveItemChanged -= TabWebBrowserControls_OnActiveItemChanged;

                //TabWebBrowserControls.Clear();
            }

            web_searcher_entries = null;

            wbc_browsing = null;
            active_wbc = null;
            CurrentLibrary = null;

            web_searcher_preference_control = null;

            // // DeleteSearchers(); ===>
            //web_searcher_entries.Clear();

            ++dispose_count;
        }

        #endregion

    }
}
