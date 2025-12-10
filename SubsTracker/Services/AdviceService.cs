using SubsTracker.Models;

namespace SubsTracker.Services
{
    public class AdviceService
    {
        // Simulate an Async API call
        public async Task<Advice> GenerateAdviceAsync(List<Subscription> subs)
        {
            // Simulate network delay (optional, feels more real)
            await Task.Delay(100);

            // --- LOCAL LOGIC (Replace this with API Call later) ---

            if (!subs.Any())
            {
                return new Advice
                {
                    Title = "Get Started",
                    Message = "Add your first subscription to get AI insights.",
                    Type = AdviceType.Info
                };
            }

            var streamingTotal = subs.Where(s => s.Category == "Streaming").Sum(s => s.Price);
            var total = subs.Sum(s => s.Price);

            if (streamingTotal > 50)
            {
                return new Advice
                {
                    Title = "High Streaming Cost",
                    Message = $"You spend ${streamingTotal:F0} just on streaming! Rotating services could save you ~${streamingTotal / 2:F0}/mo.",
                    Type = AdviceType.Warning
                };
            }

            if (total > 200)
            {
                return new Advice
                {
                    Title = "Budget Alert",
                    Message = "Your total monthly subscriptions exceed $200. Review if you use all of them.",
                    Type = AdviceType.Warning
                };
            }

            return new Advice
            {
                Title = "Healthy Spending",
                Message = "Your subscription portfolio looks balanced. Good job!",
                Type = AdviceType.Good
            };
        }
    }
}