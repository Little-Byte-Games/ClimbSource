using System;
using System.Collections.Generic;
using Climb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Climb.Core;
using Climb.ViewModels;
using Microsoft.Extensions.Configuration;
using MoreLinq;

namespace Climb.Controllers
{
    public class LeaguesController : Controller
    {
        private readonly ClimbContext _context;
        private readonly IConfiguration configuration;

        public LeaguesController(ClimbContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var climbContext = _context.League.Include(l => l.Game).Include(l => l.Admin);
            return View(await climbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _context.League
                .Include(l => l.Game)
                .Include(l => l.Admin)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (league == null)
            {
                return NotFound();
            }

            return View(league);
        }

        public IActionResult Create()
        {
            ViewData[nameof(League.GameID)] = new SelectList(_context.Game, nameof(Game.ID), nameof(Game.Name));
            ViewData[nameof(League.AdminID)] = new SelectList(_context.User, nameof(Models.User.ID), nameof(Models.User.Username));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,GameID,AdminID")] League league)
        {
            if (ModelState.IsValid)
            {
                _context.Add(league);
                await _context.SaveChangesAsync();

                var admin = new LeagueUser { UserID = league.AdminID, League = league };
                _context.Add(admin);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["GameID"] = new SelectList(_context.Game, "ID", "ID", league.GameID);
            ViewData["UserID"] = new SelectList(_context.User, "ID", "ID", league.AdminID);
            return View(league);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _context.League.SingleOrDefaultAsync(m => m.ID == id);
            if (league == null)
            {
                return NotFound();
            }
            ViewData["GameID"] = new SelectList(_context.Game, "ID", "ID", league.GameID);
            ViewData["UserID"] = new SelectList(_context.User, "ID", "ID", league.AdminID);
            return View(league);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,GameID,UserID")] League league)
        {
            if (id != league.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(league);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeagueExists(league.ID))
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
            ViewData["GameID"] = new SelectList(_context.Game, "ID", "ID", league.GameID);
            ViewData["UserID"] = new SelectList(_context.User, "ID", "ID", league.AdminID);
            return View(league);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _context.League
                .Include(l => l.Game)
                .Include(l => l.Admin)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (league == null)
            {
                return NotFound();
            }

            return View(league);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var league = await _context.League.SingleOrDefaultAsync(m => m.ID == id);
            _context.League.Remove(league);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeagueExists(int id)
        {
            return _context.League.Any(e => e.ID == id);
        }

        public async Task<IActionResult> Join(int id)
        {
            var league = await _context.League.SingleOrDefaultAsync(l => l.ID == id);
            var leagueUsers = await _context.LeagueUser.Where(u => u.LeagueID == id && !u.HasLeft).Include(l => l.User).ToListAsync();
            var users = await _context.User.Where(u => leagueUsers.All(lu => lu.UserID != u.ID)).ToListAsync();

            return View(new LeagueJoinViewModel(league, users, leagueUsers));
        }

        [HttpPost]
        public async Task<IActionResult> Join(int leagueID, int userID)
        {
            var league = await _context.League.SingleOrDefaultAsync(l => l.ID == leagueID);
            if(league == null)
            {
                return NotFound();
            }

            var user = await _context.User.SingleOrDefaultAsync(u => u.ID == userID);
            if(user == null)
            {
                return NotFound();
            }

            var leagueUser = await _context.LeagueUser.SingleOrDefaultAsync(u => u.LeagueID == leagueID && u.UserID == userID);
            if(leagueUser != null)
            {
                leagueUser.HasLeft = false;
                _context.Update(leagueUser);
            }
            else
            {
                leagueUser = new LeagueUser
                {
                    Elo = 2000,
                    League = league,
                    User = user
                };
                await _context.AddAsync(leagueUser);
            }

            
            await _context.SaveChangesAsync();

            return RedirectToAction("Leagues", "Compete");
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int leagueID, int userID)
        {
            var leagueUser = await _context.LeagueUser.SingleOrDefaultAsync(u => u.LeagueID == leagueID && u.UserID == userID);
            if(leagueUser != null)
            {
                leagueUser.HasLeft = true;
                _context.Update(leagueUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Leagues", "Compete");
        }

        public async Task<IActionResult> Home(int id, int? seasonID)
        {
            var league = await _context.League
                .Include(l => l.Members).ThenInclude(lu => lu.User)
                .Include(l => l.Members).ThenInclude(lu => lu.RankSnapshots)
                .Include(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player1)
                .Include(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player2)
                .SingleOrDefaultAsync(l => l.ID == id);
            if(league == null)
            {
                return NotFound();
            }

            var currentSeason = league.CurrentSeason;
            var completedSets = currentSeason?.Sets.Where(s => s.IsComplete && !s.IsBye);

            if(seasonID == null)
            {
                if (currentSeason != null)
                {
                    seasonID = currentSeason.ID; 
                }
                else if (league.Seasons?.Count > 0)
                {
                    seasonID = league.Seasons.Last().ID;
                }
            }

            var viewModel = new LeagueHomeViewModel(league, configuration, completedSets, seasonID);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TakeRankSnapshot(int id)
        {
            var league = await _context.League
                .Include(l => l.Members).ThenInclude(lu => lu.RankSnapshots)
                .Include(l => l.Members).ThenInclude(lu => lu.User)
                .SingleOrDefaultAsync(l => l.ID == id);
            if(league == null)
            {
                return NotFound();
            }

            var createdDate = DateTime.Now;
            var rankSnapshots = new HashSet<RankSnapshot>();
            foreach(var member in league.Members)
            {
                RankSnapshot lastSnapshot = null;
                if(member.RankSnapshots?.Count > 0)
                {
                    lastSnapshot = member.RankSnapshots?.MaxBy(rs => rs.CreatedDate);
                }
                var rankDelta = member.Rank - (lastSnapshot?.Rank ?? 0);
                var eloDelta = member.Elo - (lastSnapshot?.Elo ?? 0);
                var rankSnapshot = new RankSnapshot {LeagueUser = member, Rank = member.Rank, DeltaRank = rankDelta, Elo = member.Elo, DeltaElo = eloDelta, CreatedDate = createdDate };
                rankSnapshots.Add(rankSnapshot);
            }
            await _context.RankSnapshot.AddRangeAsync(rankSnapshots);
            await _context.SaveChangesAsync();

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

        public async Task<IActionResult> BestCharacter(int id)
        {
            var league = await _context.League.SingleOrDefaultAsync(l => l.ID == id);
            if(league == null)
            {
                return NotFound();
            }

            return View();
        }
    }
}
