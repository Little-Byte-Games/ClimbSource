using Climb.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Climb.Services
{
    public interface IAccountService
    {
        Task<(ApplicationUser user, IdentityResult result)> CreateUser(string email, string username, string password);
    }
}