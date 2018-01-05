using Climb.Models;
using Climb.Services;
using Climb.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Climb.ViewModels._General;

namespace Climb.Controllers
{
    public class ErrorController : ModelController
    {
        public ErrorController(IUserService userService, UserManager<ApplicationUser> userManager)
            : base(userService, userManager)
        {
        }

        [HttpGet("/Error/{statusCode:int}")]
        public async Task<IActionResult> Index(int statusCode, string message = null)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new ErrorPageViewModel(user, statusCode, message);
            return View(viewModel);
        }
    }
}