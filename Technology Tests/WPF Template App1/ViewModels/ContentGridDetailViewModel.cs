using System;
using System.Linq;

using GalaSoft.MvvmLight;

using WPF_Template_App1.Contracts.ViewModels;
using WPF_Template_App1.Core.Contracts.Services;
using WPF_Template_App1.Core.Models;

namespace WPF_Template_App1.ViewModels
{
    public class ContentGridDetailViewModel : ViewModelBase, INavigationAware
    {
        private readonly ISampleDataService _sampleDataService;
        private SampleOrder _item;

        public SampleOrder Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }

        public ContentGridDetailViewModel(ISampleDataService sampleDataService)
        {
            _sampleDataService = sampleDataService;
        }

        public async void OnNavigatedTo(object parameter)
        {
            if (parameter is long orderID)
            {
                var data = await _sampleDataService.GetContentGridDataAsync();
                Item = data.First(i => i.OrderID == orderID);
            }
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
