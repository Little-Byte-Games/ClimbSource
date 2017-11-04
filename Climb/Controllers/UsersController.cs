using Climb.Models;
using Climb.Services;
using Climb.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize]
        public async Task<IActionResult> Home(int? id)
        {
            var appUser = await userManager.GetUserAsync(User);
            if (id == null)
            {
                id = appUser.UserID;
            }

            var user = _context.User
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.RankSnapshots)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Game)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User)
                .SingleOrDefault(u => u.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            var viewingUser = await _context.User
                .Include(u => u.LeagueUsers)
                .SingleOrDefaultAsync(u => u.ID == appUser.UserID);

            var viewModel = CompeteHomeViewModel.Create(user, viewingUser);
            return View(viewModel);
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
