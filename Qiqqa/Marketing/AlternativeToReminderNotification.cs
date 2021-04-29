using System;
using icons;
using Qiqqa.Common.Configuration;
using Qiqqa.UtilisationTracking;
using Utilities.Misc;

namespace Qiqqa.Marketing
{
    public static class AlternativeToReminderNotification
    {
        internal static void CheckIfWeWantToNotify()
        {
            // Wait at least till the end of the first month before spamming away...
            if (DateTime.UtcNow.Subtract(ConfigurationManager.Instance.ConfigurationRecord.Marketing_LastNotificationOfAlternativeTo).TotalDays > 140)
            {
                ConfigurationManager.Instance.ConfigurationRecord.Marketing_LastNotificationOfAlternativeTo = DateTime.UtcNow;
                ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.Marketing_LastNotificationOfAlternativeTo));
                return;
            }

            // Don't spam more than once a month
            if (DateTime.UtcNow.Subtract(ConfigurationManager.Instance.ConfigurationRecord.Marketing_LastNotificationOfAlternativeTo).TotalDays < 28)
            {
                return;
            }

            // Remember when we last spammed them
            ConfigurationManager.Instance.ConfigurationRecord.Marketing_LastNotificationOfAlternativeTo = DateTime.UtcNow;
            ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.Marketing_LastNotificationOfAlternativeTo));

            // Spam away!
            NotificationManager.Instance.AddPendingNotification(
                new NotificationManager.Notification(
                    "It would massively help us out if you could please 'like' us on alternativeto.net.",
                    "Spread the word!",
                    NotificationManager.NotificationType.Info,
                    Icons.Champion,
                    "Do it now!",
                    FindOutMore
                    ));
        }

        private static void FindOutMore()
        {
            FeatureTrackingManager.Instance.UseFeature(Features.Marketing_AlternativeTo);
            WebsiteAccess.OpenWebsite(WebsiteAccess.Url_AlternativeTo);

            // If they have gone to investigate, don't harass them for a loooong time
            ConfigurationManager.Instance.ConfigurationRecord.Marketing_LastNotificationOfAlternativeTo = DateTime.UtcNow.AddMonths(12);
            ConfigurationManager.Instance.ConfigurationRecord_Bindable.NotifyPropertyChanged(nameof(ConfigurationManager.Instance.ConfigurationRecord.Marketing_LastNotificationOfAlternativeTo));
        }
    }
}
