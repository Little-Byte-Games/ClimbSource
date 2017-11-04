using Climb.Models;
using Climb.Services;
using Climb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Climb.Controllers
{
    public class CompeteController : ModelController
    {
        private readonly ClimbContext _context;

        public CompeteController(ClimbContext context, UserManager<ApplicationUser> userManager, IUserService userService)
            : base(userService, userManager)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Fight(int id)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return NotFound();
            }

            var set = await _context.Set
                .Include(s => s.Matches)
                .Include(s => s.Season)
                .Include(s => s.League).ThenInclude(l => l.Game).ThenInclude(g => g.Characters)
                .Include(s => s.League).ThenInclude(l => l.Game).ThenInclude(g => g.Stages)
                .Include(s => s.Player1).ThenInclude(lu => lu.User)
                .Include(s => s.Player2).ThenInclude(lu => lu.User)
                .SingleOrDefaultAsync(s => s.ID == id);
            if(set == null)
            {
                return NotFound();
            }

            var viewModel = new GenericViewModel<Set>(user, set);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Exhibition(int challengerID, int challengedID)
        {
            var challenged = await _context.LeagueUser.SingleOrDefaultAsync(lu => lu.ID == challengedID);
            if(challenged == null)
            {
                return NotFound($"No challenged league user with id '{challengedID} found.");
            }

            var challenger = await _context.User
                .Include(u => u.LeagueUsers)
                .SingleOrDefaultAsync(u => u.ID == challengerID);
            if(challenger == null)
            {
                return NotFound($"No challenger user with id '{challengerID} found.");
            }

            var challengerLeagueUser = challenger.LeagueUsers.SingleOrDefault(lu => lu.LeagueID == challenged.LeagueID);
            if(challengerLeagueUser == null)
            {
                return NotFound($"No challenger league user for league ID '{challenged.LeagueID}' found.");
            }

            var set = new Set
            {
                Player1ID = challengerLeagueUser.ID,
                Player2ID = challengedID,
                DueDate = DateTime.Now.Date,
                LeagueID = challengerLeagueUser.LeagueID,
            };
            await _context.Set.AddAsync(set);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Fight), new {id = set.ID});
        }
    }
}