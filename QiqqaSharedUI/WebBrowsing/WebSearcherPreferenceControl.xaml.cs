using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using icons;

namespace Qiqqa.WebBrowsing
{
    /// <summary>
    /// Interaction logic for WebSearcherPreferenceControl.xaml
    /// </summary>
    public partial class WebSearcherPreferenceControl : UserControl
    {
        private WebBrowserHostControl web_browser_host_control;

        public WebSearcherPreferenceControl(WebBrowserHostControl web_browser_host_control)
        {
            this.web_browser_host_control = web_browser_host_control;

            InitializeComponent();

            ObjHeader.Img = Icons.GetAppIcon(Icons.WebPreferences);
            ObjHeader.Caption = "Web Preferences";
            ObjHeader.SubCaption = "Please select the WebSearchers that you would like to appear as tabs in your Qiqqa web browser.";

            CmdSave.Icon = Icons.GetAppIcon(Icons.Save);
            CmdSave.Caption = "Save preferences";
            CmdSave.Click += CmdSave_Click;

            CmdCancel.Icon = Icons.GetAppIcon(Icons.Cancel);
            CmdCancel.Caption = "Reset";
            CmdCancel.Click += CmdCancel_Click;

            RepopulateWebSearchers();

            Keyboard.Focus(ObjListWebSearchers);
        }

        private void CmdCancel_Click(object sender, RoutedEventArgs e)
        {
            RepopulateWebSearchers();
        }

        private void CmdSave_Click(object sender, RoutedEventArgs e)
        {
            HashSet<string> requested_web_searchers = new HashSet<string>();
            foreach (WebSearcher web_searcher in ObjListWebSearchers.SelectedItems)
            {
                requested_web_searchers.Add(web_searcher.key);
            }
            WebSearcherPreferenceManager.Instance.SavePreferences(requested_web_searchers);

            web_browser_host_control.RebuildSearchers(requested_web_searchers);
        }

        private void RepopulateWebSearchers()
        {
            // Show all the available options
            ObjListWebSearchers.ItemsSource = null;
            ObjListWebSearchers.ItemsSource = WebSearchers.WEB_SEARCHERS;

            // Select the ones already chosen by the user
            HashSet<string> requested_web_searchers = WebSearcherPreferenceManager.Instance.LoadPreferences();
            foreach (var searcher in WebSearchers.WEB_SEARCHERS)
            {
                if (requested_web_searchers.Contains(searcher.key))
                {
                    ObjListWebSearchers.SelectedItems.Add(searcher);
                }
            }
        }
    }
}
