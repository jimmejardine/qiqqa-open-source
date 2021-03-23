using System.Threading.Tasks;

namespace WPF_Template_App1.Contracts.Services
{
    public interface IApplicationHostService
    {
        Task StartAsync();

        Task StopAsync();
    }
}
