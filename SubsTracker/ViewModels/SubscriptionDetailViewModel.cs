using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubsTracker.Models;
using SubsTracker.Services;
using System.Collections.ObjectModel;

namespace SubsTracker.ViewModels
{
    [QueryProperty(nameof(Subscription), "Subscription")]
    public partial class SubscriptionDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private Subscription _subscription;

        public List<string> Categories { get; } = new()
        { "Streaming", "Gaming", "Software", "Gym", "Utilities", "Music", "Other" };

        public List<BillingCycle> BillingCycles { get; } = Enum.GetValues(typeof(BillingCycle)).Cast<BillingCycle>().ToList();

        public SubscriptionDetailViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        partial void OnSubscriptionChanged(Subscription value)
        {
            if (value != null)
            {
                Title = value.Name;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (_subscription == null) return;

            // This calls the method we made in DatabaseService. 
            // Since the ID already exists, it will perform an UPDATE, not an INSERT.
            await _databaseService.SaveSubscriptionAsync(_subscription);

            // Notify user
            await Shell.Current.DisplayAlert("Success", "Subscription updated!", "OK");

            // Go back to the dashboard
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            bool confirm = await Shell.Current.DisplayAlert("Delete", $"Delete {_subscription.Name}?", "Yes", "No");
            if (confirm)
            {
                await _databaseService.DeleteSubscriptionAsync(_subscription);
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}