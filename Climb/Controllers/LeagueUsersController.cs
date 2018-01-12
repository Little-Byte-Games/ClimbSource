using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Climb.Controllers
{
    public class LeagueUsersController : ModelController
    {
        private readonly ClimbContext context;
        private readonly CdnService cdnService;

        public LeagueUsersController(ClimbContext context, CdnService cdnService, IUserService userService, UserManager<ApplicationUser> userManager)
            : base(userService, userManager)
        {
            this.context = context;
            this.cdnService = cdnService;
        }

        #region API
        public async Task<IActionResult> GetUserTrend(int id)
        {
            var leagueUser = await context.LeagueUser
                .Include(lu => lu.RankSnapshots)
                .SingleOrDefaultAsync(lu => lu.ID == id);
            if(leagueUser == null)
            {
                return NotFound();
            }

            var rankDifference = leagueUser.GetRankTrendDelta();
            return Ok(new {leagueUserID = id, rankDelta = rankDifference});
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSlackUsername(int id, string slackUsername)
        {
            var leagueUser = await context.LeagueUser.SingleOrDefaultAsync(lu => lu.ID == id);
            if(leagueUser == null)
            {
                return NotFound($"No league user with ID '{id}' found.");
            }

            leagueUser.SlackUsername = slackUsername;
            context.Update(leagueUser);
            await context.SaveChangesAsync();

            return Ok(new {id, slackUsername});
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDisplayName(int id, string displayName)
        {
            var leagueUser = await context.LeagueUser.SingleOrDefaultAsync(lu => lu.ID == id);
            if(leagueUser == null)
            {
                return NotFound($"No league user with ID '{id}' found.");
            }

            var regex = new Regex(@"\s+");
            var strippedName = regex.Replace(displayName.ToLower(), string.Empty);
            var isNameTaken = await context.LeagueUser.AnyAsync(lu => lu.ID != id && regex.Replace(lu.DisplayName, string.Empty) == strippedName);
            if(isNameTaken)
            {
                return BadRequest($"League member with name similar to '{displayName}' already exists.");
            }

            leagueUser.DisplayName = displayName;
            context.Update(leagueUser);
            await context.SaveChangesAsync();

            return Ok(leagueUser);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePoints(int id, int points)
        {
            var leagueUser = await context.LeagueUser.FirstOrDefaultAsync(lu => lu.ID == id);
            if(leagueUser == null)
            {
                return NotFound($"No league user with ID '{id}' found.");
            }

            leagueUser.Points = points;
            context.Update(leagueUser);
            await context.SaveChangesAsync();

            return Ok(leagueUser);
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> UploadProfilePic(int id, IFormFile file)
        {
            var leagueUser = await context.LeagueUser.SingleOrDefaultAsync(lu => lu.ID == id);
            if(leagueUser == null)
            {
                return NotFound($"No league user with ID '{id}' found.");
            }

            leagueUser.ProfilePicKey = await cdnService.UploadImage(CdnService.ImageTypes.ProfilePic, file);
            context.Update(leagueUser);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(UsersController.Account), "Users");
        }
    }
}