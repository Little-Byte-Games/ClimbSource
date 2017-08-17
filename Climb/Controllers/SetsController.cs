using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Climb.Models;

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
                .Include(s => s.Player1)
                .Include(s => s.Player2)
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

        public IActionResult AddMatches(Set set)
        {
            //var set = await _context.Set.SingleOrDefaultAsync(m => m.ID == id);
            return View("../Matches/Create", new SetMatch(set));
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
