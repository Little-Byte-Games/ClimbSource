using System.Threading.Tasks;
using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Climb.Controllers
{
    public abstract class ModelController : Controller
    {
        protected readonly IUserService userService;
        protected readonly UserManager<ApplicationUser> userManager;

        protected ModelController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            this.userService = userService;
            this.userManager = userManager;
        }

        protected async Task<User> GetViewUserAsync()
        {
            var appUser = await userManager.GetUserAsync(User);
            return await userService.GetUserForViewAsync(appUser);
        }

        protected IActionResult UserNotFound()
        {
            return NotFound("User could not be found!");
        }
    }
}
