using System.Threading.Tasks;

using WPF_Template_App1.Core.Models;

namespace WPF_Template_App1.Core.Contracts.Services
{
    public interface IMicrosoftGraphService
    {
        Task<User> GetUserInfoAsync(string accessToken);

        Task<string> GetUserPhoto(string accessToken);
    }
}
