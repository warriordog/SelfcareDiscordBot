using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using SelfcareBot.DataLayer.context;
using SelfcareBot.DataLayer.entities;

namespace SelfcareBot.Services
{
    public interface IUserService
    {
        public Task<KnownUser> GetOrCreateKnownUserForDiscordUser(DiscordUser discordUser);
    }

    public class UserService : IUserService
    {
        private readonly ISelfcareDbContext _selfcareDb;

        public UserService(ISelfcareDbContext selfcareDb)
        {
            _selfcareDb = selfcareDb;
        }

        public async Task<KnownUser> GetOrCreateKnownUserForDiscordUser(DiscordUser discordUser)
        {
            // Get user (if present)
            var knownUser = await _selfcareDb.KnownUsers.Where(kn => kn.DiscordId == discordUser.Id)
                .FirstOrDefaultAsync();

            // Record user if not present
            if (knownUser == null)
            {
                // Create db user from discord user
                knownUser = new KnownUser
                {
                    DiscordId = discordUser.Id,
                    Username = discordUser.Username,
                    Discriminator = discordUser.Discriminator
                };

                // Add to database
                _selfcareDb.KnownUsers.Add(knownUser);
                await _selfcareDb.SaveChangesAsync();
            }

            return knownUser;
        }
    }
}