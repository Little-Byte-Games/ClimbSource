using Climb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Climb.Controllers
{
    public class RankEventsController : Controller
    {
        private readonly ClimbContext _context;

        public RankEventsController(ClimbContext context)
        {
            _context = context;
        }

        // GET: RankEvents
        public async Task<IActionResult> Index()
        {
            var climbContext = _context.RankEvent.Include(r => r.League).Include(r => r.Set);
            return View(await climbContext.ToListAsync());
        }

        // GET: RankEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var rankEvent = await _context.RankEvent.Include(r => r.League).Include(r => r.Set).SingleOrDefaultAsync(m => m.ID == id);
            if(rankEvent == null)
            {
                return NotFound();
            }

            return View(rankEvent);
        }

        // GET: RankEvents/Create
        public IActionResult Create()
        {
            ViewData["LeagueID"] = new SelectList(_context.League, "ID", "ID");
            ViewData["SetID"] = new SelectList(_context.Set, "ID", "ID");
            return View();
        }

        // POST: RankEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,SetID,LeagueID,Elo,Rank")] RankEvent rankEvent)
        {
            if(ModelState.IsValid)
            {
                _context.Add(rankEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeagueID"] = new SelectList(_context.League, "ID", "ID", rankEvent.LeagueID);
            ViewData["SetID"] = new SelectList(_context.Set, "ID", "ID", rankEvent.SetID);
            return View(rankEvent);
        }

        // GET: RankEvents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var rankEvent = await _context.RankEvent.SingleOrDefaultAsync(m => m.ID == id);
            if(rankEvent == null)
            {
                return NotFound();
            }
            ViewData["LeagueID"] = new SelectList(_context.League, "ID", "ID", rankEvent.LeagueID);
            ViewData["SetID"] = new SelectList(_context.Set, "ID", "ID", rankEvent.SetID);
            return View(rankEvent);
        }

        // POST: RankEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,SetID,LeagueID,Elo,Rank")] RankEvent rankEvent)
        {
            if(id != rankEvent.ID)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    _context.Update(rankEvent);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(!RankEventExists(rankEvent.ID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeagueID"] = new SelectList(_context.League, "ID", "ID", rankEvent.LeagueID);
            ViewData["SetID"] = new SelectList(_context.Set, "ID", "ID", rankEvent.SetID);
            return View(rankEvent);
        }

        // GET: RankEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var rankEvent = await _context.RankEvent.Include(r => r.League).Include(r => r.Set).SingleOrDefaultAsync(m => m.ID == id);
            if(rankEvent == null)
            {
                return NotFound();
            }

            return View(rankEvent);
        }

        // POST: RankEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rankEvent = await _context.RankEvent.SingleOrDefaultAsync(m => m.ID == id);
            _context.RankEvent.Remove(rankEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RankEventExists(int id)
        {
            return _context.RankEvent.Any(e => e.ID == id);
        }
    }
}