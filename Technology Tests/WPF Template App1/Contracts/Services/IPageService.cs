using System;
using System.Windows.Controls;

using GalaSoft.MvvmLight;

namespace WPF_Template_App1.Contracts.Services
{
    public interface IPageService
    {
        Type GetPageType(string key);

        Page GetPage(string key);

        void Configure<VM, V>()
            where VM : ViewModelBase
            where V : Page;
    }
}
