using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubsTracker.Models;
using SubsTracker.Services;
using Microcharts;
using SkiaSharp;

namespace SubsTracker.ViewModels
{
    public partial class InsightsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private decimal _totalMonthlyCost;

        [ObservableProperty]
        private decimal _estimatedYearlyCost;

        [ObservableProperty]
        private string _mostExpensiveName = "None";

        [ObservableProperty]
        private decimal _mostExpensiveCost;

        [ObservableProperty]
        private Chart _categoryChart;

        [ObservableProperty]
        private string _smartAdvice;

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
                    TotalMonthlyCost = subscriptions.Sum(s => s.Cost);
                    EstimatedYearlyCost = TotalMonthlyCost * 12;

                    var expensive = subscriptions.OrderByDescending(s => s.Cost).FirstOrDefault();
                    MostExpensiveName = expensive?.Name ?? "None";
                    MostExpensiveCost = expensive?.Cost ?? 0;

                    CreateChart(subscriptions);

                    GenerateAdvice(subscriptions);
                }
                else
                {
                    TotalMonthlyCost = 0;
                    EstimatedYearlyCost = 0;
                    CategoryChart = null;
                    SmartAdvice = "Add subscriptions to see insights!";
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
            // Group subscriptions by Category and sum their costs
            var categoryData = subs
                .GroupBy(s => s.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Total = g.Sum(s => s.Cost),
                    Color = GetColorForCategory(g.Key)
                })
                .ToList();

            // Convert to Microcharts entries
            var entries = categoryData.Select(d => new ChartEntry((float)d.Total)
            {
                Label = d.Category,
                ValueLabel = d.Total.ToString("0.00"),
                Color = SKColor.Parse(d.Color)
            }).ToArray();

            // Create the Chart Object
            CategoryChart = new DonutChart // 'Donut' looks more modern than 'Pie'
            {
                Entries = entries,
                LabelTextSize = 30,
                HoleRadius = 0.6f, // Makes it a donut
                BackgroundColor = SKColors.Transparent
            };
        }

        private void GenerateAdvice(List<Subscription> subs)
        {
            var streamingTotal = subs.Where(s => s.Category == "Streaming").Sum(s => s.Cost);
            var subCount = subs.Count;

            // Simple "AI" Logic
            if (streamingTotal > 50)
            {
                SmartAdvice = $"You spend ${streamingTotal:F0} on streaming! Consider rotating services (cancel one, watch the other) to save ~${streamingTotal / 2:F0}/mo.";
            }
            else if (subCount > 8)
            {
                SmartAdvice = "You have a lot of active subscriptions. Check if you are actually using all of them properly.";
            }
            else if (MostExpensiveCost > 100)
            {
                SmartAdvice = $"{MostExpensiveName} is your biggest expense. Is there a cheaper annual plan available?";
            }
            else
            {
                SmartAdvice = "Your spending looks balanced! Keep it up.";
            }
        }

        private string GetColorForCategory(string category)
        {
            return category switch
            {
                "Streaming" => "#E50914",
                "Music" => "#1DB954",
                "Gaming" => "#107C10",
                "Gym" => "#007AFF",
                "Utilities" => "#FF9900",
                _ => "#808080"
            };
        }
    }
}