using SQLite;

namespace SubsTracker.Models
{
    [Table("Subscriptions")]
    public class Subscription
    {       
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(100)]
        public string Name { get; set; }       

        public decimal Price { get; set; }      

        public string Currency { get; set; }    

        public DateTime ExpiryDate { get; set; }

        public string BillingCycle { get; set; } 

        public DateTime FirstBillDate { get; set; }

        public DateTime NextPaymentDate { get; set; }

        public string PaymentMethod { get; set; } 

        public string Category { get; set; }      

        public string Notes { get; set; }

        public int ReminderDays { get; set; }

        public string HexColor { get; set; }      

        public bool IsActive { get; set; } = true;



        [Ignore]
        public string DisplayPrice
        {
            get => $"{Currency} {Price:N0}"; 
        }

        [Ignore]
        public int DaysUntilDue
        {
            get => (NextPaymentDate.Date - DateTime.Now.Date).Days;
        }

        [Ignore]
        public string DueDateString
        {
            get
            {
                if (DaysUntilDue == 0) return "Due Today";
                if (DaysUntilDue == 1) return "Due Tomorrow";
                if (DaysUntilDue < 0) return $"Overdue by {Math.Abs(DaysUntilDue)} days";
                return $"Due in {DaysUntilDue} days";
            }
        }

        [Ignore]
        public Color ColorObj
        {            
            get
            {
                if (string.IsNullOrEmpty(HexColor)) return Colors.Gray;
                return Color.FromArgb(HexColor);
            }
        }
    }
}