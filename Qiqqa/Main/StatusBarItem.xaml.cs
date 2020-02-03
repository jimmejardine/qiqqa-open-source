using System;
using System.Diagnostics;
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
        private Stopwatch creation_time = Stopwatch.StartNew();
        private string key = null;
        private Stopwatch last_status_update_time = Stopwatch.StartNew();

        public StatusBarItem()
        {
            InitializeComponent();

            ObjProgressBar.Maximum = 100;
            ObjTextSquare.Click += ObjTextSquare_Click;
        }

        public void SetStatus(StatusEntry status)
        {
            key = status.key;
            last_status_update_time.Restart();

            string text = status.LastStatusMessage;
            bool cancel = status.LastStatusMessageCancellable;

            ObjTextSquare.IsEnabled = cancel;
            ObjTextSquare.Width = 16;
            ObjTextSquareText.Text = cancel ? "X" : "■";
            ObjTextBlock.Text = Utilities.Strings.StringTools.TrimToLengthWithEllipsis(text);
            ObjTextBlock.ToolTip = text;
            double w = ObjTextBlock.ActualWidth;

            if (String.IsNullOrEmpty(text))
            {
                Visibility = Visibility.Collapsed;
                ObjProgressBar.Value = 0;
                ObjProgressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                Visibility = Visibility.Visible;

                double pct = status.UpdatePerunage;

                ObjProgressBar.Value = 100 * pct;
                ObjProgressBar.Visibility = pct > 0.0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ObjTextSquare_Click(object sender, RoutedEventArgs e)
        {
            StatusManager.Instance.SetCancelled(key);
        }

        public Stopwatch CreationTime => creation_time;

        public Stopwatch TimeSinceLastStatusUpdate => last_status_update_time;
    }
}
