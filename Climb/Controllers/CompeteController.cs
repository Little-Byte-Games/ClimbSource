using System.Collections.Generic;
using Climb.Models;
using Climb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Climb.Controllers
{
    public class CompeteController : Controller
    {
        private readonly ClimbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompeteController(ClimbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var appUser = await _userManager.GetUserAsync(User);
            var userID = appUser.UserID;

            var users = _context.User.Select(u => u).ToList();
            var user = users.Single(u => u.ID == userID);

            var sets = await _context.Set
                .Include(s => s.Season).ThenInclude(s => s.League)
                .Include(s => s.Player1).ThenInclude(u => u.User)
                .Include(s => s.Player2).ThenInclude(u => u.User)
                .Where(s => s.Player1.UserID == userID || s.Player2.UserID == userID).ToListAsync();

            var viewModel = new CompeteIndexViewModel(user, new ReadOnlyCollection<User>(users), sets);
            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> Schedule(int? leagueID, int? seasonID)
        {
            var appUser = await _userManager.GetUserAsync(User);
            var userID = appUser.UserID;

            var user = await _context.User
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League)
                .SingleOrDefaultAsync(u => u.ID == userID);
            if(user == null)
            {
                return NotFound();
            }

            var leagues = user.LeagueUsers.Select(ul => ul.League).ToList();
            var leagueUser = user.LeagueUsers.SingleOrDefault(lu => lu.LeagueID == leagueID) ?? user.LeagueUsers.FirstOrDefault();

            var selectedLeague = leagues.SingleOrDefault(l => l.ID == leagueID);
            List<Season> selectedSeasons = null;

            if (leagueUser != null)
            {
                var seasons = await _context.Season
                        .Include(s => s.Participants)
                        .Include(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User)
                        .Include(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User)
                        .ToListAsync();
                selectedSeasons = seasons.Where(season => season.Participants != null && season.Participants.Any(lus => lus.LeagueUserID == leagueUser.ID)).ToList(); 
            }

            var selectedSeason = selectedSeasons?.SingleOrDefault(s => s.ID == seasonID) ?? selectedSeasons?.FirstOrDefault();

            var viewModel = new CompeteScheduleViewModel(selectedLeague, selectedSeason, leagues, selectedSeasons, leagueUser);
            return View(viewModel);
        }

        public async Task<IActionResult> Home(int? userID)
        {
            if(userID == null)
            {
                var appUser = await _userManager.GetUserAsync(User);
                userID = appUser.UserID;
            }

            var user = _context.User
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League)
                .SingleOrDefault(u => u.ID == userID);
            if(user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> Leagues()
        {
            var appUser = await _userManager.GetUserAsync(User);
            var user = await _context.User.Include(u => u.LeagueUsers).SingleOrDefaultAsync(u => u.ID == appUser.UserID);
            var leagues = await _context.League
                .Include(l => l.Members)
                .Include(l => l.Admin)
                .Include(l => l.Game)
                .ToListAsync();

            var viewModel = CompeteLeaguesViewModel.Create(appUser.UserID, user.LeagueUsers, leagues);
            return View(viewModel);
        }
    }
}