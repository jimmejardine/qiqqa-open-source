using System.Windows;
using System.Windows.Controls;
using Utilities.Internet;

namespace Qiqqa.Common.Configuration
{
    /// <summary>
    /// Interaction logic for ProxySettingsControl.xaml
    /// </summary>
    public partial class ProxySettingsControl : UserControl
    {
        public class StandardProxySettings
        {
            public bool Proxy_UseProxy { get; set; }
            public string Proxy_Hostname { get; set; }
            public int Proxy_Port { get; set; }
            public string Proxy_Username { get; set; }
            public string Proxy_Password { get; set; }
        }

        public ProxySettingsControl()
        {
            InitializeComponent();

            CheckUseProxy.Checked += CheckUseProxy_Checked;
            CheckUseProxy.Unchecked += CheckUseProxy_Unchecked;

            ObjHyperLink_UseDefaultCredentials.Click += ObjHyperLink_UseDefaultCredentials_Click;
            ObjHyperLink_UseDefaultNetworkCredentials.Click += ObjHyperLink_UseDefaultNetworkCredentials_Click;

            ReevaluateEnabledNess();
        }

        private void ObjHyperLink_UseDefaultNetworkCredentials_Click(object sender, RoutedEventArgs e)
        {
            ObjUserName.Text = ProxyTools.USERNAME_DEFAULT_CREDENTIALS;
            ObjPassword.Password = "";
        }

        private void ObjHyperLink_UseDefaultCredentials_Click(object sender, RoutedEventArgs e)
        {
            ObjUserName.Text = ProxyTools.USERNAME_DEFAULT_NETWORK_CREDENTIALS;
            ObjPassword.Password = "";
        }

        private void CheckUseProxy_Checked(object sender, RoutedEventArgs e)
        {
            ReevaluateEnabledNess();
        }

        private void CheckUseProxy_Unchecked(object sender, RoutedEventArgs e)
        {
            ReevaluateEnabledNess();
        }

        private void ReevaluateEnabledNess()
        {
            if (CheckUseProxy.IsChecked ?? false)
            {
                GridProxyDetails.IsEnabled = true;
            }
            else
            {
                GridProxyDetails.IsEnabled = false;
            }
        }
    }
}
