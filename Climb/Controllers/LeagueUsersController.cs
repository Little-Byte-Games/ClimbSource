using Climb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Climb.Controllers
{
    public class LeagueUsersController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ClimbContext _context;

        public LeagueUsersController(IConfiguration configuration, ClimbContext context)
        {
            this.configuration = configuration;
            _context = context;
        }

        // GET: LeagueUsers
        public async Task<IActionResult> Index()
        {
            var climbContext = _context.LeagueUser.Include(l => l.League).Include(l => l.User);
            return View(await climbContext.ToListAsync());
        }

        // GET: LeagueUsers/Details/5
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

            var profilePicUrl = @"https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcQpAB85y5CpzuT3QLLh7dVkrSIWRwQ8gANIH2OHeqph6k2Caa2UFnHgKbwu";
            if(leagueUser.ProfilePicKey != null)
            {
                var awsSection = configuration.GetSection("AWS");
                var root = awsSection["RootUrl"];
                var bucket = awsSection["Bucket"];
                profilePicUrl = string.Join("/", root, bucket, leagueUser.ProfilePicKey);
            }
            ViewBag.ProfilePic = profilePicUrl;

            return View(leagueUser);
        }

        // GET: LeagueUsers/Create
        public IActionResult Create()
        {
            ViewData["LeagueID"] = new SelectList(_context.League, "ID", "ID");
            ViewData["UserID"] = new SelectList(_context.User, "ID", "ID");
            return View();
        }

        // POST: LeagueUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: LeagueUsers/Edit/5
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

        // POST: LeagueUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: LeagueUsers/Delete/5
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

        // POST: LeagueUsers/Delete/5
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
    }
}