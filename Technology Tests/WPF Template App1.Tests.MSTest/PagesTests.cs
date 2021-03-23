using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows.Controls;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WPF_Template_App1.Contracts.Services;
using WPF_Template_App1.Contracts.Views;
using WPF_Template_App1.Core.Contracts.Services;
using WPF_Template_App1.Core.Services;
using WPF_Template_App1.Models;
using WPF_Template_App1.Services;
using WPF_Template_App1.ViewModels;
using WPF_Template_App1.Views;

namespace WPF_Template_App1.Tests.MSTest
{
    [TestClass]
    public class PagesTests
    {
        public PagesTests()
        {
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(appLocation)
                .AddJsonFile("appsettings.json")
                .Build();

            var appConfig = configuration
                .GetSection(nameof(AppConfig))
                .Get<AppConfig>();

            SimpleIoc.Default.Reset();

            // Register configurations to IoC
            SimpleIoc.Default.Register(() => configuration);
            SimpleIoc.Default.Register(() => appConfig);

            // App Host
            SimpleIoc.Default.Register<IApplicationHostService, ApplicationHostService>();

            // Core Services
            SimpleIoc.Default.Register<ISystemService, SystemService>();
            SimpleIoc.Default.Register<ISampleDataService, SampleDataService>();
            SimpleIoc.Default.Register<IFileService, FileService>();
            SimpleIoc.Default.Register<IIdentityService, IdentityService>();
            SimpleIoc.Default.Register<IMicrosoftGraphService, MicrosoftGraphService>();
            SimpleIoc.Default.Register<IApplicationInfoService, ApplicationInfoService>();

            // Services
            SimpleIoc.Default.Register<IThemeSelectorService, ThemeSelectorService>();
            SimpleIoc.Default.Register<IPersistAndRestoreService, PersistAndRestoreService>();
            SimpleIoc.Default.Register<IUserDataService, UserDataService>();
            SimpleIoc.Default.Register<IIdentityCacheService, IdentityCacheService>();
            SimpleIoc.Default.Register(() => GetHttpClientFactory());
            SimpleIoc.Default.Register<IPageService, PageService>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>();

            // Window
            SimpleIoc.Default.Register<IShellWindow, ShellWindow>();
            SimpleIoc.Default.Register<ShellViewModel>();

            // Pages
            RegisterPage<WebViewViewModel, WebViewPage>();
            RegisterPage<SettingsViewModel, SettingsPage>();
            RegisterPage<MasterDetailViewModel, MasterDetailPage>();
            RegisterPage<MainViewModel, MainPage>();
            RegisterPage<DataGridViewModel, DataGridPage>();
            RegisterPage<ContentGridViewModel, ContentGridPage>();
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

        private void RegisterPage<VM, V>()
            where VM : ViewModelBase
            where V : Page
        {
            SimpleIoc.Default.Register<VM>();
            SimpleIoc.Default.Register<V>();
            SimpleIoc.Default.GetInstance<IPageService>().Configure<VM, V>();
        }

        // TODO WTS: Add tests for functionality you add to WebViewViewModel.
        [TestMethod]
        public void TestWebViewViewModelCreation()
        {
            var vm = SimpleIoc.Default.GetInstance<WebViewViewModel>();
            Assert.IsNotNull(vm);
        }

        [TestMethod]
        public void TestGetWebViewPageType()
        {
            var pageService = SimpleIoc.Default.GetInstance<IPageService>();

            var pageType = pageService.GetPageType(typeof(WebViewViewModel).FullName);

            Assert.AreEqual(typeof(WebViewPage), pageType);
        }

        // TODO WTS: Add tests for functionality you add to SettingsViewModel.
        [TestMethod]
        public void TestSettingsViewModelCreation()
        {
            var vm = SimpleIoc.Default.GetInstance<SettingsViewModel>();
            Assert.IsNotNull(vm);
        }

        [TestMethod]
        public void TestGetSettingsPageType()
        {
            var pageService = SimpleIoc.Default.GetInstance<IPageService>();

            var pageType = pageService.GetPageType(typeof(SettingsViewModel).FullName);

            Assert.AreEqual(typeof(SettingsPage), pageType);
        }

        // TODO WTS: Add tests for functionality you add to MasterDetailViewModel.
        [TestMethod]
        public void TestMasterDetailViewModelCreation()
        {
            var vm = SimpleIoc.Default.GetInstance<MasterDetailViewModel>();
            Assert.IsNotNull(vm);
        }

        [TestMethod]
        public void TestGetMasterDetailPageType()
        {
            var pageService = SimpleIoc.Default.GetInstance<IPageService>();

            var pageType = pageService.GetPageType(typeof(MasterDetailViewModel).FullName);

            Assert.AreEqual(typeof(MasterDetailPage), pageType);
        }

        // TODO WTS: Add tests for functionality you add to MainViewModel.
        [TestMethod]
        public void TestMainViewModelCreation()
        {
            var vm = SimpleIoc.Default.GetInstance<MainViewModel>();
            Assert.IsNotNull(vm);
        }

        [TestMethod]
        public void TestGetMainPageType()
        {
            var pageService = SimpleIoc.Default.GetInstance<IPageService>();

            var pageType = pageService.GetPageType(typeof(MainViewModel).FullName);

            Assert.AreEqual(typeof(MainPage), pageType);
        }

        // TODO WTS: Add tests for functionality you add to DataGridViewModel.
        [TestMethod]
        public void TestDataGridViewModelCreation()
        {
            var vm = SimpleIoc.Default.GetInstance<DataGridViewModel>();
            Assert.IsNotNull(vm);
        }

        [TestMethod]
        public void TestGetDataGridPageType()
        {
            var pageService = SimpleIoc.Default.GetInstance<IPageService>();

            var pageType = pageService.GetPageType(typeof(DataGridViewModel).FullName);

            Assert.AreEqual(typeof(DataGridPage), pageType);
        }

        // TODO WTS: Add tests for functionality you add to ContentGridViewModel.
        [TestMethod]
        public void TestContentGridViewModelCreation()
        {
            var vm = SimpleIoc.Default.GetInstance<ContentGridViewModel>();
            Assert.IsNotNull(vm);
        }

        [TestMethod]
        public void TestGetContentGridPageType()
        {
            var pageService = SimpleIoc.Default.GetInstance<IPageService>();

            var pageType = pageService.GetPageType(typeof(ContentGridViewModel).FullName);

            Assert.AreEqual(typeof(ContentGridPage), pageType);
        }
    }
}
