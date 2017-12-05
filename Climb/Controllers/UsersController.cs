﻿using Climb.Models;
using Climb.Services;
using Climb.ViewModels;
using Climb.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UserApp.Controllers;

namespace Climb.Controllers
{
    public class UsersController : ModelController
    {
        private readonly ClimbContext context;

        public UsersController(ClimbContext context, IUserService userService, UserManager<ApplicationUser> userManager)
            : base(userService, userManager)
        {
            this.context = context;
        }

        #region Pages
        [Authorize]
        public async Task<IActionResult> Home(int? id)
        {
            var user = await GetViewUserAsync();
            if(user == null)
            {
                return RedirectToAction(nameof(AccountController.Login), "Account");
            }

            if(id == null)
            {
                id = user.ID;
            }

            var homeUser = context.User
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.RankSnapshots)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Game)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player1).ThenInclude(lu => lu.User)
                .Include(u => u.LeagueUsers).ThenInclude(lu => lu.League).ThenInclude(l => l.Seasons).ThenInclude(s => s.Sets).ThenInclude(s => s.Player2).ThenInclude(lu => lu.User)
                .SingleOrDefault(u => u.ID == id);
            if(homeUser == null)
            {
                return NotFound($"Could not find User with ID '{id}'.");
            }

            var viewModel = CompeteHomeViewModel.Create(user, homeUser);
            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> Account()
        {
            var user = await GetViewUserAsync();
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserAccountViewModel(user);
            return View(viewModel);
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("ID,Username")] User user)
        {
            if (id != user.ID)
            {
                return NotFound($"ID's do not match {id} vs {user.ID}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(user);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    var userExists = await userService.DoesUserExist(user.ID);
                    if (!userExists)
                    {
                        return NotFound();
                    }

                    throw;
                }
                return RedirectToAction(nameof(Account));
            }

            var viewUser = await GetViewUserAsync();
            var viewModel = new UserAccountViewModel(viewUser);
            return View(nameof(Account), viewModel);
        }
    }
}
