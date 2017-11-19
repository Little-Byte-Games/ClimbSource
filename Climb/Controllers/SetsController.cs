using Climb.Core;
using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Set = Climb.Models.Set;

namespace Climb.Controllers
{
    public class SetsController : Controller
    {
        public readonly IConfiguration configuration;
        private readonly ClimbContext _context;
        private readonly ISeasonService seasonService;

        public SetsController(ClimbContext context, IConfiguration configuration, ISeasonService seasonService)
        {
            _context = context;
            this.configuration = configuration;
            this.seasonService = seasonService;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var set = await _context.Set
                .Include(s => s.Player1).ThenInclude(p => p.User)
                .Include(s => s.Player2).ThenInclude(p => p.User)
                .Include(s => s.Matches)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (set == null)
            {
                return NotFound();
            }

            return View(set);
        }

        [HttpPost]
        public async Task<IActionResult> AddMatch(int setID)
        {
            var set = await _context.Set.Include(s => s.Matches).SingleAsync(s => s.ID == setID);
            var match = new Match
            {
                Index = set.Matches.Count
            };

            await _context.AddAsync(match);
            set.Matches.Add(match);
            _context.Update(set);
            await _context.SaveChangesAsync();

            return RedirectToAction("Fight", "Compete", new {id = setID });
        }

        [HttpPost]
        public async Task<IActionResult> Submit(int setID, IList<Match> matches)
        {
            var set = await _context.Set.Include(s => s.Matches)
                .Include(s => s.Season).ThenInclude(s => s.Participants)
                .Include(s => s.Player1).ThenInclude(p => p.User)
                .Include(s => s.Player2).ThenInclude(p => p.User)
                .SingleOrDefaultAsync(s => s.ID == setID);
            if(set == null)
            {
                return NotFound($"Could not find set with ID '{setID}'.");
            }

            set.Player1Score = 0;
            set.Player2Score = 0;

            set.UpdatedDate = DateTime.Now;
            foreach(var match in matches)
            {
                if(match.Player1Score > 0 && match.Player1Score > match.Player2Score)
                {
                    ++set.Player1Score;
                }
                else if(match.Player2Score > 0 && match.Player2Score > match.Player1Score)
                {
                    ++set.Player2Score;
                }

                var oldMatch = set.Matches.SingleOrDefault(m => match.ID == m.ID);
                if(oldMatch == null)
                {
                    set.Matches.Add(match);
                    _context.Update(match);
                }
                else
                {
                    _context.Entry(oldMatch).CurrentValues.SetValues(match);
                }
            }

            // TODO: Inflate k-factor


            const int startingElo = 2000;
            set.Player1.Elo = set.Player1.Elo == 0 ? startingElo : set.Player1.Elo;
            set.Player2.Elo = set.Player2.Elo == 0 ? startingElo : set.Player2.Elo;

            int p1Wins = matches.Count(m => m.Player1Score > m.Player2Score);
            int p2Wins = matches.Count(m => m.Player2Score > m.Player1Score);
            var player1Won = p1Wins >= p2Wins;
            var newElo = PlayerScoreCalculator.CalculateElo(set.Player1.Elo, set.Player2.Elo, player1Won);
            set.Player1.Elo = newElo.Item1;
            set.Player2.Elo = newElo.Item2;

            _context.Update(set);

            var users = await _context.LeagueUser.Where(lu => lu.LeagueID == set.LeagueID).ToListAsync();
            users.Sort();
            var rank = 0;
            var lastElo = -1;
            for(var i = 0; i < users.Count; i++)
            {
                LeagueUser member = users[i];
                if(member.Elo != lastElo)
                {
                    lastElo = member.Elo;
                    rank = i + 1;
                }
                member.Rank = rank;
            }
            _context.UpdateRange(users);

            await _context.SaveChangesAsync();

            if(!set.IsExhibition)
            {
                await seasonService.UpdateStandings(set.SeasonID.Value);
            }

            var message = $"{set.Player1.User.Username} ({p1Wins}) v {set.Player2.User.Username} ({p2Wins})";
            var apiKey = configuration.GetSection("Slack")["Key"];
            await SlackController.SendGroupMessage(apiKey, message);

            return RedirectToAction(nameof(UsersController.Home), "Users");
        }
    }

    public class SetMatch
    {
        public Set set;
        public Match match;

        public SetMatch(Set set)
        {
            this.set = set;
            match = new Match();
        }
    }
}
