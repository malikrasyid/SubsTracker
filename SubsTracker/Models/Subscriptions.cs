using SQLite;
using SubsTracker.Helpers;

namespace SubsTracker.Models
{
    public enum BillingPeriodUnit
    {
        Day,
        Week,
        Month,
        Year
    }

    [Table("Subscriptions")]
    public class Subscription
    {       
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(100)]
        public string Name { get; set; }   
        
        public string Description { get; set; }

        public decimal Price { get; set; }      

        public string Currency { get; set; }

        public int BillingInterval { get; set; } = 1;
        
        public BillingPeriodUnit PeriodUnit { get; set; }
        
        public DateTime FirstBillDate { get; set; }

        public DateTime NextPaymentDate { get; set; }

        public string PaymentMethod { get; set; } 

        public string Category { get; set; }      

        public int ReminderDays { get; set; }

        public bool IsActive { get; set; } = true;

        [Ignore]
        public int DaysUntilDue => SubscriptionHelper.GetDaysUntilDue(NextPaymentDate);

        [Ignore]
        public string Initial => SubscriptionHelper.GetInitial(Name);

        [Ignore]
        public string HexColor => SubscriptionHelper.GetCategoryColor(Category);
    }
}