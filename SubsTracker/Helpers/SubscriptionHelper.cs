using SubsTracker.Models;

namespace SubsTracker.Helpers
{
    public static class SubscriptionHelper
    {
        public static DateTime CalculateNextPaymentDate(DateTime fromDate, int interval, BillingPeriodUnit unit)
        {
            return unit switch
            {
                BillingPeriodUnit.Day => fromDate.AddDays(interval),
                BillingPeriodUnit.Week => fromDate.AddDays(interval * 7),
                BillingPeriodUnit.Month => fromDate.AddMonths(interval),
                BillingPeriodUnit.Year => fromDate.AddYears(interval),
                _ => fromDate.AddMonths(1)
            };
        }

        public static DateTime CalculateNextPayment(Subscription sub)
        {
            return CalculateNextPaymentDate(sub.NextPaymentDate, sub.BillingInterval, sub.PeriodUnit);
        }

        public static string GetFrequencyText(Subscription sub)
        {
            if (sub.BillingInterval == 1)
                return sub.PeriodUnit.ToString(); // "Month"

            return $"{sub.BillingInterval} {sub.PeriodUnit}s"; // "2 Weeks"
        }

        public static int GetDaysUntilDue(DateTime nextDate)
        {
            return (nextDate.Date - DateTime.Now.Date).Days;
        }

        // 2. Logic for Initial Letter
        public static string GetInitial(string name)
        {
            return string.IsNullOrWhiteSpace(name) ? "?" : name[0].ToString().ToUpper();
        }

        // 3. Logic for Category Color
        public static string GetCategoryColor(string category)
        {
            return category switch
            {
                "Streaming" => "#E50914", // Red
                "Music" => "#1DB954",     // Green
                "Gaming" => "#107C10",    // Xbox Green
                "Gym" => "#007AFF",       // Blue
                "Education" => "#FF9900", // Orange
                "Software" => "#808080",  // Gray
                _ => "#505050"            // Dark Gray
            };
        }
    }
}