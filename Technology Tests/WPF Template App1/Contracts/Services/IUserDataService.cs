using System;

using WPF_Template_App1.ViewModels;

namespace WPF_Template_App1.Contracts.Services
{
    public interface IUserDataService
    {
        event EventHandler<UserViewModel> UserDataUpdated;

        void Initialize();

        UserViewModel GetUser();
    }
}
