using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SelfcareBot.DataLayer.entities;

namespace SelfcareBot.DataLayer.context
{
    public interface ISelfcareDbContext : IDisposable
    {
        public DbSet<UserScore> UserScores { get; }
        public DbSet<KnownUser> KnownUsers { get; }
        public Task MigrateDbAsync();
    }
}