using System.Threading.Tasks;
using Climb.Models;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services
{
    public class UserService : IUserService
    {
        private readonly DbSet<User> userDb;

        public UserService(ClimbContext context)
        {
            userDb = context.User;
        }

        public async Task<bool> DoesUserExist(int id)
        {
            return await userDb.AnyAsync(u => u.ID == id);
        }

        public async Task<User> GetUserForViewAsync(ApplicationUser appUser)
        {
            if(appUser == null)
            {
                return null;
            }

            var user = await userDb
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.RankSnapshots)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Game)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User)
                .SingleOrDefaultAsync(u => u.ID == appUser.UserID);

            return user;
        }
    }
}
