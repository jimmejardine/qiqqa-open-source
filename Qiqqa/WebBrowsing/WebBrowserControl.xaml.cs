using System;
using System.Windows.Controls;
using System.Windows.Threading;
using Gecko;
using Gecko.Events;
using Qiqqa.Common;
using Qiqqa.Common.Configuration;
using Utilities;
using Utilities.GUI;
using Utilities.Misc;

// https://docs.microsoft.com/en-us/microsoft-edge/webview2/gettingstarted/wpf

namespace Qiqqa.WebBrowsing
{
    /// <summary>
    /// Interaction logic for WebBrowserControl.xaml
    /// </summary>
    public partial class WebBrowserControl : UserControl, IDisposable
    {
        private WebBrowserHostControl web_browser_host_control;

        public WebBrowserControl(WebBrowserHostControl web_browser_host_control)
        {
            this.web_browser_host_control = web_browser_host_control;

            InitializeComponent();

        }

        private void ObjWebBrowser_CreateWindow(object sender, GeckoCreateWindowEventArgs e)
        {
        }

        private void ObjWebBrowser_StatusTextChanged(object sender, EventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                GeckoWebBrowser web_control = (GeckoWebBrowser)sender;
                StatusManager.Instance.UpdateStatus("WebBrowser", web_control.StatusText);
                Logging.Info("Browser:StatusTextChanged: {0}", web_control.StatusText);
            });
        }

        private void ObjWebBrowser_Navigating(object sender, GeckoNavigatingEventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                web_browser_host_control.ObjWebBrowser_Navigating(this, e.Uri);
            });
        }

        private void ObjWebBrowser_DocumentCompleted(object sender, EventArgs e)
        {
            WPFDoEvents.SafeExec(() =>
            {
                GeckoWebBrowser web_control = (GeckoWebBrowser)sender;
                Logging.Info("Browser page contents received at url {0}", web_control.Url.ToString());
                web_browser_host_control.ObjWebBrowser_LoadCompleted(this);
            });
        }

        internal void GoForward()
        {
        }

        internal void GoBack()
        {
        }

        internal void Print()
        {
        }

        internal void Refresh()
        {
        }

        internal void Navigate(String uri)
        {
            Navigate(new Uri(uri));
        }

        internal void Navigate(Uri uri)
        {
        }

        internal Uri NavigateOnceVisibleUri
        {
            get;
            set;
        }

        internal Uri NavigateToPendingOnceVisibleUri()
        {
            var uri = NavigateOnceVisibleUri;

            Navigate(uri);

            return uri;
        }

        public string Title;

        internal Uri CurrentUri;

        public string PageText;

        public string PageHTML;

        #region --- IDisposable ------------------------------------------------------------------------

        ~WebBrowserControl()
        {
            Logging.Debug("~WebBrowserControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing WebBrowserControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("WebBrowserControl::Dispose({0}) @{1}", disposing, dispose_count);

            WPFDoEvents.InvokeInUIThread(() =>
            {
                WPFDoEvents.SafeExec(() =>
                {
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

                        // Multiple WebBrowserControl instances MAY SHARE a single WebBrowserHostControl.
                        // It is passed to this class/instance as a reference anyway, so we SHOULD NOT
                        // kill/dispose it in here!
                        //
                        //web_browser_host_control.Dispose();
                        web_browser_host_control = null;
                    }
                });

                WPFDoEvents.SafeExec(() =>
                {
                    web_browser_host_control = null;
                });

                ++dispose_count;
            });
        }

        #endregion

    }
}
