using System;
using System.Linq;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Ioc;

using WPF_Template_App1.Contracts.Services;
using WPF_Template_App1.Contracts.Views;
using WPF_Template_App1.Core.Contracts.Services;
using WPF_Template_App1.Models;
using WPF_Template_App1.ViewModels;

namespace WPF_Template_App1.Services
{
    public class ApplicationHostService : IApplicationHostService
    {
        private readonly INavigationService _navigationService;
        private readonly IPersistAndRestoreService _persistAndRestoreService;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly IIdentityService _identityService;
        private readonly IUserDataService _userDataService;
        private readonly AppConfig _appConfig;
        private readonly IRightPaneService _rightPaneService;
        private IShellWindow _shellWindow;

        public ApplicationHostService(INavigationService navigationService, IRightPaneService rightPaneService, IThemeSelectorService themeSelectorService, IPersistAndRestoreService persistAndRestoreService, IIdentityService identityService, IUserDataService userDataService, AppConfig config)
        {
            _navigationService = navigationService;
            _rightPaneService = rightPaneService;
            _themeSelectorService = themeSelectorService;
            _persistAndRestoreService = persistAndRestoreService;
            _identityService = identityService;
            _userDataService = userDataService;
            _appConfig = config;
        }

        public async Task StartAsync()
        {
            // Initialize services that you need before app activation
            await InitializeAsync();

            _identityService.InitializeWithAadAndPersonalMsAccounts(_appConfig.IdentityClientId, "http://localhost");
            await _identityService.AcquireTokenSilentAsync();

            await HandleActivationAsync();

            // Tasks after activation
            await StartupAsync();
        }

        public async Task StopAsync()
        {
            _persistAndRestoreService.PersistData();
            await Task.CompletedTask;
        }

        private async Task InitializeAsync()
        {
            _persistAndRestoreService.RestoreData();
            _themeSelectorService.InitializeTheme();
            await Task.CompletedTask;
            _userDataService.Initialize();
        }

        private async Task StartupAsync()
        {
            await Task.CompletedTask;
        }

        private async Task HandleActivationAsync()
        {
            if (App.Current.Windows.OfType<IShellWindow>().Count() == 0)
            {
                // Default activation that navigates to the apps default page
                _shellWindow = SimpleIoc.Default.GetInstance<IShellWindow>(Guid.NewGuid().ToString());
                _navigationService.Initialize(_shellWindow.GetNavigationFrame());
                _rightPaneService.Initialize(_shellWindow.GetRightPaneFrame(), _shellWindow.GetSplitView());
                _shellWindow.ShowWindow();
                _navigationService.NavigateTo(typeof(MainViewModel).FullName);
                await Task.CompletedTask;
            }
        }
    }
}
