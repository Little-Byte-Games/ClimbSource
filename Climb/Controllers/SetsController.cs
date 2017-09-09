using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Climb.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Climb.Models;
using Newtonsoft.Json;
using Set = Climb.Models.Set;

namespace Climb.Controllers
{
    public class SetsController : Controller
    {
        private readonly ClimbContext _context;

        public SetsController(ClimbContext context)
        {
            _context = context;
        }

        // GET: Sets
        public async Task<IActionResult> Index()
        {
            var climbContext = _context.Set.Include(s => s.Player1).Include(s => s.Player2);
            return View(await climbContext.ToListAsync());
        }

        // GET: Sets/Details/5
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

        // GET: Sets/Create
        public IActionResult Create()
        {
            ViewData["Player1ID"] = new SelectList(_context.User, "ID", "ID");
            ViewData["Player2ID"] = new SelectList(_context.User, "ID", "ID");
            return View();
        }

        // POST: Sets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Player1ID,Player2ID,UpdatedDate")] Set set)
        {
            if (ModelState.IsValid)
            {
                _context.Add(set);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddMatches), set);
            }
            ViewData["Player1ID"] = new SelectList(_context.User, "ID", "ID", set.Player1ID);
            ViewData["Player2ID"] = new SelectList(_context.User, "ID", "ID", set.Player2ID);
            return View(set);
        }

        // GET: Sets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var set = await _context.Set.SingleOrDefaultAsync(m => m.ID == id);
            if (set == null)
            {
                return NotFound();
            }
            ViewData["Player1ID"] = new SelectList(_context.User, "ID", "ID", set.Player1ID);
            ViewData["Player2ID"] = new SelectList(_context.User, "ID", "ID", set.Player2ID);
            return View(set);
        }

        // POST: Sets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Player1ID,Player2ID,UpdatedDate")] Set set)
        {
            if (id != set.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(set);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SetExists(set.ID))
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
            ViewData["Player1ID"] = new SelectList(_context.User, "ID", "ID", set.Player1ID);
            ViewData["Player2ID"] = new SelectList(_context.User, "ID", "ID", set.Player2ID);
            return View(set);
        }

        // GET: Sets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var set = await _context.Set
                .Include(s => s.Player1)
                .Include(s => s.Player2)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (set == null)
            {
                return NotFound();
            }

            return View(set);
        }

        // POST: Sets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var set = await _context.Set.SingleOrDefaultAsync(m => m.ID == id);
            _context.Set.Remove(set);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SetExists(int id)
        {
            return _context.Set.Any(e => e.ID == id);
        }

        public async Task<IActionResult> AddMatches(int id)
        {
            var set = await _context.Set.Include(s => s.Player1).ThenInclude(u => u.User).Include(s => s.Player2).ThenInclude(u => u.User).SingleOrDefaultAsync(m => m.ID == id);
            return View("../Matches/Create", new SetMatch(set));
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
                .Include(s => s.Season)
                .Include(s => s.Player1).ThenInclude(p => p.User)
                .Include(s => s.Player2).ThenInclude(p => p.User)
                .SingleOrDefaultAsync(s => s.ID == setID);

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

            var users = await _context.LeagueUser.Where(lu => lu.LeagueID == set.Season.LeagueID).ToListAsync();
            users.Sort();
            var rank = 0;
            var lastElo = -1;
            foreach(LeagueUser member in users)
            {
                if(member.Elo != lastElo)
                {
                    lastElo = member.Elo;
                    ++rank;
                }
                member.Rank = rank;
            }
            _context.UpdateRange(users);

            await _context.SaveChangesAsync();

            var client = new HttpClient();
            const string requestUri = @"https://hooks.slack.com/services/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";
            var message = new { text = $"{set.Player1.User.Username} ({p1Wins}) v {set.Player2.User.Username} ({p2Wins})" };
            await client.PostAsync(requestUri, new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"));

            return RedirectToAction(nameof(Index));
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
