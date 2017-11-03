using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services
{
    public interface IUserService
    {
        Task<bool> DoesUserExist(int id);
        Task<User> GetUserForViewAsync(ApplicationUser appUser);
    }
}