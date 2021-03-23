using System;
using System.Collections.ObjectModel;

using GalaSoft.MvvmLight;

using WPF_Template_App1.Contracts.ViewModels;
using WPF_Template_App1.Core.Contracts.Services;
using WPF_Template_App1.Core.Models;

namespace WPF_Template_App1.ViewModels
{
    public class DataGridViewModel : ViewModelBase, INavigationAware
    {
        private readonly ISampleDataService _sampleDataService;

        public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

        public DataGridViewModel(ISampleDataService sampleDataService)
        {
            _sampleDataService = sampleDataService;
        }

        public async void OnNavigatedTo(object parameter)
        {
            Source.Clear();

            // TODO WTS: Replace this with your actual data
            var data = await _sampleDataService.GetGridDataAsync();

            foreach (var item in data)
            {
                Source.Add(item);
            }
        }

        public void OnNavigatedFrom()
        {
        }
    }
}
