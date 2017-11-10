using Climb.Models;
using Climb.Services;
using Climb.ViewModels.Games;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Climb.Controllers
{
    public class GamesController : ModelController
    {
        private readonly ClimbContext context;
        private readonly ICdnService cdnService;

        public GamesController(ClimbContext context, ICdnService cdnService, UserManager<ApplicationUser> userManager, IUserService userService)
            : base(userService, userManager)
        {
            this.context = context;
            this.cdnService = cdnService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await context.Game.ToListAsync());
        }

        public async Task<IActionResult> Home(int id)
        {
            var user = await GetViewUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            var game = await context.Game
                .Include(g => g.Characters)
                .Include(g => g.Stages)
                .SingleOrDefaultAsync(m => m.ID == id);
            if(game == null)
            {
                return NotFound();
            }

            var viewModel = new HomeViewModel(user, game);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddCharacter(int id, string characterName, IFormFile file)
        {
            if(string.IsNullOrWhiteSpace(characterName))
            {
                return BadRequest("Character name has to be a valid string.");
            }

            var alreadyExists = await context.Character.AnyAsync(c => c.Name == characterName);
            if (alreadyExists)
            {
                return BadRequest($"Character '{characterName}' already exists.");
            }

            var game = await context.Game.SingleOrDefaultAsync(g => g.ID == id);
            if(game == null)
            {
                return NotFound();
            }

            var picKey = await cdnService.UploadCharacterPic(file);

            var character = new Character
            {
                Name = characterName,
                GameID = id,
                PicKey = picKey,
            };
            await context.AddAsync(character);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Home), new {id});
        }

        [HttpPost]
        public async Task<IActionResult> AddStage(int id, string stageName)
        {
            if (string.IsNullOrWhiteSpace(stageName))
            {
                return BadRequest("Stage name has to be a valid string.");
            }

            var alreadyExists = await context.Stage.AnyAsync(s => s.Name == stageName);
            if(alreadyExists)
            {
                return BadRequest($"Stage '{stageName}' already exists.");
            }

            var game = await context.Game.SingleOrDefaultAsync(g => g.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            var stage = new Stage
            {
                Name = stageName,
                GameID = id,
            };
            await context.AddAsync(stage);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Home), new { id });
        }
    }
}