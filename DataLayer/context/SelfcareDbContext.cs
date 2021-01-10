using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SelfcareBot.DataLayer.entities;

namespace SelfcareBot.DataLayer.context
{
    public class SelfcareDbContext : DbContext, ISelfcareDbContext
    {
        public DbSet<UserScore> UserScores { get; set; }
        public DbSet<KnownUser> KnownUsers { get; set; }

        private readonly SelfcareDatabaseOptions _options;
        
        public SelfcareDbContext(IOptions<SelfcareDatabaseOptions> options)
        {
            _options = options.Value;
        }
        
        public Task MigrateDbAsync()
        {
            return Database.MigrateAsync();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                // Automatically resolve navigation properties on access
                .UseLazyLoadingProxies()
                
                // Connect to local SQLite DB
                .UseSqlite(_options.ConnectionString)
            ;
        }
    }
}