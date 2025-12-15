using SubsTracker.Models;
using SubsTracker.Helpers; 
using Plugin.LocalNotification;

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
            await CheckAndGenerateNotifications();
            return await _databaseService.GetNotificationsAsync();
        }

        public async Task MarkAsReadAsync(Notification notification)
        {
            await _databaseService.MarkNotificationAsReadAsync(notification.Id);
        }

        public async Task CheckAndGenerateNotifications()
        {
            var subs = await _databaseService.GetSubscriptionsAsync();
            var existingNotifications = await _databaseService.GetNotificationsAsync();

            foreach (var sub in subs)
            {
                int daysDue = SubscriptionHelper.GetDaysUntilDue(sub.NextPaymentDate);

                if (daysDue <= 3 && daysDue >= 0)
                {
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

                        var request = new NotificationRequest
                        {
                            NotificationId = newNote.Id, // Use the DB ID or a random int
                            Title = newNote.Title,
                            Description = newNote.Message,
                            BadgeNumber = 1,
                            Schedule = new NotificationRequestSchedule
                            {
                                NotifyTime = DateTime.Now.AddSeconds(1) // Fire immediately
                            }
                        };
                        await LocalNotificationCenter.Current.Show(request);

                    }
                }
            }
        }
    }
}