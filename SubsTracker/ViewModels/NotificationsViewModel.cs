using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubsTracker.Models;
using SubsTracker.Services;
using System.Collections.ObjectModel;

namespace SubsTracker.ViewModels
{
    public partial class NotificationsViewModel : BaseViewModel
    {
        private readonly NotificationService _notificationService;
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Notification> Notifications { get; } = new();

        public NotificationsViewModel(NotificationService notificationService, DatabaseService databaseService)
        {
            _notificationService = notificationService;
            _databaseService = databaseService;

            Title = "Notifications";
        }

        [RelayCommand]
        public async Task LoadNotificationsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                await _notificationService.CheckAndGenerateNotifications();

                var notifs = await _notificationService.GetNotificationsAsync();

                if (Notifications.Count > 0) Notifications.Clear();

                foreach (var note in notifs)
                {
                    Notifications.Add(note);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading notifications: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task TapNotificationAsync(Notification notification)
        {
            if (notification == null) return;

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _notificationService.MarkAsReadAsync(notification);
               
                var index = Notifications.IndexOf(notification);
                Notifications[index] = notification;
            }
            
            var sub = await _databaseService.GetSubscriptionByIdAsync(notification.SubscriptionId);

            if (sub != null)
            {
                var navParam = new Dictionary<string, object>
                {
                    { "Subscription", sub }
                };
                await Shell.Current.GoToAsync(nameof(Views.SubscriptionDetailPage), navParam);
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "This subscription no longer exists.", "OK");
            }
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        void ClearAll()
        {
            Notifications.Clear();
        }
    }
}