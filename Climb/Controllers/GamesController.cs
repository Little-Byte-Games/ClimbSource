using Climb.Models;
using Climb.Services;
using Climb.ViewModels;
using Climb.ViewModels.Games;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Extensions;

namespace Climb.Controllers
{
    public class GamesController : ModelController
    {
        private readonly ClimbContext context;
        private readonly CdnService cdnService;

        public GamesController(ClimbContext context, CdnService cdnService, UserManager<ApplicationUser> userManager, IUserService userService)
            : base(userService, userManager)
        {
            this.context = context;
            this.cdnService = cdnService;
        }

        #region Pages
        public async Task<IActionResult> Index()
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index")});
            }

            var games = await context.Game.ToArrayAsync();
            var viewModel = new GenericViewModel<IEnumerable<Game>>(user, games);
            return View(viewModel);
        }

        [HttpGet("[controller]/Home/{id:int}")]
        public async Task<IActionResult> HomeID(int id)
        {
            var game = await context.Game.SingleOrDefaultAsync(g => g.ID == id);
            if(game != null)
            {
                return RedirectToActionPermanent("Home", new {url = game.Url});
            }

            var user = await GetViewUserAsync();
            return this.NotFoundPage(user, $"Could not find Game with ID '{id}'.");
        }

        [HttpGet("[controller]/Home/{url}")]
        public async Task<IActionResult> Home(string url)
        {
            var user = await GetViewUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Home", new {url})});
            }

            var game = await context.Game
                .Include(g => g.Characters)
                .Include(g => g.Stages)
                .SingleOrDefaultAsync(m => m.Url == url);
            if (game == null)
            {
                return this.NotFoundPage(user, $"Could not find Game with URL '{url}'.");
            }

            var viewModel = new HomeViewModel(user, game);
            return View(viewModel);
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> AddCharacter(int id, string characterName, IFormFile file)
        {
            if(string.IsNullOrWhiteSpace(characterName))
            {
                return BadRequest("Character name has to be a valid string.");
            }

            var alreadyExists = await context.Character.AnyAsync(c => c.GameID == id && c.Name == characterName);
            if (alreadyExists)
            {
                return BadRequest($"Character '{characterName}' already exists.");
            }

            var game = await context.Game.SingleOrDefaultAsync(g => g.ID == id);
            if(game == null)
            {
                return NotFound($"Could not find Game with ID '{id}'.");
            }

            var picKey = await cdnService.UploadImage(CdnService.ImageTypes.CharacterPic, file);

            var character = new Character
            {
                Name = characterName,
                GameID = id,
                PicKey = picKey,
            };
            context.Add(character);
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
                return NotFound($"Could not find Game with ID '{id}'.");
            }

            var stage = new Stage
            {
                Name = stageName,
                GameID = id,
            };
            context.Add(stage);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Home), new { id });
        }
    }
}