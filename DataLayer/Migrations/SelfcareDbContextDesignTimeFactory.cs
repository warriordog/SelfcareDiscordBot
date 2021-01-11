using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using SelfcareBot.DataLayer.context;

namespace SelfcareBot.DataLayer.migrations
{
    public class SelfcareDbContextDesignTimeFactory : IDesignTimeDbContextFactory<SelfcareDbContext>
    {
        public SelfcareDbContext CreateDbContext(string[] args)
        {
            return new(new OptionsWrapper<SelfcareDatabaseOptions>(new SelfcareDatabaseOptions()
            {
                ConnectionString = "DataSource=selfcare.db"
            }));
        }
    }
}