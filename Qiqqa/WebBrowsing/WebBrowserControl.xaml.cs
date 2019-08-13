using System;
using System.Windows.Controls;
using Gecko;
using Gecko.Events;
using Qiqqa.Common;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;

namespace Qiqqa.WebBrowsing
{
    /// <summary>
    /// Interaction logic for WebBrowserControl.xaml
    /// </summary>
    public partial class WebBrowserControl : UserControl, IDisposable
    {
        WebBrowserHostControl web_browser_host_control;

        public WebBrowserControl(WebBrowserHostControl web_browser_host_control)
        {
            this.web_browser_host_control = web_browser_host_control;

            InitializeComponent();

            ObjWebBrowser.CreateControl();
            ObjWebBrowser.Navigating += ObjWebBrowser_Navigating;
            ObjWebBrowser.DocumentCompleted += ObjWebBrowser_DocumentCompleted;
            ObjWebBrowser.CreateWindow += ObjWebBrowser_CreateWindow;
            
            // Seems to crash Qiqqa in Gecko v13 - perhaps the statuses are updating too quickly or in parallel?!
            // Seems to work with gecko v21...
            ObjWebBrowser.StatusTextChanged += ObjWebBrowser_StatusTextChanged;
        }

        void ObjWebBrowser_CreateWindow(object sender, GeckoCreateWindowEventArgs e)
        {
            WebBrowserHostControl wbhc = MainWindowServiceDispatcher.Instance.OpenWebBrowser();
            WebBrowserControl wbc = wbhc.OpenNewWindow();
            e.WebBrowser = wbc.ObjWebBrowser;
        }
        
        void ObjWebBrowser_StatusTextChanged(object sender, EventArgs e)
        {   
            GeckoWebBrowser web_control = (GeckoWebBrowser)sender;
            StatusManager.Instance.UpdateStatus("WebBrowser", web_control.StatusText);
            Logging.Info("Browser:StatusTextChanged: {0}", web_control.StatusText);
        }

        void ObjWebBrowser_Navigating(object sender, GeckoNavigatingEventArgs e)
        {
            web_browser_host_control.ObjWebBrowser_Navigating(this, e.Uri);
        }

        void ObjWebBrowser_DocumentCompleted(object sender, EventArgs e)
        {
            GeckoWebBrowser web_control = (GeckoWebBrowser)sender;
            Logging.Info("Browser page contents received at url {0}", web_control.Url.ToString());
            web_browser_host_control.ObjWebBrowser_LoadCompleted(this);
        }

        internal void GoForward()
        {
            ObjWebBrowser.GoForward();
        }

        internal void GoBack()
        {
            ObjWebBrowser.GoBack();
        }

        internal void Print()
        {
            ObjWebBrowser.Navigate("javascript:print()");
        }

        internal void Refresh()
        {
            try
            {
                ObjWebBrowser.Reload();
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem refreshing web page");
            }
        }

        internal void Navigate(String uri)
        {
            Navigate(new Uri(uri));
        }
        
        internal void Navigate(Uri uri)
        {
            try
            {
                // Clear out any pending uri
                this.navigate_once_visible_uri = null;

                ObjWebBrowser.Navigate(uri.ToString());
            }
            catch (Exception ex)
            {
                Logging.Warn(ex, "Problem navigating to website {0}", uri.ToString());
                MessageBoxes.Error("There was a problem navigating to {0}.  Please try again after checking the web address.", uri.ToString());
            }
        }

        private Uri navigate_once_visible_uri = null;
        internal void NavigateOnceVisible(Uri uri)
        {
            this.navigate_once_visible_uri = uri;
        }

        internal void NavigateToPendingOnceVisibleUri()
        {
            if (null != this.navigate_once_visible_uri)
            {
                Navigate(this.navigate_once_visible_uri);
            }
        }

        public string Title
        {
            get
            {
                return ObjWebBrowser.DocumentTitle;
            }
        }

        internal Uri CurrentUri
        {
            get
            {
                return ObjWebBrowser.Url;
            }
        }

        public string PageText
        {
            get
            {
                return ObjWebBrowser.Document.Body.TextContent;
            }
        }

        public string PageHTML
        {
            get
            {   
                return ObjWebBrowser.Document.GetElementsByTagName("html")[0].OuterHtml;
            }
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~WebBrowserControl()
        {
            Logging.Info("~WebBrowserControl()");
            Dispose(false);            
        }

        public void Dispose()
        {
            Logging.Info("Disposing WebBrowserControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        private void Dispose(bool disposing)
        {
            ++dispose_count;
            Logging.Debug("WebBrowserControl::Dispose({0}) @{1}", disposing ? "true" : "false", dispose_count);
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
            if (dispose_count == 1)
            {
                if (disposing)
                {
                    try
                    {
                        // Get rid of managed resources
                        ObjWebBrowser.Dispose();
                        ObjWebBrowser = null;
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(ex, "Error disposing Gecko webbrowser");
                    }

                    // Multiple WebBrowserControl instances MAY SHARE a single WebBrowserHostControl.
                    // It is passed to this class/instance as a reference anyway, so we SHOULD NOT
                    // kill/dispose it in here!
                    //
                    //web_browser_host_control.Dispose();
                }

                ObjWebBrowser = null;
                web_browser_host_control = null;

                // Get rid of unmanaged resources 
            }
        }

        #endregion

    }
}
