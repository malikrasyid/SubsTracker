using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubsTracker.Models;
using SubsTracker.Services;
using System.Collections.ObjectModel;

namespace SubsTracker.ViewModels
{
    public partial class AddSubscriptionViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private decimal _cost;

        [ObservableProperty]
        private DateTime _nextPaymentDate = DateTime.Today;

        [ObservableProperty]
        private string _selectedCategory;

        [ObservableProperty]
        private string _customCategory;

        public ObservableCollection<string> CommonCategories { get; } = new()
        {
            "Streaming", "Gaming", "Software", "Gym", "Utilities", "Other"
        };

        [ObservableProperty]
        private BillingCycle _selectedCycle = BillingCycle.Monthly;

        public List<BillingCycle> BillingCycles { get; } = Enum.GetValues(typeof(BillingCycle)).Cast<BillingCycle>().ToList();

        private readonly DatabaseService _databaseService;
        
        public AddSubscriptionViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Add Subscription";
            SelectedCategory = CommonCategories.First();
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || _cost <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Enter valid name and cost.", "OK");
                return;
            }

            string finalCategory = _selectedCategory;
            if (_selectedCategory == "Other" && !string.IsNullOrWhiteSpace(_customCategory))
            {
                finalCategory = _customCategory;
            }

            var newSub = new Subscription
            {
                Name = _name,
                Cost = _cost, 
                NextPaymentDate = _nextPaymentDate, // Fixed
                Category = finalCategory,
                BillingCycle = _selectedCycle
            };

            await _databaseService.SaveSubscriptionAsync(newSub);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}