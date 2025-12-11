using SQLite;
using SubsTracker.Models;

namespace SubsTracker.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        async Task Init()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "SubTrack.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<Subscription>();

            await SeedDatabase();
        }

        private async Task SeedDatabase()
        {
            // Check if the table is empty
            var count = await _database.Table<Subscription>().CountAsync();

            if (count == 0)
            {
                // If empty, add dummy data
                var dummyData = new List<Subscription>
                {
                    // 1. Standard Monthly (Netflix)
                    new Subscription
                    {
                        Name = "Netflix",
                        Description = "Premium Family Plan (4k)",
                        Price = 18,
                        BillingInterval = 1,
                        PeriodUnit = BillingPeriodUnit.Month,
                        NextPaymentDate = DateTime.Now.AddDays(5),
                        Category = "Streaming",
                        Currency = "USD"
                    },

                    // 2. Weekly (Gym - specific coaching)
                    new Subscription
                    {
                        Name = "Personal Trainer",
                        Description = "Weekly coaching session",
                        Price = 25,
                        BillingInterval = 1,
                        PeriodUnit = BillingPeriodUnit.Week,
                        NextPaymentDate = DateTime.Now.AddDays(2),
                        Category = "Gym",
                        Currency = "USD"
                    },

                    // 3. Yearly (VPN)
                    new Subscription
                    {
                        Name = "NordVPN",
                        Description = "2-Year Plan charged annually",
                        Price = 69,
                        BillingInterval = 1,
                        PeriodUnit = BillingPeriodUnit.Year,
                        NextPaymentDate = DateTime.Now.AddMonths(4),
                        Category = "Software",
                        Currency = "USD"
                    },

                    // 4. Flexible (Spotify Duo - Every 3 Months scenario?)
                    // Let's stick to standard Monthly for music
                    new Subscription
                    {
                        Name = "Spotify Duo",
                        Description = "Music for two",
                        Price = 12,
                        BillingInterval = 1,
                        PeriodUnit = BillingPeriodUnit.Month,
                        NextPaymentDate = DateTime.Now.AddDays(12),
                        Category = "Music",
                        Currency = "USD"
                    },

                    // 5. Gaming (Xbox)
                    new Subscription
                    {
                        Name = "Xbox Game Pass",
                        Description = "Ultimate Tier",
                        Price = 16,
                        BillingInterval = 1,
                        PeriodUnit = BillingPeriodUnit.Month,
                        NextPaymentDate = DateTime.Now.AddDays(1),
                        Category = "Gaming",
                        Currency = "USD"
                    }
                };

                await _database.InsertAllAsync(dummyData);
            }
        }

        public async Task<List<Subscription>> GetSubscriptionsAsync()
        {
            await Init();
            return await _database.Table<Subscription>()
                                  .OrderBy(s => s.NextPaymentDate)
                                  .ToListAsync();
        }

        public async Task<Subscription> GetSubscriptionByIdAsync(int id)
        {
            await Init();
            return await _database.Table<Subscription>()
                                  .Where(i => i.Id == id)
                                  .FirstOrDefaultAsync();
        }

        public async Task<int> SaveSubscriptionAsync(Subscription item)
        {
            await Init();
            if (item.Id != 0)
            {
                return await _database.UpdateAsync(item);
            }
            else
            {
                return await _database.InsertAsync(item);
            }
        }

        public async Task<int> DeleteSubscriptionAsync(Subscription item)
        {
            await Init();
            return await _database.DeleteAsync(item);
        }

        public async Task<List<CategoryTotal>> GetCategoryTotalsAsync()
        {
            await Init();

            // Note: SQLite-net-pcl doesn't support complex LINQ GroupBy fully in async.
            // It's safer to fetch all and group in memory for small datasets, 
            // OR use a raw SQL query like below:

            var query = "SELECT Category, SUM(Price) as TotalAmount FROM Subscriptions WHERE IsActive = 1 GROUP BY Category";
            return await _database.QueryAsync<CategoryTotal>(query);
        }
    }

    // Helper class for the Insights query result
    public class CategoryTotal
    {
        public string Category { get; set; }
        public decimal TotalAmount { get; set; }
    }
}