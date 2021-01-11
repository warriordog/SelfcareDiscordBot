using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        
        public Task MigrateAsync() => Database.MigrateAsync();

        public Task<int> SaveChangesAsync() => base.SaveChangesAsync();

        public Task<IDbContextTransaction> BeginTransactionAsync() => Database.BeginTransactionAsync();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                // Automatically resolve navigation properties on access
                .UseLazyLoadingProxies()
                
                // Connect to local SQLite DB
                .UseSqlite(_options.ConnectionString)
            ;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KnownUser>()
                .HasAlternateKey(kn => kn.DiscordId);
        }
    }
}