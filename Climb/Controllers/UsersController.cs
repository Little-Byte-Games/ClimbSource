using Climb.Models;
using Climb.Services;
using Climb.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Climb.Controllers
{
    public class UsersController : ModelController
    {
        private readonly ClimbContext _context;

        public UsersController(ClimbContext context, IUserService userService, UserManager<ApplicationUser> userManager)
            : base(userService, userManager)
        {
            _context = context;
        }

        public async Task<IActionResult> AvailableSets(int id)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return NotFound();
            }

            var sets = await _context.Set
                .Include(s => s.Season).ThenInclude(s => s.League)
                .Include(s => s.Player1).ThenInclude(u => u.User)
                .Include(s => s.Player2).ThenInclude(u => u.User)
                .Where(s => user.LeagueUsers.Any(u => u.ID == s.Player1ID || u.ID == s.Player2ID)).ToListAsync();

            var viewData = new AvailableSetsViewModel(user, sets);
            return View(viewData);
        }
    }
}
