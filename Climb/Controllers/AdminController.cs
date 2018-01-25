using Climb.Consts;
using Climb.Data;
using Climb.FormModels.Admin;
using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Climb.Controllers
{
    //[Authorize(Roles = AdminRole)]
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : Controller
    {
        public const string AdminRole = "Admin";

        private readonly ClimbContext context;
        private readonly IHostingEnvironment environment;
        private readonly ILeagueService leagueService;
        private readonly IAccountService accountService;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(ClimbContext context, IHostingEnvironment environment, ILeagueService leagueService, IAccountService accountService, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.environment = environment;
            this.leagueService = leagueService;
            this.accountService = accountService;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        #region Pages
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region API
        [HttpPost]
        public IActionResult ResetDB()
        {
            DbInitializer.Initialize(context, environment, leagueService, true);
            return Ok("Db reset");
        }

        [HttpPost]
        public async Task<IActionResult> TakeRankSnapshots()
        {
            var leagues = await context.League
                .Include(l => l.Sets)
                .Include(l => l.Members).ThenInclude(lu => lu.RankSnapshots)
                .Include(l => l.Members).ThenInclude(lu => lu.User)
                .ToArrayAsync();

            try
            {
                foreach(var league in leagues)
                {
                    var rankSnapshots = await leagueService.TakeSnapshot(league);
                    if (FeatureToggles.Slack)
                    {
                        await leagueService.SendSnapshotUpdate(rankSnapshots, league); 
                    }
                }
            }
            catch(Exception exception)
            {
                return BadRequest(exception);
            }

            return Ok("Snapshots taken");
        }

        [HttpPost]
        public async Task<IActionResult> SendSetReminders()
        {
            var leagues = await context.League
                .Include(l => l.Seasons).ThenInclude(s => s.Participants).ThenInclude(lu => lu.LeagueUser).ThenInclude(lu => lu.User)
                .Include(l => l.Seasons).ThenInclude(s => s.Sets)
                .ToArrayAsync();

            try
            {
                foreach(var league in leagues)
                {
                    await leagueService.SendSetReminders(league);
                }
            }
            catch(Exception exception)
            {
                return BadRequest(exception);
            }

            return Ok("Set reminders sent");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateAdminAccount()
        {
            if(environment.IsProduction())
            {
                return Unauthorized();
            }

            const string username = "admin";
            const string email = "a@a.com";
            const string password = "Abc_123";

            var adminUser = await context.User.FirstOrDefaultAsync(u => u.Username == username);
            if(adminUser != null)
            {
                await signInManager.PasswordSignInAsync(email, password, true, false);
            }
            else
            {
                //var adminRoleExists = await roleManager.RoleExistsAsync(AdminRole);
                //if(!adminRoleExists)
                //{
                //    var adminRole = new IdentityRole(AdminRole);
                //    var result = await roleManager.CreateAsync(adminRole);
                //    if(!result.Succeeded)
                //    {
                //        return StatusCode(StatusCodes.Status500InternalServerError, "Could not create Admin role.");
                //    }

                //    await context.SaveChangesAsync();
                //}

                try
                {
                    var result = await accountService.CreateUser(email, username, password);
                    if(!result.result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Could not create admin user.");
                    }

                    //var roleResult = await userManager.AddToRoleAsync(result.user, AdminRole);
                    //if(!roleResult.Succeeded)
                    //{
                    //    return StatusCode(StatusCodes.Status500InternalServerError, "Could not add admin user to Admin role.");
                    //}

                    var claimResult = await userManager.AddClaimAsync(result.user, new Claim(ClaimTypes.Role, AdminRole));
                    if (!claimResult.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Could not give Admin claim to admin user.");
                    }
                    await context.SaveChangesAsync();
                }
                catch(Exception exception)
                {
                    return BadRequest($"Could not create Admin User. {exception}");
                }
            }

            return Ok("Admin User Created");
        }

        [HttpPost]
        public IActionResult UpdateFeatureToggles(UpdateFeatureTogglesModel toggles)
        {
            FeatureToggles.Slack = toggles.Slack;
            FeatureToggles.Challonge = toggles.Challonge;

            return Ok("Feature toggles updated.");
        }
        #endregion
    }
}