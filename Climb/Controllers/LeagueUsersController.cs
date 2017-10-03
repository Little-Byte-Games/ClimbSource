using Climb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Climb.Services;

namespace Climb.Controllers
{
    public class LeagueUsersController : Controller
    {
        private readonly ClimbContext _context;
        private readonly ICDNService cdnService;
        private readonly LeagueUserService leagueUserService;

        public LeagueUsersController(ClimbContext context, ICDNService cdnService)
        {
            _context = context;
            this.cdnService = cdnService;
            leagueUserService = new LeagueUserService(context);
        }

        public async Task<IActionResult> Index()
        {
            var climbContext = _context.LeagueUser.Include(l => l.League).Include(l => l.User);
            return View(await climbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var leagueUser = await _context.LeagueUser.Include(l => l.League).Include(l => l.User).SingleOrDefaultAsync(m => m.ID == id);
            if(leagueUser == null)
            {
                return NotFound();
            }

            ViewBag.ProfilePic = cdnService.GetProfilePic(leagueUser);

            return View(leagueUser);
        }

        public IActionResult Create()
        {
            ViewData["LeagueID"] = new SelectList(_context.League, "ID", "ID");
            ViewData["UserID"] = new SelectList(_context.User, "ID", "ID");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserID,LeagueID,Elo")] LeagueUser leagueUser)
        {
            if(ModelState.IsValid)
            {
                _context.Add(leagueUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeagueID"] = new SelectList(_context.League, "ID", "ID", leagueUser.LeagueID);
            ViewData["UserID"] = new SelectList(_context.User, "ID", "ID", leagueUser.UserID);
            return View(leagueUser);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var leagueUser = await _context.LeagueUser.SingleOrDefaultAsync(m => m.ID == id);
            if(leagueUser == null)
            {
                return NotFound();
            }
            ViewData["LeagueID"] = new SelectList(_context.League, "ID", "ID", leagueUser.LeagueID);
            ViewData["UserID"] = new SelectList(_context.User, "ID", "ID", leagueUser.UserID);
            return View(leagueUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserID,LeagueID,Elo")] LeagueUser leagueUser)
        {
            if(id != leagueUser.ID)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    _context.Update(leagueUser);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(!LeagueUserExists(leagueUser.ID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeagueID"] = new SelectList(_context.League, "ID", "ID", leagueUser.LeagueID);
            ViewData["UserID"] = new SelectList(_context.User, "ID", "ID", leagueUser.UserID);
            return View(leagueUser);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var leagueUser = await _context.LeagueUser.Include(l => l.League).Include(l => l.User).SingleOrDefaultAsync(m => m.ID == id);
            if(leagueUser == null)
            {
                return NotFound();
            }

            return View(leagueUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leagueUser = await _context.LeagueUser.SingleOrDefaultAsync(m => m.ID == id);
            _context.LeagueUser.Remove(leagueUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeagueUserExists(int id)
        {
            return _context.LeagueUser.Any(e => e.ID == id);
        }

        public async Task<IActionResult> GetUserTrend(int id)
        {
            var leagueUser = await _context.LeagueUser
                .Include(lu => lu.RankSnapshots)
                .SingleOrDefaultAsync(lu => lu.ID == id);
            if(leagueUser == null)
            {
                return NotFound();
            }

            var rankDifference = leagueUser.GetRankTrendDelta();
            return Ok(new {leagueUserID = id, rankDelta = rankDifference });
        }

        public async Task<string> FavoriteCharacter(int id)
        {
            var character = await leagueUserService.GetFavoriteCharacter(id);
            return character?.Name ?? "None";
        }
    }
}