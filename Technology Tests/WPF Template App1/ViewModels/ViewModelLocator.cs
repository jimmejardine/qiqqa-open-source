using System;
using System.Net.Http;
using System.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WPF_Template_App1.Contracts.Services;
using WPF_Template_App1.Contracts.Views;
using WPF_Template_App1.Core.Contracts.Services;
using WPF_Template_App1.Core.Services;
using WPF_Template_App1.Models;
using WPF_Template_App1.Services;
using WPF_Template_App1.Views;

namespace WPF_Template_App1.ViewModels
{
    public class ViewModelLocator
    {
        private IPageService PageService
            => SimpleIoc.Default.GetInstance<IPageService>();

        public ShellViewModel ShellViewModel
            => SimpleIoc.Default.GetInstance<ShellViewModel>();

        public MainViewModel MainViewModel
            => SimpleIoc.Default.GetInstance<MainViewModel>();

        public WebViewViewModel WebViewViewModel
            => SimpleIoc.Default.GetInstance<WebViewViewModel>();

        public MasterDetailViewModel MasterDetailViewModel
            => SimpleIoc.Default.GetInstance<MasterDetailViewModel>();

        public ContentGridViewModel ContentGridViewModel
            => SimpleIoc.Default.GetInstance<ContentGridViewModel>();

        public ContentGridDetailViewModel ContentGridDetailViewModel
            => SimpleIoc.Default.GetInstance<ContentGridDetailViewModel>();

        public DataGridViewModel DataGridViewModel
            => SimpleIoc.Default.GetInstance<DataGridViewModel>();

        public SettingsViewModel SettingsViewModel
            => SimpleIoc.Default.GetInstance<SettingsViewModel>();

        public ViewModelLocator()
        {
            // App Host
            SimpleIoc.Default.Register<IApplicationHostService, ApplicationHostService>();

            // Core Services
            SimpleIoc.Default.Register<IMicrosoftGraphService, MicrosoftGraphService>();
            SimpleIoc.Default.Register<IIdentityCacheService, IdentityCacheService>();
            SimpleIoc.Default.Register<IIdentityService, IdentityService>();
            SimpleIoc.Default.Register(() => GetHttpClientFactory());
            SimpleIoc.Default.Register<IApplicationInfoService, ApplicationInfoService>();
            SimpleIoc.Default.Register<IFileService, FileService>();
            SimpleIoc.Default.Register<ISampleDataService, SampleDataService>();
            SimpleIoc.Default.Register<ISystemService, SystemService>();

            // Services
            SimpleIoc.Default.Register<IUserDataService, UserDataService>();
            SimpleIoc.Default.Register<IWindowManagerService, WindowManagerService>();
            SimpleIoc.Default.Register<IPersistAndRestoreService, PersistAndRestoreService>();
            SimpleIoc.Default.Register<IThemeSelectorService, ThemeSelectorService>();
            SimpleIoc.Default.Register<IRightPaneService, RightPaneService>();
            SimpleIoc.Default.Register<IPageService, PageService>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>();

            // Window
            SimpleIoc.Default.Register<IShellWindow, ShellWindow>();
            SimpleIoc.Default.Register<ShellViewModel>();

            // Pages
            Register<MainViewModel, MainPage>();
            Register<WebViewViewModel, WebViewPage>();
            Register<MasterDetailViewModel, MasterDetailPage>();
            Register<ContentGridViewModel, ContentGridPage>();
            Register<ContentGridDetailViewModel, ContentGridDetailPage>();
            Register<DataGridViewModel, DataGridPage>();
            Register<SettingsViewModel, SettingsPage>();
        }

        private void Register<VM, V>()
            where VM : ViewModelBase
            where V : Page
        {
            SimpleIoc.Default.Register<VM>();
            SimpleIoc.Default.Register<V>();
            PageService.Configure<VM, V>();
        }

        public void AddConfiguration(IConfiguration configuration)
        {
            var appConfig = configuration
                .GetSection(nameof(AppConfig))
                .Get<AppConfig>();

            // Register configurations to IoC
            SimpleIoc.Default.Register(() => configuration);
            SimpleIoc.Default.Register(() => appConfig);
        }

        private IHttpClientFactory GetHttpClientFactory()
        {
            var services = new ServiceCollection();
            services.AddHttpClient("msgraph", client =>
            {
                client.BaseAddress = new System.Uri("https://graph.microsoft.com/v1.0/");
            });

            return services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>();
        }
    }
}
