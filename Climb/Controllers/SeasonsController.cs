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
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new CreateViewModel(user);
            return View(viewModel);
        }
        #endregion

        #region API
        public class Participant
        {
            public int UserID { get; set; }
            public int DivisionIndex { get; set; }
        }

        public class CreateData
        {
            public int LeagueID { get; set; }
            public DateTime StartDate { get; set; }
            public IList<Participant> Participants { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateData data)
        {
            var league = await context.League
                .Include(l => l.Seasons)
                .SingleOrDefaultAsync(l => l.ID == data.LeagueID);
            if (league == null)
            {
                return NotFound();
            }

            await seasonService.Create(league, data.StartDate);
            return Ok($"leagueID={data.LeagueID}, startDate={data.StartDate}, participants={string.Join("; ", data.Participants.Select(p => $"UserID={p.UserID}, Division={p.DivisionIndex}"))}");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuto(int leagueID)
        {
            var league = await context.League
                .Include(l => l.Seasons)
                .Include(l => l.Members)
                .SingleOrDefaultAsync(l => l.ID == leagueID);
            if (league == null)
            {
                return NotFound($"Could not find league with ID '{leagueID}.'");
            }

            var season = await seasonService.Create(league);

            var participants = new HashSet<Participant>();
            var i = 0;
            foreach (var member in league.Members)
            {
                participants.Add(new Participant { UserID = member.ID, DivisionIndex = i });
                i = (i + 1) % 2;
            }
            await seasonService.Start(season, participants);
            return Ok($"leagueID={leagueID}, startDate={season.StartDate}");
        }
        #endregion

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateForLeague(int leagueID, DateTime? startDate)
        {
            var league = await context.League
                .Include(l => l.Seasons)
                .SingleOrDefaultAsync(l => l.ID == leagueID);
            if (league == null)
            {
                return NotFound();
            }

            await seasonService.Create(league, startDate);

            return RedirectToAction(nameof(LeaguesController.Home), "Leagues");
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
            if (season == null)
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
            if (season == null)
            {
                return NotFound();
            }

            var leagueUser = await context.LeagueUser.SingleOrDefaultAsync(u => u.ID == userID);
            if (leagueUser == null)
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

        public async Task<IActionResult> GetStatus(int id)
        {
            var season = await context.Season
                .Include(s => s.Sets)
                .SingleOrDefaultAsync(s => s.ID == id);
            if (season == null)
            {
                return NotFound();
            }

            var status = season.GetStatus();
            return Ok(JsonConvert.SerializeObject(status));
        }
    }
}
