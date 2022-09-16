using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using icons;
using Utilities.Misc;
using Utilities.Reflection;

namespace Utilities.GUI.Notifications
{
    /// <summary>
    /// Notification bar at the top of the screen that displays messages such as updates available.
    /// Can be closed, and has two buttons that can do something contextually useful.
    /// </summary>
    public partial class NotificationBar
    {
        public NotificationBar()
        {
            InitializeComponent();

            RenderOptions.SetBitmapScalingMode(NotificationImage, BitmapScalingMode.HighQuality);

            CloseButton.Source = Icons.GetAppIcon(Icons.NotificationBarClose);

            //  register for new events from the NotificationManager, don't bother deregistering since this will live for the life of the app
            NotificationManager.Instance.NewNotificationFired += Instance_NewNotificationFired;
        }

        /// <summary>
        /// Need to do this until can load the images from the xaml :(
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(NotificationBar));
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        private void DisplayNotification(NotificationManager.Notification notification)
        {
            if (!CheckAccess())
            {
                WPFDoEvents.InvokeInUIThread(() => DisplayNotification(notification));
                return;
            }

            ImageSource = string.IsNullOrEmpty(notification.ImageName) ? null : Icons.GetAppIcon(notification.ImageName);
            DataContext = new AugmentedBindable<NotificationManager.Notification>(notification);
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// The NotificationManager is responsible for telling us when to update our notification.
        /// </summary>
        private void Instance_NewNotificationFired(NotificationManager.Notification notification)
        {
            DisplayNotification(notification);
        }

        private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Logging.Debug特("Notification bar closed by user for: {0}", MainText.Text);
            Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            try
            {
                Logging.Debug特("User clicked on notification bar button for '{0}', executing action in thread pool", MainText.Text);
                Button button = sender as Button;
                if (button == null)
                {
                    Logging.Warn("Notification button is null?");
                    return;
                }
                //  give this to the thread pool
                SafeThreadPool.QueueUserWorkItem((WaitCallback)button.Tag);
            }
            catch (Exception ex)
            {
                Logging.Error(ex, "Problem executing action triggered from notification bar button click");
            }
        }
    }
}
