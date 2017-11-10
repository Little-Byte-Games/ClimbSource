using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Climb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClimbContext context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICdnService cdnService;

        public HomeController(ClimbContext context, SignInManager<ApplicationUser> signInManager, ICdnService cdnService)
        {
            this.context = context;
            _signInManager = signInManager;
            this.cdnService = cdnService;
        }

        public IActionResult Index()
        {
            if(_signInManager.IsSignedIn(User))
            {
                return RedirectToAction(nameof(UsersController.Home), "Users");
            }

            return RedirectToAction("Login", "Account");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // TODO: Move to league user controller.
        [HttpPost]
        public async Task<IActionResult> UploadProfilePic(int id, IFormFile file)
        {
            var fileKey = await cdnService.UploadProfilePic(file);

            var user = await context.LeagueUser.SingleOrDefaultAsync(lu => lu.ID == id);
            if (user != null)
            {
                user.ProfilePicKey = fileKey;
                context.Update(user);
                await context.SaveChangesAsync();
            }

            return View("Index");
        }
    }
}
