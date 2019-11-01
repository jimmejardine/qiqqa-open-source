using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Utilities.GUI
{
    public class AugmentedToolWindow : Window
    {
        public AugmentedToolWindow(UIElement child, string title)
        {
            Title = title;
            SizeToContent = SizeToContent.WidthAndHeight;
            ShowInTaskbar = true;
            Closing += AugmentedToolWindow_Closing;
            WindowStyle = WindowStyle.ToolWindow;
            KeyDown += AugmentedToolWindow_KeyDown;

            Content = child;
        }

        private void AugmentedToolWindow_Closing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void AugmentedToolWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Hide();
            }
        }

        public bool IsOpen
        {
            get => true;    // HACK: only used by Microsoft Designer...
            set
            {
                if (value == true)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }
    }
}
