using System.Threading.Tasks;
using Climb.Models;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services
{
    public class UserService
    {
        private readonly DbSet<User> userDb;

        public UserService(DbSet<User> userDb)
        {
            this.userDb = userDb;
        }

        public async Task<bool> DoesUserExist(int id)
        {
            return await userDb.AnyAsync(u => u.ID == id);
        }
    }
}
