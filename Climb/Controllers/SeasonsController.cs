﻿using Climb.Consts;
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

            return Ok(season);
        }
        
        [HttpPost]
        public async Task<IActionResult> Leave(int id, int leagueUserID)
        {
            var season = await context.Season
                .Include(s => s.Participants)
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

            return Ok(participant);
        }
        
        [HttpPost]
        public async Task<IActionResult> Join(int id, int leagueUserID)
        {
            var season = await context.Season
                .Include(s => s.Participants)
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(s => s.ID == id);
            if(season == null)
            {
                return NotFound($"No Season with ID {id} found.");
            }

            var participant = season.Participants.FirstOrDefault(lus => lus.LeagueUserID == leagueUserID);
            if (participant == null)
            {
                participant = await seasonService.Join(season, leagueUserID);
            }
            else if(participant.HasLeft)
            {
                context.Update(participant);
                participant.HasLeft = false;
                await context.SaveChangesAsync();
            }

            // TODO: create missing sets


            return Ok(participant);
        }
        #endregion

        [Obsolete]
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
    }
}
