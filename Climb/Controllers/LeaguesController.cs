using Climb.Core;
using Climb.Models;
using Climb.Services;
using Climb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Climb.Controllers
{
    public class LeaguesController : ModelController
    {
        private readonly ClimbContext context;
        private readonly IConfiguration configuration;
        private readonly ILeagueService leagueService;

        public LeaguesController(ClimbContext context, IConfiguration configuration, IUserService userService, ILeagueService leagueService, UserManager<ApplicationUser> userManger)
            : base(userService, userManger)
        {
            this.context = context;
            this.configuration = configuration;
            this.leagueService = leagueService;
        }

        #region Pages
        public async Task<IActionResult> Index()
        {
            var user = await GetViewUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            var leagues = await context.League
                .Include(l => l.Members)
                .Include(l => l.Admin)
                .Include(l => l.Game)
                .ToListAsync();

            var viewModel = CompeteLeaguesViewModel.Create(user, user.LeagueUsers, leagues);
            return View(viewModel);
        }

        public async Task<IActionResult> Home(int id)
        {
            var user = await GetViewUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            var league = await context.League
                .Include(l => l.Members).ThenInclude(lu => lu.User)
                .Include(l => l.Members).ThenInclude(lu => lu.RankSnapshots)
                .Include(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player1)
                .Include(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player2)
                .SingleOrDefaultAsync(l => l.ID == id);
            if (league == null)
            {
                return NotFound();
            }

            var currentSeason = league.CurrentSeason;
            var completedSets = currentSeason?.Sets.Where(s => s.IsComplete && !s.IsBye);

            int? seasonID = null;
            if (currentSeason != null)
            {
                seasonID = currentSeason.ID;
            }
            else if (league.Seasons?.Count > 0)
            {
                seasonID = league.Seasons.Last().ID;
            }

            var viewModel = new LeagueHomeViewModel(user, league, configuration, completedSets, seasonID);
            return View(viewModel);
        }

        public async Task<IActionResult> Schedule(int? leagueID, int? seasonIndex)
        {
            var user = await GetViewUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            var leagues = user.LeagueUsers.Select(ul => ul.League).ToList();
            var leagueUser = user.LeagueUsers.SingleOrDefault(lu => lu.LeagueID == leagueID) ?? user.LeagueUsers.FirstOrDefault();

            var selectedLeague = leagues.SingleOrDefault(l => l.ID == leagueID);
            List<Season> selectedSeasons = null;

            if (leagueUser != null)
            {
                var seasons = await context.Season
                    .Include(s => s.Participants)
                    .Include(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User)
                    .Include(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User)
                    .ToListAsync();
                selectedSeasons = seasons.Where(season => season.Participants != null && season.Participants.Any(lus => lus.LeagueUserID == leagueUser.ID)).ToList();
            }

            var selectedSeason = selectedSeasons?.SingleOrDefault(s => s.Index == seasonIndex) ?? selectedSeasons?.FirstOrDefault();

            var viewModel = new CompeteScheduleViewModel(user, selectedLeague, selectedSeason, leagues, selectedSeasons, leagueUser);
            return View(viewModel);
        }
        #endregion

        #region API
        [HttpPost]
        public async Task<IActionResult> TakeAllRankSnapshots([FromServices]IHttpContextAccessor contextAccessor)
        {
            var key = contextAccessor.HttpContext.Request.Headers["key"];
            if (key == "steve")
            {
                var leagues = await context.League
                    .Include(l => l.Members).ThenInclude(lu => lu.RankSnapshots)
                    .Include(l => l.Members).ThenInclude(lu => lu.User)
                    .ToArrayAsync();
                foreach (var league in leagues)
                {
                    await leagueService.TakeSnapshot(league);
                }
                return Accepted();
            }

            return Forbid();
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Join(int leagueID, int userID)
        {
            var league = await context.League.SingleOrDefaultAsync(l => l.ID == leagueID);
            if(league == null)
            {
                return NotFound();
            }

            var user = await context.User.SingleOrDefaultAsync(u => u.ID == userID);
            if(user == null)
            {
                return NotFound();
            }

            await leagueService.JoinLeague(user, league);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int leagueID, int userID)
        {
            var leagueUser = await context.LeagueUser.SingleOrDefaultAsync(u => u.LeagueID == leagueID && u.UserID == userID);
            if(leagueUser != null)
            {
                leagueUser.HasLeft = true;
                context.Update(leagueUser);
                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> TakeRankSnapshot(int id)
        {
            var league = await context.League
                .Include(l => l.Members).ThenInclude(lu => lu.RankSnapshots)
                .Include(l => l.Members).ThenInclude(lu => lu.User)
                .SingleOrDefaultAsync(l => l.ID == id);
            if(league == null)
            {
                return NotFound();
            }

            var rankSnapshots = await leagueService.TakeSnapshot(league);

            var orderedSnapshots = rankSnapshots.OrderBy(lu => lu.Rank);
            var message = new StringBuilder();
            message.AppendLine($"*{league.Name} PR*");
            foreach(var snapshot in orderedSnapshots)
            {
                message.AppendLine($"{snapshot.Rank} ({snapshot.DisplayDeltaRank}) {snapshot.LeagueUser.User.Username}");
            }
            var apiKey = configuration.GetSection("Slack")["Key"];
            await SlackController.SendGroupMessage(apiKey, message.ToString());

            return RedirectToAction(nameof(Home), new {id});
        }
    }
}
