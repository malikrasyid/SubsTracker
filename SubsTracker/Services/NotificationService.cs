using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption; // Needed for specific Android settings
using SubsTracker.Models;

namespace SubsTracker.Services
{
    public class NotificationService
    {
        // 1. Schedule a Notification
        public async Task ScheduleSubscriptionReminder(Subscription sub)
        {
            // Permission Check (Crucial for Android 13+ and iOS)
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            // Logic: When should we notify?
            // If ReminderDays = 1, we notify 1 day before the NextPaymentDate
            var notifyDate = sub.NextPaymentDate.AddDays(-sub.ReminderDays);

            // If that date is in the past (e.g., user is adding a sub due today), 
            // set it for 1 hour from now or skip. Let's set it to 9:00 AM.
            var notifyTime = new DateTime(notifyDate.Year, notifyDate.Month, notifyDate.Day, 9, 0, 0);

            // If the calculated time is already passed, don't spam the user immediately.
            // (Optional logic: You could schedule it for next month's cycle)
            if (notifyTime < DateTime.Now) return;

            var notification = new NotificationRequest
            {
                NotificationId = sub.Id, // Use the DB ID so we can cancel it later easily
                Title = $"Bill Due: {sub.Name}",
                Description = $"{sub.Currency} {sub.Price} is due on {sub.NextPaymentDate:dd MMM}.",
                ReturningData = sub.Id.ToString(), // Pass ID to open the Detail Page when tapped
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime,
                    RepeatType = NotificationRepeat.No // We will reschedule manually after each payment for accuracy
                },
                Android = new AndroidOptions
                {
                    ChannelId = "subtrack_reminders",
                    IconSmallName = { ResourceName = "icon_notification" } // You need an icon in Resources/Drawable
                }
            };

            await LocalNotificationCenter.Current.Show(notification);
        }

        // 2. Cancel a Notification (When user deletes a sub or edits dates)
        public void CancelReminder(int subscriptionId)
        {
            LocalNotificationCenter.Current.Cancel(subscriptionId);
        }
    }
}