﻿using System;
using System.Collections.Generic;
using Climb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Climb.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Set = Climb.Models.Set;

namespace Climb.Controllers
{
    public partial class SeasonsController : Controller
    {
        private readonly ClimbContext _context;

        public SeasonsController(ClimbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Season.Include(s => s.Participants).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _context.Season.Include(s => s.Participants).SingleOrDefaultAsync(m => m.ID == id);
            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }

        public IActionResult Create()
        {
            ViewData["LeagueID"] = new SelectList(_context.League, "ID", "ID");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID", "LeagueID, StartDate")] Season season)
        {
            if (ModelState.IsValid)
            {
                var count = await _context.Season.Where(s => s.LeagueID == season.LeagueID).CountAsync();
                season.Index = count;
                _context.Add(season);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(season);
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

            var season = new Season { Index = league.Seasons.Count, LeagueID = leagueID, StartDate = startDate ?? DateTime.UtcNow.AddDays(7) };
            await _context.AddAsync(season);
            await _context.SaveChangesAsync();

            return RedirectToAction("Leagues", "Compete");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _context.Season.SingleOrDefaultAsync(m => m.ID == id);
            if (season == null)
            {
                return NotFound();
            }
            return View(season);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID")] Season season)
        {
            if (id != season.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(season);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeasonExists(season.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(season);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var season = await _context.Season
                .SingleOrDefaultAsync(m => m.ID == id);
            if (season == null)
            {
                return NotFound();
            }

            return View(season);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var season = await _context.Season.SingleOrDefaultAsync(m => m.ID == id);
            _context.Season.Remove(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeasonExists(int id)
        {
            return _context.Season.Any(e => e.ID == id);
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

            var leagueUserSeason = new LeagueUserSeason { Season = season, LeagueUser = leagueUser };

            season.Participants.Add(leagueUserSeason);
            _context.Update(season);
            await _context.SaveChangesAsync();
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

            if (season.Sets != null)
            {
                _context.RemoveRange(season.Sets); 
            }
            season.Sets = new HashSet<Set>();

            var participants = season.Participants.Select(lus => lus.LeagueUser.ID).ToList();
            var rounds = ScheduleGenerator.Generate(10, participants, season.StartDate);
            foreach(var round in rounds)
            {
                foreach(var setData in round.sets)
                {
                    var byePlayer = 0;

                    int? player1 = setData.player1;
                    if(player1 == ScheduleGenerator.Bye)
                    {
                        player1 = null;
                        byePlayer = 1;
                    }
                    int? player2 = setData.player2;
                    if (player2 == ScheduleGenerator.Bye)
                    {
                        player2 = null;
                        byePlayer = 2;
                    }

                    var set = new Set
                    {
                        DueDate = round.dueDate,
                        Player1ID = player1,
                        Player2ID = player2,
                        Player1Score = byePlayer == 1 ? - 1 : 0,
                        Player2Score = byePlayer == 2 ? - 1 : 0,
                    };

                    season.Sets.Add(set);
                }
            }

            _context.UpdateRange(season.Sets);
            _context.Update(season);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Start), new {id});
        }
    }
}
