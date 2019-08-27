using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Utilities.GUI
{
    public class AugmentedToolWindow : Window
    {
        public AugmentedToolWindow(UIElement child, string title)
        {
            this.Title = title;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ShowInTaskbar = true;
            this.Closing += AugmentedToolWindow_Closing;
            this.WindowStyle = WindowStyle.ToolWindow;
            this.KeyDown += AugmentedToolWindow_KeyDown;

            this.Content = child;
        }

        void AugmentedToolWindow_Closing(object sender, CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        void AugmentedToolWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
            }
        }

        public bool IsOpen
        {
            get
            {
                return true;    // HACK: only used by Microsoft Designer...
            }
            set
            {
                if (value == true)
                {
                    this.Show();
                }
                else
                {
                    this.Hide();
                }
            }
        }
    }
}
