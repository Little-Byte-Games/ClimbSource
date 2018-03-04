using Climb.Data;
using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Climb.Consts;
using Climb.FormModels.Admin;

namespace Climb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ClimbContext context;
        private readonly IHostingEnvironment environment;
        private readonly ILeagueService leagueService;
        private readonly IAccountService accountService;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AdminController(ClimbContext context, IHostingEnvironment environment, ILeagueService leagueService, IAccountService accountService, SignInManager<ApplicationUser> signInManager)
        {
            this.context = context;
            this.environment = environment;
            this.leagueService = leagueService;
            this.accountService = accountService;
            this.signInManager = signInManager;
        }

        #region Pages
        public IActionResult Index()
        {
            if(!environment.IsDevelopment())
            {
                return NotFound("You're not a site admin!");
            }

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
                    await leagueService.SendSetReminders(league, Url);
                }
            }
            catch(Exception exception)
            {
                return BadRequest(exception);
            }

            return Ok("Set reminders sent");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdminAccount()
        {
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
                try
                {
                    await accountService.CreateUser(email, username, password);
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

        [HttpPost]
        public async Task<IActionResult> SyncSetsPlayed()
        {
            var leagues = await context.League
                .Include(l => l.Members).ThenInclude(lu => lu.P1Sets)
                .Include(l => l.Members).ThenInclude(lu => lu.P2Sets)
                .ToListAsync();

            foreach(var league in leagues)
            {
                context.UpdateRange(league.Members);
                foreach(var member in league.Members)
                {
                    member.SetsPlayed = member.P1Sets.Union(member.P2Sets).Count(s => s.IsComplete && !s.IsBye);
                }
            }

            await context.SaveChangesAsync();
            return Ok("Sets played updated!");
        }
        #endregion
    }
}