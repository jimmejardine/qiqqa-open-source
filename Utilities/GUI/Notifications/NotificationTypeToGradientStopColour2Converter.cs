using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Utilities.Misc;

namespace Utilities.GUI.Notifications
{
    public class NotificationTypeToGradientStopColour2Converter : IValueConverter
    {
        public object Convert(object value, Type target_type, object parameter, CultureInfo culture)
        {
            //  use the Notification rather than the notification type, since need a ref object
            NotificationManager.Notification notification = value as NotificationManager.Notification;
            if (notification == null) return DependencyProperty.UnsetValue;
            switch (notification.Type)
            {
                case NotificationManager.NotificationType.Info:
                    return "#b5cbe8";
                case NotificationManager.NotificationType.Warning:
                    return "#fae793";
                case NotificationManager.NotificationType.Error:
                    return "#fe4242";
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
