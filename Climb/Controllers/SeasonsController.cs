﻿using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Climb.Controllers
{
    public partial class SeasonsController : ModelController
    {
        private readonly ClimbContext _context;
        private readonly ISeasonService seasonService;

        public SeasonsController(ClimbContext context, ISeasonService seasonService, IUserService userService, UserManager<ApplicationUser> userManager)
            : base(userService, userManager)
        {
            _context = context;
            this.seasonService = seasonService;
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateForLeague(int leagueID, DateTime? startDate)
        {
            var league = await _context.League
                .Include(l => l.Seasons)
                .SingleOrDefaultAsync(l => l.ID == leagueID);
            if(league == null)
            {
                return NotFound();
            }

            await seasonService.Create(league, startDate);

            return RedirectToAction("Leagues", "Compete");
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
            var season = await _context.Season
                .Include(s => s.Participants).ThenInclude(user => user.LeagueUser).ThenInclude(u => u.User)
                .SingleOrDefaultAsync(s => s.ID == id);
            if(season == null)
            {
                return NotFound();
            }

            var nonparticipants = await _context.LeagueUser
                .Where(u => u.LeagueID == season.LeagueID && season.Participants.All(lus => lus.LeagueUserID != u.ID))
                .Include(leagueUser => leagueUser.User)
                .ToListAsync();

            return View(new JoinList(season, nonparticipants));
        }

        [HttpPost]
        public async Task<IActionResult> Join(int seasonID, int userID)
        {
            var season = await _context.Season.Include(s => s.Participants).SingleOrDefaultAsync(s => s.ID == seasonID);
            if(season == null)
            {
                return NotFound();
            }

            var leagueUser = await _context.LeagueUser.SingleOrDefaultAsync(u => u.ID == userID);
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
            var season = await _context.Season.Include(s => s.Participants).SingleOrDefaultAsync(s => s.ID == seasonID);
            if (season == null)
            {
                return NotFound();
            }

            var leagueUser = await _context.LeagueUser.SingleOrDefaultAsync(u => u.ID == userID && u.LeagueID == season.LeagueID);
            if (leagueUser == null)
            {
                return NotFound();
            }

            season.Participants.RemoveWhere(lus => lus.LeagueUserID == leagueUser.ID);
            _context.Update(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Join), new { id = seasonID });
        }

        public async Task<IActionResult> Start(int id)
        {
            var season = await _context.Season.Include(s => s.Sets)
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
            var season = await _context.Season
                .Include(s => s.Sets)
                .Include(s => s.Participants).ThenInclude(lus => lus.LeagueUser).ThenInclude(u => u.User)
                .SingleOrDefaultAsync(s => s.ID == id);
            if (season == null)
            {
                return NotFound();
            }

            await seasonService.Start(season);
            
            return RedirectToAction(nameof(Start), new {id});
        }

        public async Task<IActionResult> GetStatus(int id)
        {
            var season = await _context.Season
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
