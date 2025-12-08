using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubsTracker.Models;
using SubsTracker.Services;

namespace SubsTracker.ViewModels
{
    public partial class AddSubscriptionViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private decimal _price;

        [ObservableProperty]
        private DateTime _expiryDate = DateTime.Today;

        private readonly DatabaseService _databaseService;
        public AddSubscriptionViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Add Subscription";
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || Price <= 0)
            {
                // Simple validation alert
                await Shell.Current.DisplayAlert("Invalid Input", "Please enter a valid name and price.", "OK");
                return;
            }

            // Create the new object
            var newSub = new Subscription
            {
                Name = Name,
                Price = Price,
                ExpiryDate = ExpiryDate
            };

            await _databaseService.SaveSubscriptionAsync(newSub);

            // Navigate back to the previous page (Dashboard)
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            // Just go back without saving
            await Shell.Current.GoToAsync("..");
        }
    }
}