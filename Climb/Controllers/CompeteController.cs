﻿using System;
using System.Collections.Generic;
using Climb.Models;
using Climb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Climb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Schedule(int? leagueID, int? seasonID)
        {
            var appUser = await userManager.GetUserAsync(User);
            var userID = appUser.UserID;

            var user = await _context.User
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League)
                .SingleOrDefaultAsync(u => u.ID == userID);
            if(user == null)
            {
                return NotFound();
            }

            var leagues = user.LeagueUsers.Select(ul => ul.League).ToList();
            var leagueUser = user.LeagueUsers.SingleOrDefault(lu => lu.LeagueID == leagueID) ?? user.LeagueUsers.FirstOrDefault();

            var selectedLeague = leagues.SingleOrDefault(l => l.ID == leagueID);
            List<Season> selectedSeasons = null;

            if (leagueUser != null)
            {
                var seasons = await _context.Season
                        .Include(s => s.Participants)
                        .Include(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User)
                        .Include(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User)
                        .ToListAsync();
                selectedSeasons = seasons.Where(season => season.Participants != null && season.Participants.Any(lus => lus.LeagueUserID == leagueUser.ID)).ToList(); 
            }

            var selectedSeason = selectedSeasons?.SingleOrDefault(s => s.ID == seasonID) ?? selectedSeasons?.FirstOrDefault();

            var viewModel = new CompeteScheduleViewModel(selectedLeague, selectedSeason, leagues, selectedSeasons, leagueUser);
            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> Leagues()
        {
            var appUser = await userManager.GetUserAsync(User);
            var user = await _context.User.Include(u => u.LeagueUsers).SingleOrDefaultAsync(u => u.ID == appUser.UserID);
            var leagues = await _context.League
                .Include(l => l.Members)
                .Include(l => l.Admin)
                .Include(l => l.Game)
                .ToListAsync();

            var viewModel = CompeteLeaguesViewModel.Create(appUser.UserID, user.LeagueUsers, leagues);
            return View(viewModel);
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