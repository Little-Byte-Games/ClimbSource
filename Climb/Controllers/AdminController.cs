using System;
using System.Threading.Tasks;
using Climb.Data;
using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Climb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ClimbContext context;
        private readonly IHostingEnvironment environment;
        private readonly ILeagueService leagueService;

        public AdminController(ClimbContext context, IHostingEnvironment environment, ILeagueService leagueService)
        {
            this.context = context;
            this.environment = environment;
            this.leagueService = leagueService;
        }

        #region Pages
        public IActionResult Index()
        {
            if (!environment.IsDevelopment())
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
            DbInitializer.Initialize(context, environment, true);
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
                foreach (var league in leagues)
                {
                    var rankSnapshots = await leagueService.TakeSnapshot(league);
                    await leagueService.SendSnapshotUpdate(rankSnapshots, league);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }

            return Ok("Snapshots taken");
        }

        [HttpPost]
        public async Task<IActionResult> SendSetReminders()
        {
            var leagues = await context.League
                .Include(l => l.CurrentSeason).ThenInclude(s => s.Participants).ThenInclude(lu => lu.LeagueUser)
                .Include(l => l.CurrentSeason).ThenInclude(s => s.Sets)
                .ToArrayAsync();

            try
            {
                foreach (var league in leagues)
                {
                    await leagueService.SendSetReminders(league);
                }
            }
            catch (Exception exception)
            {
                return BadRequest(exception);
            }

            return Ok("Set reminders sent");
        }
        #endregion
    }
}