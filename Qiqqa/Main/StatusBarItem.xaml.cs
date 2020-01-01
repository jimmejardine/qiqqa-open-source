using System;
using System.Windows;
using System.Windows.Controls;
using Utilities.Mathematics;
using Utilities.Misc;
using static Utilities.Misc.StatusManager;

namespace Qiqqa.Main
{
    /// <summary>
    /// Interaction logic for StatusBarItem.xaml
    /// </summary>
    public partial class StatusBarItem : UserControl
    {
        private DateTime creation_time = DateTime.UtcNow;
        private string key = null;
        private DateTime last_status_update_time = DateTime.MinValue;

        public StatusBarItem()
        {
            InitializeComponent();

            ObjProgressBar.Maximum = 100;
            ObjTextSquare.Click += ObjTextSquare_Click;
        }

        public void SetStatus(StatusEntry status)
        {
            key = status.key;
            last_status_update_time = DateTime.UtcNow;

            string text = status.LastStatusMessage;
            bool cancel = status.LastStatusMessageCancellable;
            long current = status.current_update_number;
            long max = status.total_update_count;

            ObjTextSquare.IsEnabled = cancel;
            ObjTextSquare.Width = 16;
            ObjTextSquareText.Text = cancel ? "X" : "■";
            ObjTextBlock.Text = Utilities.Strings.StringTools.TrimToLengthWithEllipsis(text);
            ObjTextBlock.ToolTip = text;
            double w = ObjTextBlock.ActualWidth;

            if (String.IsNullOrEmpty(text))
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                Visibility = Visibility.Visible;
            }

            if (0 == max && 0 == current)
            {
                ObjProgressBar.Value = 100;
                ObjProgressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                ObjProgressBar.Value = 100 * Perunage.Calc(current, max);
                ObjProgressBar.Visibility = Visibility.Visible;
            }
        }

        private void ObjTextSquare_Click(object sender, RoutedEventArgs e)
        {
            StatusManager.Instance.SetCancelled(key);
        }

        public DateTime CreationTime => creation_time;

        public DateTime LastStatusUpdateTime => last_status_update_time;
    }
}
