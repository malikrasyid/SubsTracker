using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubsTracker.Models;
using SubsTracker.Services;
using Microcharts;
using SkiaSharp;
using SubsTracker.Helpers; 
using SubsTracker.Constants;
using System.Collections.ObjectModel;

namespace SubsTracker.ViewModels
{
    public class ChartLegendItem
    {
        public string Name { get; set; }
        public string Amount { get; set; }
        public Color Color { get; set; }
    }

    public partial class InsightsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private decimal _totalMonthlyPrice;

        [ObservableProperty]
        private decimal _estimatedYearlyPrice;
       
        [ObservableProperty]
        private Chart _categoryChart;

        [ObservableProperty]
        private Advice _smartAdvice;

        private readonly DatabaseService _databaseService;

        private readonly AdviceService _adviceService;

        public ObservableCollection<ChartLegendItem> LegendItems { get; } = new();

        public InsightsViewModel(DatabaseService databaseService, AdviceService adviceService)
        {
            _databaseService = databaseService;
            _adviceService = adviceService;
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
                    TotalMonthlyPrice = subscriptions.Sum(s => s.Price);
                    EstimatedYearlyPrice = TotalMonthlyPrice * 12;                  

                    CreateChart(subscriptions);

                    SmartAdvice = await _adviceService.GenerateAdviceAsync(subscriptions);
                }
                else
                {
                    TotalMonthlyPrice = 0;
                    EstimatedYearlyPrice = 0;
                    LegendItems.Clear();
                    CategoryChart = null;
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
        private void CreateChart(List<Subscription> subs)
        {
            var categoryData = subs
                .GroupBy(s => s.Category?.Trim() ?? AppConstants.Categories.Other, StringComparer.OrdinalIgnoreCase)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(s => s.Price),
                    HexColor = SubscriptionHelper.GetCategoryColor(g.Key)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            var entries = new List<ChartEntry>();
            LegendItems.Clear(); 

            foreach (var item in categoryData)
            {
                entries.Add(new ChartEntry((float)item.Total)
                {
                    Color = SKColor.Parse(item.HexColor),
                    ValueLabel = "", 
                    Label = ""
                });

                LegendItems.Add(new ChartLegendItem
                {
                    Name = item.Category,
                    Amount = item.Total.ToString("F2"),
                    Color = Color.FromArgb(item.HexColor) 
                });
            }

            CategoryChart = new DonutChart
            {
                Entries = entries,
                HoleRadius = 0.6f,        
                BackgroundColor = SKColors.Transparent,
                LabelTextSize = 0,        
                GraphPosition = GraphPosition.Center,
                Margin = 0
            };
        }
    }
}