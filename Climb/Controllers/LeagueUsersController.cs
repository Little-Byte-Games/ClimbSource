using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update([Bind("ID,DisplayName,SlackUsername,ChallongeUsername")]
            LeagueUser leagueUser)
        {
            if(TryValidateModel(leagueUser))
            {
                var leagueUserToUpdate = await context.LeagueUser.SingleOrDefaultAsync(lu => lu.ID == leagueUser.ID);
                if(leagueUserToUpdate == null)
                {
                    return NotFound($"No LeagueUser with ID '{leagueUser.ID}' found.");
                }

                var updateSuccess = await TryUpdateModelAsync(leagueUserToUpdate,
                    "",
                    u => u.DisplayName,
                    u => u.SlackUsername,
                    u => u.ChallongeUsername);

                if(updateSuccess)
                {
                    await context.SaveChangesAsync();
                    return Ok(leagueUser);
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
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