using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Fluent;

using Microsoft.Xaml.Behaviors;

using WPF_Template_App1.Contracts.Services;
using WPF_Template_App1.Contracts.ViewModels;

namespace WPF_Template_App1.Behaviors
{
    public class BackstageTabNavigationBehavior : Behavior<BackstageTabControl>
    {
        private IPageService _pageService;

        public BackstageTabNavigationBehavior()
        {
        }

        public void Initialize(IPageService pageService)
        {
            _pageService = pageService;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is BackstageTabItem tabItem)
            {
                var frame = new Frame()
                {
                    Focusable = false,
                    NavigationUIVisibility = NavigationUIVisibility.Hidden
                };
                frame.Navigated += OnNavigated;
                tabItem.Content = frame;
                var page = _pageService.GetPage(tabItem.Tag as string);
                frame.Navigate(page);
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is FrameworkElement element)
            {
                if (element.DataContext is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(e.ExtraData);
                }
            }
        }
    }
}
