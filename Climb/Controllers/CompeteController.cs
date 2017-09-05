using Climb.Models;
using Climb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Climb.Controllers
{
    public class CompeteController : Controller
    {
        private readonly ClimbContext _context;

        public CompeteController(ClimbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int userID)
        {
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

        public async Task<IActionResult> Schedule(int userID, int? leagueID, int? seasonID)
        {
            var user = await _context.User
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League)
                .SingleOrDefaultAsync(u => u.ID == userID);
            if(user == null)
            {
                return NotFound();
            }

            var leagues = user.LeagueUsers.Select(ul => ul.League).ToList();

            var leagueUser = user.LeagueUsers.First();
            if(leagueID != null)
            {
                leagueUser = user.LeagueUsers.Single(lu => lu.LeagueID == leagueID);
            }

            var seasons = await _context.Season.Include(s => s.Participants).Include(s => s.Sets).ToListAsync();
            var selectedSeasons = seasons.Where(season => season.Participants.Any(lus => lus.LeagueUserID == leagueUser.ID)).ToList();

            var viewModel = new CompeteScheduleViewModel(leagues, selectedSeasons, leagueID ?? leagues.First().ID, seasonID ?? selectedSeasons.First().ID);
            return View(viewModel);
        }
    }
}