using Climb.Consts;
using Climb.Core;
using Climb.Models;
using Climb.Services;
using Climb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Set = Climb.Models.Set;

namespace Climb.Controllers
{
    public class SetsController : ModelController
    {
        public const int MaxMatchCount = 5;

        private readonly ClimbContext context;
        private readonly IConfiguration configuration;
        private readonly ISeasonService seasonService;
        private readonly ISetService setService;

        public SetsController(ClimbContext context, IConfiguration configuration, ISeasonService seasonService, IUserService userService, UserManager<ApplicationUser> userManager, ISetService setService)
            : base(userService, userManager)
        {
            this.context = context;
            this.configuration = configuration;
            this.seasonService = seasonService;
            this.setService = setService;
        }

        #region Pages
        [Authorize]
        public async Task<IActionResult> Fight(int id)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Fight", new { id }) });
            }

            var set = await context.Set
                .Include(s => s.Matches)
                .Include(s => s.Season)
                .Include(s => s.League).ThenInclude(l => l.Game).ThenInclude(g => g.Characters)
                .Include(s => s.League).ThenInclude(l => l.Game).ThenInclude(g => g.Stages)
                .Include(s => s.Player1).ThenInclude(lu => lu.User)
                .Include(s => s.Player2).ThenInclude(lu => lu.User)
                .SingleOrDefaultAsync(s => s.ID == id);
            if(set == null)
            {
                return NotFound($"Could not find Set with ID '{id}'.");
            }

            var viewModel = new GenericViewModel<Set>(user, set);
            return View(viewModel);
        }
        #endregion

        #region API
        [HttpPost]
        public async Task<IActionResult> UpdateStandings(int id)
        {
            await seasonService.UpdateStandings(id);
            return Ok("Standings updated.");
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int id, List<Match> matches)
        {
            var set = await context.Set
                .Include(s => s.Matches).ThenInclude(m => m.MatchCharacters)
                .Include(s => s.Season).ThenInclude(s => s.Participants)
                .Include(s => s.Player1).ThenInclude(p => p.User)
                .Include(s => s.Player2).ThenInclude(p => p.User)
                .SingleOrDefaultAsync(s => s.ID == id);
            if (set == null)
            {
                return NotFound($"Could not find set with ID '{id}'.");
            }

            if (set.IsLocked)
            {
                return BadRequest($"Set {id} is locked.");
            }

            matches.RemoveAll(m => m.MatchCharacters.Count == 0 || m.StageID < 0);

            try
            {
                await setService.Put(set, matches);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return BadRequest($"Set {id} could not be submitted.");
            }

            if (FeatureToggles.Slack)
            {
                await SendSetCompletedMessage(set); 
            }

            return Ok(JsonConvert.SerializeObject(set));
        }
        #endregion

        private async Task SendSetCompletedMessage(Set set)
        {
            var message = $"{set.Player1.GetSlackName} [{set.Player1Score} - {set.Player2Score}] {set.Player2.GetSlackName}";
            message += $"\n{Url.Action(new UrlActionContext {Action = nameof(Fight), Values = new { id = set.ID }, Protocol = "https"})}";
            var apiKey = configuration.GetSection("Slack")["Key"];
            await SlackController.SendGroupMessage(apiKey, message);
        }

        [HttpPost]
        public async Task<IActionResult> Exhibition(int challengerID, int challengedID)
        {
            var challenged = await context.LeagueUser.SingleOrDefaultAsync(lu => lu.ID == challengedID);
            if(challenged == null)
            {
                return NotFound($"No challenged league user with id '{challengedID} found.");
            }

            var challenger = await context.User
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
            await context.Set.AddAsync(set);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Fight), new {id = set.ID});
        }
    }
}