using SQLite;

namespace SubsTracker.Models
{
    public enum BillingCycle
    {
        Weekly,
        Monthly,
        Yearly
    }

    [Table("Subscriptions")]
    public class Subscription
    {       
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [SQLite.MaxLength(100)]
        public string Name { get; set; }       

        public decimal Cost { get; set; }      

        public string Currency { get; set; }    

        public BillingCycle BillingCycle { get; set; } 

        public DateTime FirstBillDate { get; set; }

        public DateTime NextPaymentDate { get; set; }

        public string PaymentMethod { get; set; } 

        public string Category { get; set; }      

        public string Notes { get; set; }

        public int ReminderDays { get; set; }

        public bool IsActive { get; set; } = true;

        [Ignore]
        public int DaysUntilDue
        {
            get => (NextPaymentDate.Date - DateTime.Now.Date).Days;
        }

        [Ignore]
        public string Initial => string.IsNullOrWhiteSpace(Name) ? "?" : Name[0].ToString().ToUpper();

        [Ignore]
        public string HexColor
        {
            get
            {
                return Category switch
                {
                    "Streaming" => "#E50914", 
                    "Music" => "#1DB954",     
                    "Gaming" => "#107C10",    
                    "Gym" => "#007AFF",       
                    _ => "#808080"           
                };
            }
        }
    }
}