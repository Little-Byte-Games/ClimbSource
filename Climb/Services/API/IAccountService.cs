using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Climb.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> CreateUser(string email, string password);
    }
}