using System;
using System.Windows;
using System.Windows.Controls;
using Utilities.Misc;

namespace Qiqqa.Main
{
    /// <summary>
    /// Interaction logic for StatusBarItem.xaml
    /// </summary>
    public partial class StatusBarItem : UserControl
    {
        DateTime creation_time = DateTime.UtcNow;
        
        string key = null;
        DateTime last_status_update_time = DateTime.MinValue;

        public StatusBarItem()
        {
            InitializeComponent();
            
            ObjProgressBar.Maximum = 100;
            ObjTextSquare.Click += ObjTextSquare_Click;
        }

        public void SetStatus(string key, string text, bool cancellable, long current, long max)
        {
            this.key = key;            
            this.last_status_update_time = DateTime.UtcNow;

            ObjTextSquare.IsEnabled = cancellable;
            ObjTextSquare.Width = 16;
            ObjTextSquareText.Text = cancellable ? "X" : "■";
            ObjTextBlock.Text = Utilities.Strings.StringTools.TrimToLengthWithEllipsis(text);
            ObjTextBlock.ToolTip = text;

            if (String.IsNullOrEmpty(text))
            {
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }

            if (0 == max)
            {
                ObjProgressBar.Value = 100;
                if (Visibility.Collapsed != ObjProgressBar.Visibility)
                {
                    ObjProgressBar.Visibility = Visibility.Collapsed;
                }
            }
            else if (1 == current)
            {
                ObjProgressBar.Value = 100 * Math.Pow(1.0 * current / max, 1.0/3.0);
                if (Visibility.Visible != ObjProgressBar.Visibility)
                {
                    ObjProgressBar.Visibility = Visibility.Visible;
                }
            }
            else
            {
                ObjProgressBar.Value = 100 * current / max;
                if (Visibility.Visible != ObjProgressBar.Visibility)
                {
                    ObjProgressBar.Visibility = Visibility.Visible;
                }
            }            
        }

        void ObjTextSquare_Click(object sender, RoutedEventArgs e)
        {
            StatusManager.Instance.SetCancelled(this.key);
        }

        public DateTime CreationTime
        {
            get
            {
                return creation_time;
            }
        }

        public DateTime LastStatusUpdateTime
        {
            get
            {
                return last_status_update_time;
            }
        }
    }
}
