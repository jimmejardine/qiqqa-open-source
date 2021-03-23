using System.Collections.Generic;
using System.Threading.Tasks;

using WPF_Template_App1.Core.Models;

namespace WPF_Template_App1.Core.Contracts.Services
{
    public interface ISampleDataService
    {
        Task<IEnumerable<SampleOrder>> GetContentGridDataAsync();

        Task<IEnumerable<SampleOrder>> GetGridDataAsync();

        Task<IEnumerable<SampleOrder>> GetMasterDetailDataAsync();
    }
}
