using SubsTracker.Models;
using SubsTracker.Helpers; // To use SubscriptionHelper logic

namespace SubsTracker.Services
{
    public class NotificationService
    {
        private readonly DatabaseService _databaseService;

        public NotificationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Notification>> GetNotificationsAsync()
        {
            // 1. First, check if we need to generate new alerts
            await CheckAndGenerateNotifications();

            // 2. Then return the list from DB
            return await _databaseService.GetNotificationsAsync();
        }

        public async Task MarkAsReadAsync(Notification notification)
        {
            await _databaseService.MarkNotificationAsReadAsync(notification.Id);
        }

        // --- THE LOGIC ENGINE ---
        private async Task CheckAndGenerateNotifications()
        {
            var subs = await _databaseService.GetSubscriptionsAsync();
            var existingNotifications = await _databaseService.GetNotificationsAsync();

            foreach (var sub in subs)
            {
                // 1. Calculate days until due using your existing Helper
                int daysDue = SubscriptionHelper.GetDaysUntilDue(sub.NextPaymentDate);

                // 2. Logic: Notify if due in 3 days, 1 day, or Today (0 days)
                if (daysDue <= 3 && daysDue >= 0)
                {
                    // 3. SPAM PREVENTION:
                    // Check if we already created a notification for this subscription TODAY.
                    bool alreadyNotifiedToday = existingNotifications.Any(n => 
                        n.SubscriptionId == sub.Id && 
                        n.Timestamp.Date == DateTime.Today);

                    if (!alreadyNotifiedToday)
                    {
                        var newNote = new Notification
                        {
                            Title = daysDue == 0 ? $"{sub.Name} is due TODAY!" : $"Upcoming Payment: {sub.Name}",
                            Message = $"Prepare ${sub.Price} for your {sub.BillingInterval} {sub.PeriodUnit} payment.",
                            Timestamp = DateTime.Now,
                            IsRead = false,
                            SubscriptionId = sub.Id,
                            CategoryColor = sub.HexColor // Uses the model's color logic
                        };

                        await _databaseService.SaveNotificationAsync(newNote);
                    }
                }
            }
        }
    }
}