using System.Windows.Controls;

using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;

using WPF_Template_App1.ViewModels;

namespace WPF_Template_App1.Views
{
    public partial class WebViewPage : Page
    {
        private WebViewViewModel ViewModel
            => DataContext as WebViewViewModel;

        public WebViewPage()
        {
            InitializeComponent();
            ViewModel.Initialize(webView);
        }

        private void OnNavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
            => ViewModel.OnNavigationCompleted(e);
    }
}
