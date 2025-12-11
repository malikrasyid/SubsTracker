using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubsTracker.Constants;
using SubsTracker.Helpers;
using SubsTracker.Models;
using SubsTracker.Services;
using SubsTracker.Constants;
using System.Collections.ObjectModel;

namespace SubsTracker.ViewModels
{
    public partial class AddSubscriptionViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private decimal _price;

        [ObservableProperty]
        private string _currency;

        [ObservableProperty]
        private DateTime _firstBillDate = DateTime.Today;

        [ObservableProperty]
        private string _selectedCategory;

        [ObservableProperty]
        private string _customCategory;

        [ObservableProperty]
        private int _billingInterval = 1; // Default "1"

        [ObservableProperty]
        private BillingPeriodUnit _selectedPeriodUnit = BillingPeriodUnit.Month; // Default "Month"

        public List<BillingPeriodUnit> PeriodUnits { get; } = Enum.GetValues(typeof(BillingPeriodUnit)).Cast<BillingPeriodUnit>().ToList();

        public ObservableCollection<string> CommonCategories { get; } = new(AppConstants.Categories.All);

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
                await Shell.Current.DisplayAlert("Error", "Enter valid name and cost.", "OK");
                return;
            }

            string finalCategory = SelectedCategory == AppConstants.Categories.Other && !string.IsNullOrWhiteSpace(CustomCategory)
                ? CustomCategory
                : SelectedCategory;

            var newSub = new Subscription
            {
                Name = Name,
                Description = Description,
                Price = Price,
                Currency = Currency,
                FirstBillDate = FirstBillDate,

                BillingInterval = BillingInterval,
                PeriodUnit = SelectedPeriodUnit,

                Category = finalCategory,
                IsActive = true
            };

            newSub.NextPaymentDate = SubscriptionHelper.CalculateNextPayment(newSub);

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