using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SelfcareBot.DataLayer.entities;

namespace SelfcareBot.DataLayer.context
{
    public interface ISelfcareDbContext : IDisposable
    {
        public DbSet<UserScore> UserScores { get; }
        public DbSet<KnownUser> KnownUsers { get; }
        
        public Task MigrateAsync();
        public Task<int> SaveChangesAsync();
        public Task<IDbContextTransaction> BeginTransactionAsync();
    }
}