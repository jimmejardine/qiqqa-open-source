using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Utilities.Misc;

namespace Utilities.GUI.Notifications
{
    public class NotificationTypeToGradientStopColour1Converter : IValueConverter
    {
        public object Convert(object value, Type target_type, object parameter, CultureInfo culture)
        {
            //  use the Notification rather than the notification type, since need a ref object
            NotificationManager.Notification notification = value as NotificationManager.Notification;
            if (notification == null) return DependencyProperty.UnsetValue;
            switch (notification.Type)
            {
                case NotificationManager.NotificationType.Info:
                    return "#d9e6f9";
                case NotificationManager.NotificationType.Warning:
                    return "#fff2b7";
                case NotificationManager.NotificationType.Error:
                    return "#f9afaf";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type target_type, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
