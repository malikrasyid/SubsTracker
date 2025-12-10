using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubsTracker.Models;
using SubsTracker.Services;
using SubsTracker.Helpers;

namespace SubsTracker.ViewModels
{
    [QueryProperty(nameof(Subscription), "Subscription")]
    public partial class SubscriptionDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotEditing))]
        private bool _isEditing;

        public bool IsNotEditing => !IsEditing;

        [ObservableProperty]
        private Subscription _subscription;

        public List<string> Categories { get; } = new()
        { "Streaming", "Gaming", "Software", "Gym", "Utilities", "Music", "Other" };

        public List<BillingPeriodUnit> PeriodUnits { get; } = Enum.GetValues(typeof(BillingPeriodUnit)).Cast<BillingPeriodUnit>().ToList();

        public string FrequencyText => _subscription != null ? SubscriptionHelper.GetFrequencyText(_subscription) : "";

        public SubscriptionDetailViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        partial void OnSubscriptionChanged(Subscription value)
        {
            if (value != null)
            {
                Title = value.Name;
                OnPropertyChanged(nameof(FrequencyText));
            }
        }

        [RelayCommand]
        private void StartEdit()
        {
            IsEditing = true;
        }

        [RelayCommand]
        private async Task CancelEditAsync()
        {
            // Reload data from DB to discard unsaved changes
            if (_subscription != null)
            {
                var original = await _databaseService.GetSubscriptionByIdAsync(_subscription.Id);
                Subscription = original;
            }
            IsEditing = false;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (_subscription == null) return;

            // Recalculate Next Payment Date if they changed the cycle? 
            // Ideally, we only recalculate if they explicitly ask, but updating the DB is safe.
            // If you want to force update the next date based on new interval:
            // _subscription.NextPaymentDate = SubscriptionHelper.CalculateNextPayment(_subscription);

            await _databaseService.SaveSubscriptionAsync(_subscription);
            IsEditing = false;
            OnPropertyChanged(nameof(FrequencyText)); // Refresh helper text

            await Shell.Current.DisplayAlert("Success", "Updated!", "OK");
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