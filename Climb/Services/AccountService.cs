using Climb.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Climb.Services
{
    public class AccountService : IAccountService
    {
        private readonly ClimbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AccountService> logger;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountService(ClimbContext context, UserManager<ApplicationUser> userManager, ILogger<AccountService> logger, SignInManager<ApplicationUser> signInManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
            this.signInManager = signInManager;
        }

        public async Task<IdentityResult> CreateUser(string email, string username, string password)
        {
            var user = new User { Username = username };
            await context.User.AddAsync(user);
            await context.SaveChangesAsync();

            var applicationUser = new ApplicationUser { UserName = email, Email = email, User = user };
            var result = await userManager.CreateAsync(applicationUser, password);
            if (result.Succeeded)
            {
                logger.LogInformation("User created a new account with password.");

                await signInManager.PasswordSignInAsync(applicationUser, password, isPersistent: false, lockoutOnFailure: false);
                logger.LogInformation("User created a new account with password.");
            }

            return result;
        }
    }
}
