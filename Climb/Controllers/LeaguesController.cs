using Climb.Consts;
using Climb.Models;
using Climb.Services;
using Climb.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Climb.Extensions;
using Microsoft.AspNetCore.Authorization;

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
            if(user == null)
            {
                return NotFound();
            }

            var leagues = await context.League
                .Include(l => l.Members)
                .Include(l => l.Admin)
                .Include(l => l.Game)
                .ToListAsync();

            var games = await context.Game.OrderBy(g => g.Name).ToListAsync();

            var viewModel = CompeteLeaguesViewModel.Create(user, user.LeagueUsers, leagues, games);
            return View(viewModel);
        }

        public async Task<IActionResult> Home(int id)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return UserNotFound();
            }

            var league = await context.League.AsNoTracking()
                .Include(l => l.Game).AsNoTracking()
                .SingleOrDefaultAsync(l => l.ID == id);
            if(league == null)
            {
                return NotFound();
            }

            var viewModel = new LeagueHomeViewModel(user, league);
            return View(viewModel);
        }

        public async Task<IActionResult> PowerRanks(int id)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return NotFound();
            }

            var league = await context.League
                .Include(l => l.Game)
                .Include(l => l.Members)
                .Include(l => l.Members).ThenInclude(lu => lu.RankSnapshots)
                .Include(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player1)
                .Include(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player2)
                .SingleOrDefaultAsync(l => l.ID == id);
            if(league == null)
            {
                return NotFound();
            }

            var viewModel = new LeaguePowerRankViewModel(user, league, configuration);
            return View(viewModel);
        }

        public async Task<IActionResult> Schedule(int? leagueID, int? seasonIndex)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return NotFound();
            }

            var leagues = user.LeagueUsers.Select(ul => ul.League).ToList();
            var leagueUser = user.LeagueUsers.SingleOrDefault(lu => lu.LeagueID == leagueID) ?? user.LeagueUsers.FirstOrDefault();

            var selectedLeague = leagues.SingleOrDefault(l => l.ID == leagueID);
            List<Season> selectedSeasons = null;

            if(leagueUser != null)
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
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> TakeAllRankSnapshots([FromServices] IHttpContextAccessor contextAccessor)
        {
            var sentKey = contextAccessor.HttpContext.Request.Headers["key"];
            var localKey = configuration.GetSection("Jobs")["SecretKey"];
            if(sentKey == localKey)
            {
                var leagues = await context.League
                    .Include(l => l.Sets).AsNoTracking()
                    .Include(l => l.Members).ThenInclude(lu => lu.RankSnapshots)
                    .Include(l => l.Members).ThenInclude(lu => lu.User)
                    .ToArrayAsync();
                foreach(var league in leagues)
                {
                    var rankSnapshots = await leagueService.TakeSnapshot(league);
                    await leagueService.SendSnapshotUpdate(rankSnapshots, league);
                }

                return Ok();
            }

            return BadRequest("Incorrect key");
        }

        [HttpPost]
        public async Task<IActionResult> SendSetReminders([FromServices] IHttpContextAccessor contextAccessor)
        {
            var sentKey = contextAccessor.HttpContext.Request.Headers["key"];
            var localKey = configuration.GetSection("Jobs")["SecretKey"];
            if(sentKey == localKey)
            {
                var leagues = await context.League
                    .Include(l => l.Seasons).ThenInclude(s => s.Participants).ThenInclude(lu => lu.LeagueUser).ThenInclude(lu => lu.User)
                    .Include(l => l.Seasons).ThenInclude(s => s.Sets)
                    .ToArrayAsync();
                foreach(var league in leagues)
                {
                    await leagueService.SendSetReminders(league, Url);
                }

                return Ok();
            }

            return BadRequest("Incorrect key");
        }

        [HttpPost]
        public async Task<IActionResult> Create(League league)
        {
            if(ModelState.IsValid)
            {
                var admin = await context.User.FirstOrDefaultAsync(u => u.ID == league.AdminID);
                if (admin == null)
                {
                    return BadRequest($"No User with ID '{league.AdminID}' found.");
                }

                var gameExists = await context.Game.AnyAsync(g => g.ID == league.GameID);
                if (!gameExists)
                {
                    return BadRequest($"No Game with ID '{league.GameID}' found.");
                }

                var regex = new Regex(@"\s+");
                var strippedName = regex.Replace(league.Name.ToLower(), string.Empty);
                var isNameTaken = await context.League.AnyAsync(l => regex.Replace(l.Name.ToLower(), string.Empty) == strippedName);
                if(isNameTaken)
                {
                    return BadRequest($"League with name similar to '{league.Name}' already exists.");
                }

                await context.AddAsync(league);
                await leagueService.JoinLeague(admin, league);
                await context.SaveChangesAsync();

                return CreatedAtAction("Home", league);
            }

            return BadRequest(ModelState.GetErrors());
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
            if(FeatureToggles.Slack)
            {
                await leagueService.SendSnapshotUpdate(rankSnapshots, league);
            }

            return RedirectToAction(nameof(Home), new {id});
        }
    }
}