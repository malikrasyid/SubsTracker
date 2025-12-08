using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubsTracker.Models;
using SubsTracker.Services;

namespace SubsTracker.ViewModels
{
    // This attribute links the navigation parameter "Subscription" to the property below
    [QueryProperty(nameof(Subscription), "Subscription")]
    public partial class SubscriptionDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private Subscription _subscription;

        public SubscriptionDetailViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        partial void OnSubscriptionChanged(Subscription value)
        {
            // Update the Page Title to the subscription name
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
            if (_subscription == null) return;

            // Confirm before deleting
            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Subscription",
                $"Are you sure you want to delete {_subscription.Name}?",
                "Yes",
                "No");

            if (confirm)
            {
                await _databaseService.DeleteSubscriptionAsync(_subscription);

                // Go back to dashboard
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