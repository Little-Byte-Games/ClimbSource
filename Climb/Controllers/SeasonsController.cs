using Climb.Consts;
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
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Create", new {leagueID})});
            }

            var viewModel = new CreateViewModel(user);
            return View(viewModel);
        }

        public async Task<IActionResult> Home(int id)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Home", new { id })});
            }

            var season = await context.Season
                .Include(s => s.League).ThenInclude(l => l.Game)
                .Include(s => s.Sets)
                .Include(s => s.Participants).ThenInclude(lus => lus.LeagueUser)
                .SingleOrDefaultAsync(s => s.ID == id);
            if(season == null)
            {
                return NotFound($"No season with ID '{id}' found.");
            }

            var viewModel = HomeViewModel.Create(user, season);
            return View(viewModel);
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
        #endregion

        #region API
        [HttpPost]
        public async Task<IActionResult> EndSeason(int id)
        {
            await seasonService.End(id);

            return Ok($"Season {id} has been ended.");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAndStart(int leagueID)
        {
            var league = await context.League
                .Include(l => l.Seasons).ThenInclude(s => s.Sets)
                .SingleOrDefaultAsync(l => l.ID == leagueID);
            if (league == null)
            {
                return BadRequest($"League with ID '{leagueID}' not found.");
            }

            if(league.CurrentSeason != null)
            {
                return BadRequest("There is already a running season.");
            }

            var season = await seasonService.Create(league);
            await StartPost(season.ID);

            if(FeatureToggles.Challonge)
            {
                await seasonService.CreateTournament(season.ID);
            }

            return Created(Url.Action("Home", new {id = season.ID}), JsonConvert.SerializeObject(season));
        }

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
            await seasonService.UpdateStandings(season.ID);

            return Ok(season);
        }
        
        [HttpPost]
        public async Task<IActionResult> Leave(int id, int leagueUserID)
        {
            var season = await context.Season
                .Include(s => s.Participants).ThenInclude(lus => lus.LeagueUser)
                .Include(s => s.Sets)
                .SingleOrDefaultAsync(s => s.ID == id);
            if (season == null)
            {
                return NotFound($"No Season with ID {id} found.");
            }

            var participant = season.Participants.FirstOrDefault(lus => lus.LeagueUserID == leagueUserID);
            if (participant == null)
            {
                return NotFound($"No Participant with League User ID {leagueUserID} found.");
            }

            await seasonService.Leave(participant);
            await seasonService.UpdateStandings(season.ID);

            return Ok(participant);
        }
        
        [HttpPost]
        public async Task<IActionResult> Join(int id, int leagueUserID)
        {
            var season = await context.Season
                .IgnoreQueryFilters()
                .Include(s => s.Participants).ThenInclude(lus => lus.LeagueUser)
                .Include(s => s.Sets)
                .SingleOrDefaultAsync(s => s.ID == id);
            if(season == null)
            {
                return NotFound($"No Season with ID {id} found.");
            }

            var opponents = new HashSet<int> {leagueUserID};

            var participant = season.Participants.FirstOrDefault(lus => lus.LeagueUserID == leagueUserID);
            if (participant == null)
            {
                participant = await seasonService.Join(season, leagueUserID);
            }
            else if(participant.HasLeft)
            {
                context.Update(participant);
                participant.HasLeft = false;

                foreach(var set in season.Sets.Where(s => s.IsPlaying(participant.LeagueUserID)))
                {
                    set.IsDeactivated = false;
                    context.Update(set);

                    opponents.Add(set.GetOpponentID(participant.LeagueUserID));
                }
            }

            var newSets = new List<Set>();
            var newSetCount = 0;
            foreach(var opponent in season.Participants.Where(lus => lus.HasLeft == false))
            {
                if(opponents.Contains(opponent.LeagueUserID))
                {
                    continue;
                }

                ++newSetCount;
                var set = new Set
                {
                    LeagueID = season.LeagueID,
                    SeasonID = season.ID,
                    Player1ID = participant.LeagueUserID,
                    Player2ID = opponent.LeagueUserID,
                    DueDate = DateTime.UtcNow.AddDays(7 * newSetCount)
                };
                newSets.Add(set);
            }

            await context.AddRangeAsync(newSets);
            await context.SaveChangesAsync();

            await seasonService.UpdateStandings(season.ID);

            return Ok(participant);
        }
        #endregion
    }
}
