using System.Windows.Controls;

using MahApps.Metro.Controls;

using WPF_Template_App1.Behaviors;

namespace WPF_Template_App1.Contracts.Views
{
    public interface IShellWindow
    {
        Frame GetNavigationFrame();

        void ShowWindow();

        void CloseWindow();

        Frame GetRightPaneFrame();

        SplitView GetSplitView();

        RibbonTabsBehavior GetRibbonTabsBehavior();
    }
}
