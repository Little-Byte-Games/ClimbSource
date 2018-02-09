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

        #region Page Forms
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update([Bind("ID,DisplayName,SlackUsername,ChallongeUsername")]LeagueUser leagueUser)
        {
            if(TryValidateModel(leagueUser))
            {
                var leagueUserToUpdate = await context.LeagueUser.SingleOrDefaultAsync(lu => lu.ID == leagueUser.ID);
                if(leagueUserToUpdate == null)
                {
                    return NotFound($"No LeagueUser with ID '{leagueUser.ID}' found.");
                }

                var updateSuccess = await TryUpdateModelAsync(leagueUserToUpdate,
                    "leagueUser",
                    u => u.DisplayName,
                    u => u.SlackUsername,
                    u => u.ChallongeUsername);

                if(updateSuccess)
                {
                    await context.SaveChangesAsync();
                    return RedirectToAction("Account", "Users");
                }
            }

            return RedirectToAction("Account", "Users");
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> UploadProfilePic(int id, IFormFile file)
        {
            var leagueUser = await context.LeagueUser
                .Include(lu => lu.User).AsNoTracking()
                .SingleOrDefaultAsync(lu => lu.ID == id);
            if(leagueUser == null)
            {
                return NotFound($"No league user with ID '{id}' found.");
            }

            if(!string.IsNullOrWhiteSpace(leagueUser.ProfilePicKey) && leagueUser.User.ProfilePicKey != leagueUser.ProfilePicKey)
            {
                await cdnService.DeleteImage(CdnService.ImageTypes.ProfilePic, leagueUser.ProfilePicKey);
            }

            leagueUser.ProfilePicKey = await cdnService.UploadImage(CdnService.ImageTypes.ProfilePic, file);
            context.Update(leagueUser);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(UsersController.Account), "Users");
        }
    }
}