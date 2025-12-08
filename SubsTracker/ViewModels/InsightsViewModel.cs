using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubsTracker.Models;
using SubsTracker.Services;
using System.Linq;

namespace SubsTracker.ViewModels
{
    public partial class InsightsViewModel : BaseViewModel
    {
        // 1. Properties for the UI to display
        [ObservableProperty]
        private decimal _totalMonthlyExpenses;

        [ObservableProperty]
        private decimal _estimatedYearlyExpenses;

        [ObservableProperty]
        private string _mostExpensiveName = "None";

        [ObservableProperty]
        private decimal _mostExpensivePrice;

        private readonly DatabaseService _databaseService;
        public InsightsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Insights";            
        }

        [RelayCommand]
        private async Task LoadInsightsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var subscriptions = await _databaseService.GetSubscriptionsAsync();

                if (subscriptions.Any())
                {
                    // Calculate Monthly Total
                    TotalMonthlyExpenses = subscriptions.Sum(s => s.Price);

                    // Calculate Yearly (Simple projection: Monthly * 12)
                    EstimatedYearlyExpenses = _totalMonthlyExpenses * 12;

                    // Find the most expensive subscription
                    var expensiveSub = subscriptions.OrderByDescending(s => s.Price).FirstOrDefault();
                    if (expensiveSub != null)
                    {
                        MostExpensiveName = expensiveSub.Name;
                        MostExpensivePrice = expensiveSub.Price;
                    }
                }
                else
                {
                    // Reset if no data
                    TotalMonthlyExpenses = 0;
                    EstimatedYearlyExpenses = 0;
                    MostExpensiveName = "None";
                    MostExpensivePrice = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating insights: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}