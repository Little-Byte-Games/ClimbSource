﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Climb.Models;

namespace Climb.Controllers
{
    public class GamesController : Controller
    {
        private readonly ClimbContext _context;

        public GamesController(ClimbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Game.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .Include(g => g.Characters)
                .Include(g => g.Stages)
                .SingleOrDefaultAsync(m => m.ID == id);
            if(game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] Game game)
        {
            if(ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var game = await _context.Game.SingleOrDefaultAsync(m => m.ID == id);
            if(game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] Game game)
        {
            if(id != game.ID)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if(!GameExists(game.ID))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var game = await _context.Game.SingleOrDefaultAsync(m => m.ID == id);
            if(game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Game.SingleOrDefaultAsync(m => m.ID == id);
            _context.Game.Remove(game);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Game.Any(e => e.ID == id);
        }

        public async Task<IActionResult> Home(int id)
        {
            var game = await _context.Game.SingleOrDefaultAsync(m => m.ID == id);
            if(game == null)
            {
                return NotFound();
            }

            return NotFound();
        }

        public async Task<IActionResult> AddCharacter(int id, string characterName)
        {
            if(string.IsNullOrWhiteSpace(characterName))
            {
                return BadRequest("Character name has to be a valid string.");
            }

            var game = await _context.Game.SingleOrDefaultAsync(g => g.ID == id);
            if(game == null)
            {
                return NotFound();
            }

            var character = new Character
            {
                Name = characterName,
                GameID = id,
            };
            await _context.AddAsync(character);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new {id});
        }

        public async Task<IActionResult> AddStage(int id, string stageName)
        {
            if (string.IsNullOrWhiteSpace(stageName))
            {
                return BadRequest("Stage name has to be a valid string.");
            }

            var alreadyExists = await _context.Stage.AnyAsync(s => s.Name == stageName);
            if(alreadyExists)
            {
                return BadRequest($"Stage '{stageName}' already exists.");
            }

            var game = await _context.Game.SingleOrDefaultAsync(g => g.ID == id);
            if (game == null)
            {
                return NotFound();
            }

            var stage = new Stage
            {
                Name = stageName,
                GameID = id,
            };
            await _context.AddAsync(stage);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}