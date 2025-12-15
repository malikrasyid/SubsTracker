using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using SubsTracker.Models;
using SubsTracker.Services;
using System.Collections.ObjectModel;

namespace SubsTracker.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        // ObservableCollection automatically updates the UI when items are added/removed
        public ObservableCollection<Subscription> Subscriptions { get; } = new();

        [ObservableProperty]
        private decimal _totalMonthlyPrice;

        private readonly DatabaseService _databaseService;

        public DashboardViewModel(DatabaseService databaseService) // <--- INJECT HERE
        {
            _databaseService = databaseService; // <--- SAVE IT
            Title = "Dashboard";
        }

        [RelayCommand]
        private async Task LoadSubscriptionsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
            
                var subs = await _databaseService.GetSubscriptionsAsync();

                if (Subscriptions.Count > 0) Subscriptions.Clear();
                foreach (var sub in subs) Subscriptions.Add(sub);

                CalculateTotal();
            }
            catch (Exception ex)
            {
                // In a real app, use a proper logging service
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CalculateTotal()
        {
            // Simple LINQ sum
            TotalMonthlyPrice = Subscriptions.Sum(s => s.Price);
        }

        [RelayCommand]
        private async Task GoToAddSubscriptionAsync()
        {
            // Navigate to the registered route
            await Shell.Current.GoToAsync(nameof(Views.AddSubscriptionPage));
        }

        [RelayCommand]
        private async Task GoToDetailsAsync(Subscription subscription)
        {
            if (subscription == null) return;

            // Pass the object to the details page
            var navigationParameter = new Dictionary<string, object>
    {
        { "Subscription", subscription }
    };

            await Shell.Current.GoToAsync(nameof(Views.SubscriptionDetailPage), navigationParameter);
        }

        [RelayCommand]
        async Task GoToNotifications()
        {
            // Navigates to the NotificationPage defined in AppShell routes
            await Shell.Current.GoToAsync(nameof(Views.NotificationPage));
        }
    }
}