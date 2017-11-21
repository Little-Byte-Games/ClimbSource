using Climb.Models;
using Climb.Services;
using Climb.ViewModels.Seasons;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Climb.Controllers
{
    public partial class SeasonsController : ModelController
    {
        private readonly ClimbContext context;
        private readonly ISeasonService seasonService;

        public SeasonsController(ClimbContext context, ISeasonService seasonService, IUserService userService, UserManager<ApplicationUser> userManager)
            : base(userService, userManager)
        {
            this.context = context;
            this.seasonService = seasonService;
        }

        #region Pages
        public async Task<IActionResult> Create(int leagueID)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return UserNotFound();
            }

            var viewModel = new CreateViewModel(user);
            return View(viewModel);
        }

        public async Task<IActionResult> Home(int id)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return UserNotFound();
            }

            var season = await context.Season
                .Include(s => s.Sets).ThenInclude(s => s.League)
                .Include(s => s.Participants).ThenInclude(lus => lus.LeagueUser).ThenInclude(lu => lu.User)
                .SingleOrDefaultAsync(s => s.ID == id);
            if(season == null)
            {
                return NotFound($"No season with ID '{id}' found.");
            }

            var viewModel = HomeViewModel.Create(user, season);
            return View(viewModel);
        }
        #endregion

        #region API
        [HttpPost]
        public async Task<IActionResult> EndSeason(int id)
        {
            await seasonService.End(id);

            return Ok($"Season {id} has been ended.");
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> CreateForLeague(int leagueID, DateTime? startDate)
        {
            var league = await context.League
                .Include(l => l.Seasons)
                .SingleOrDefaultAsync(l => l.ID == leagueID);
            if(league == null)
            {
                return NotFound();
            }

            var season = await seasonService.Create(league, startDate);

            return CreatedAtAction(nameof(CreateForLeague), season);
        }

        public class JoinList
        {
            public readonly Season season;
            public readonly IEnumerable<LeagueUser> nonparticipants;

            public JoinList(Season season, IEnumerable<LeagueUser> nonparticipants)
            {
                this.season = season;
                this.nonparticipants = nonparticipants;
            }
        }

        public async Task<IActionResult> Join(int id)
        {
            var season = await context.Season
                .Include(s => s.Participants).ThenInclude(user => user.LeagueUser).ThenInclude(u => u.User)
                .SingleOrDefaultAsync(s => s.ID == id);
            if(season == null)
            {
                return NotFound();
            }

            var nonparticipants = await context.LeagueUser
                .Where(u => u.LeagueID == season.LeagueID && season.Participants.All(lus => lus.LeagueUserID != u.ID))
                .Include(leagueUser => leagueUser.User)
                .ToListAsync();

            return View(new JoinList(season, nonparticipants));
        }

        [HttpPost]
        public async Task<IActionResult> Join(int seasonID, int userID)
        {
            var season = await context.Season.Include(s => s.Participants).SingleOrDefaultAsync(s => s.ID == seasonID);
            if(season == null)
            {
                return NotFound();
            }

            var leagueUser = await context.LeagueUser.SingleOrDefaultAsync(u => u.ID == userID);
            if(leagueUser == null)
            {
                return NotFound();
            }

            await seasonService.Join(season, leagueUser);

            return RedirectToAction(nameof(Join), new { id = seasonID });
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int seasonID, int userID)
        {
            var season = await context.Season.Include(s => s.Participants).SingleOrDefaultAsync(s => s.ID == seasonID);
            if (season == null)
            {
                return NotFound();
            }

            var leagueUser = await context.LeagueUser.SingleOrDefaultAsync(u => u.ID == userID && u.LeagueID == season.LeagueID);
            if (leagueUser == null)
            {
                return NotFound();
            }

            season.Participants.RemoveWhere(lus => lus.LeagueUserID == leagueUser.ID);
            context.Update(season);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Join), new { id = seasonID });
        }

        public async Task<IActionResult> Start(int id)
        {
            var season = await context.Season.Include(s => s.Sets)
                .Include(s => s.Participants).ThenInclude(lus => lus.LeagueUser).ThenInclude(u => u.User)
                .SingleOrDefaultAsync(s => s.ID == id);

            if (season == null)
            {
                return NotFound();
            }

            var viewData = new SeasonStartViewModel(season);
            return View(viewData);
        }

        [HttpPost]
        [ActionName("Start")]
        public async Task<IActionResult> StartPost(int id)
        {
            var season = await context.Season
                .Include(s => s.League).ThenInclude(l => l.Members)
                .Include(s => s.Sets).ThenInclude(s => s.League)
                .Include(s => s.Participants).ThenInclude(lus => lus.LeagueUser).ThenInclude(u => u.User)
                .SingleOrDefaultAsync(s => s.ID == id);
            if (season == null)
            {
                return NotFound();
            }

            await seasonService.JoinAll(season);
            await seasonService.Start(season);
            
            return Ok("Season started.");
        }

        public async Task<IActionResult> GetStatus(int id)
        {
            var season = await context.Season
                .Include(s => s.Sets)
                .SingleOrDefaultAsync(s => s.ID == id);
            if(season == null)
            {
                return NotFound();
            }

            var status = season.GetStatus();
            return Ok(JsonConvert.SerializeObject(status));
        }
    }
}
