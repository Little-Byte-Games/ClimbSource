using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Climb.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Climb.Controllers
{
    // TODO: Replace Details with creating new LeagueUser page.
    public class LeagueUsersController : ModelController
    {
        private readonly ClimbContext _context;
        private readonly ICdnService cdnService;
        private readonly LeagueUserService leagueUserService;

        public LeagueUsersController(ClimbContext context, ICdnService cdnService, IUserService userService, UserManager<ApplicationUser> userManager)
            : base(userService, userManager)
        {
            _context = context;
            this.cdnService = cdnService;
            leagueUserService = new LeagueUserService(context);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return NotFound();
            }

            var leagueUser = await _context.LeagueUser.Include(l => l.League).Include(l => l.User).SingleOrDefaultAsync(m => m.ID == id);
            if(leagueUser == null)
            {
                return NotFound();
            }

            ViewBag.ProfilePic = cdnService.GetProfilePic(leagueUser);

            var viewModel = new GenericViewModel<LeagueUser>(user, leagueUser);
            return View(viewModel);
        }

        public async Task<IActionResult> GetUserTrend(int id)
        {
            var leagueUser = await _context.LeagueUser
                .Include(lu => lu.RankSnapshots)
                .SingleOrDefaultAsync(lu => lu.ID == id);
            if(leagueUser == null)
            {
                return NotFound();
            }

            var rankDifference = leagueUser.GetRankTrendDelta();
            return Ok(new {leagueUserID = id, rankDelta = rankDifference });
        }

        public async Task<string> FavoriteCharacter(int id)
        {
            var character = await leagueUserService.GetFavoriteCharacter(id);
            return character?.Name ?? "None";
        }
    }
}