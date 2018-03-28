using Climb.Consts;
using Climb.Core;
using Climb.Models;
using Climb.Requests.Sets;
using Climb.Responses.Sets;
using Climb.Services;
using Climb.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
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
        private readonly CdnService cdn;

        public SetsController(ClimbContext context, IConfiguration configuration, ISeasonService seasonService, IUserService userService, UserManager<ApplicationUser> userManager, ISetService setService, CdnService cdn)
            : base(userService, userManager)
        {
            this.context = context;
            this.configuration = configuration;
            this.seasonService = seasonService;
            this.setService = setService;
            this.cdn = cdn;
        }

        #region Pages
        public async Task<IActionResult> Fight(int id, string returnUrl = null)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return RedirectToAction("Login", "Account", new {returnUrl = Url.Action("Fight", new {id})});
            }

            var set = await context.Set
                .Include(s => s.Matches).ThenInclude(m => m.MatchCharacters)
                .Include(s => s.Season)
                .Include(s => s.League).ThenInclude(l => l.Game).ThenInclude(g => g.Characters)
                .Include(s => s.League).ThenInclude(l => l.Sets).ThenInclude(s => s.Matches).ThenInclude(m => m.MatchCharacters)
                .Include(s => s.League).ThenInclude(l => l.Game).ThenInclude(g => g.Stages)
                .Include(s => s.Player1).ThenInclude(lu => lu.User)
                .Include(s => s.Player2).ThenInclude(lu => lu.User)
                .SingleOrDefaultAsync(s => s.ID == id);
            if(set == null)
            {
                return NotFound($"Could not find Set with ID '{id}'.");
            }

            if(!string.IsNullOrWhiteSpace(returnUrl))
            {
                ViewData["ReturnUrl"] = returnUrl;
            }

            var viewModel = new GenericViewModel<Set>(user, set);
            return View(viewModel);
        }
        #endregion

        #region API
        [Route("[controller]/{id:int}")]
        public async Task<IActionResult> GetSet(int id)
        {
            var set = await context.Set
                .Include(s => s.Player1).AsNoTracking()
                .Include(s => s.Player2).AsNoTracking()
                .Include(s => s.Matches).ThenInclude(m => m.MatchCharacters).AsNoTracking()
                .Include(s => s.League).ThenInclude(l => l.Game).ThenInclude(g => g.Characters).AsNoTracking()
                .Include(s => s.League).ThenInclude(l => l.Game).ThenInclude(g => g.Stages).AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == id);

            if(set == null)
            {
                return BadRequest($"Not set with id '{id}' found.");
            }

            var response = new SetGetResponse(set, cdn);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStandings(int id)
        {
            await seasonService.UpdateStandings(id);
            return Ok("Standings updated.");
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int id, List<MatchPutRequest> matches)
        {
            var set = await context.Set
                .Include(s => s.Matches).ThenInclude(m => m.MatchCharacters)
                .Include(s => s.League)
                .Include(s => s.Season).ThenInclude(s => s.Participants)
                .Include(s => s.Player1).ThenInclude(p => p.User)
                .Include(s => s.Player2).ThenInclude(p => p.User)
                .SingleOrDefaultAsync(s => s.ID == id);
            if(set == null)
            {
                return NotFound($"Could not find set with ID '{id}'.");
            }

            if(set.IsLocked)
            {
                return BadRequest($"Set {id} is locked.");
            }

            try
            {
                await setService.Put(set, matches);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
                return BadRequest($"Set {id} could not be submitted.");
            }

            if(FeatureToggles.Slack)
            {
                await SendSetCompletedMessage(set);
            }

            return Ok(JsonConvert.SerializeObject(set));
        }

        [HttpPost]
        public async Task<IActionResult> Exhibition(int challengerID, int challengedID)
        {
            var challenged = await context.LeagueUser.SingleOrDefaultAsync(lu => lu.ID == challengedID);
            if(challenged == null)
            {
                return BadRequest($"No challenged league user with id '{challengedID} found.");
            }

            var challenger = await context.LeagueUser.SingleOrDefaultAsync(lu => lu.ID == challengerID);
            if(challenger == null)
            {
                return BadRequest($"No challenger league user with id '{challengerID} found.");
            }

            var set = new Set
            {
                Player1ID = challengerID,
                Player2ID = challengedID,
                DueDate = DateTime.Now.Date,
                LeagueID = challenger.LeagueID,
            };
            await context.Set.AddAsync(set);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(Fight), new {id = set.ID});
        }
        #endregion

        private async Task SendSetCompletedMessage(Set set)
        {
            var message = new StringBuilder();
            message.Append(set.League.Name + " | ");
            message.Append(set.IsExhibition ? "Exhibition" : set.Season.DisplayName);
            message.Append($"\n{set.Player1.GetSlackName} [{set.Player1Score} - {set.Player2Score}] {set.Player2.GetSlackName}");
            message.Append($"\n{setService.GetSetUrl(set, Url)}");
            var apiKey = configuration.GetSection("Slack")["Key"];
            await SlackController.SendGroupMessage(apiKey, message.ToString());
        }
    }
}