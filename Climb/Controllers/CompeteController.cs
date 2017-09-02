using Climb.Models;
using Climb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Climb.Controllers
{
    public class CompeteController : Controller
    {
        private readonly ClimbContext _context;

        public CompeteController(ClimbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int userID)
        {
            var users = _context.User.Select(u => u).ToList();
            var user = users.Single(u => u.ID == userID);

            var sets = await _context.Set
                .Include(s => s.Season).ThenInclude(s => s.League)
                .Include(s => s.Player1).ThenInclude(u => u.User)
                .Include(s => s.Player2).ThenInclude(u => u.User)
                .Where(s => s.Player1.UserID == userID || s.Player2.UserID == userID).ToListAsync();

            var viewModel = new CompeteIndexViewModel(user, new ReadOnlyCollection<User>(users), new ReadOnlyCollection<Set>(sets));
            return View(viewModel);
        }
    }
}