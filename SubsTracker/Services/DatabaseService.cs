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
                    new Subscription { Name = "Netflix", Price = 15.99m, ExpiryDate = DateTime.Now.AddDays(5) },
                    new Subscription { Name = "Spotify", Price = 9.99m, ExpiryDate = DateTime.Now.AddDays(12) },
                    new Subscription { Name = "Adobe Cloud", Price = 52.99m, ExpiryDate = DateTime.Now.AddDays(20) },
                    new Subscription { Name = "Xbox GamePass", Price = 14.99m, ExpiryDate = DateTime.Now.AddDays(2) },
                    new Subscription { Name = "Gym Membership", Price = 40.00m, ExpiryDate = DateTime.Now.AddDays(25) }
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