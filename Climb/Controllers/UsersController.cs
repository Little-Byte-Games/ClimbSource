using Climb.Models;
using Climb.Services;
using Climb.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Climb.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;

namespace Climb.Controllers
{
    public class UsersController : ModelController
    {
        private readonly ClimbContext context;

        public UsersController(ClimbContext context, IUserService userService, UserManager<ApplicationUser> userManager)
            : base(userService, userManager)
        {
            this.context = context;
        }

        #region Pages
        [Authorize]
        public async Task<IActionResult> Home(int? id)
        {
            var appUser = await userManager.GetUserAsync(User);
            if (id == null)
            {
                id = appUser.UserID;
            }

            var user = context.User
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.RankSnapshots)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Game)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User)
                .SingleOrDefault(u => u.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            var viewingUser = await context.User
                .Include(u => u.LeagueUsers)
                .SingleOrDefaultAsync(u => u.ID == appUser.UserID);

            var viewModel = CompeteHomeViewModel.Create(user, viewingUser);
            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> Account()
        {
            var user = await GetViewUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserAccountViewModel(user);
            return View(viewModel);
        }
        #endregion

        #region API
        public async Task<IActionResult> AvailableSets(int id)
        {
            var user = await GetViewUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            var sets = await context.Set
                .Include(s => s.Season).ThenInclude(s => s.League)
                .Include(s => s.Player1).ThenInclude(u => u.User)
                .Include(s => s.Player2).ThenInclude(u => u.User)
                .Where(s => user.LeagueUsers.Any(u => u.ID == s.Player1ID || u.ID == s.Player2ID)).ToListAsync();

            var viewData = new AvailableSetsViewModel(user, sets);
            return View(viewData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("ID,Username")] User user)
        {
            if (id != user.ID)
            {
                return NotFound($"ID's do not match {id} vs {user.ID}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(user);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    var userExists = await userService.DoesUserExist(user.ID);
                    if (!userExists)
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction(nameof(Account));
            }

            var viewUser = await GetViewUserAsync();
            var viewModel = new UserAccountViewModel(viewUser);
            return View(nameof(Account), viewModel);
        }
        #endregion
    }
}
