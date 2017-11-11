using Climb.Models;
using Climb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Climb.Controllers
{
    // TODO: Replace Details with creating new LeagueUser page.
    public class LeagueUsersController : ModelController
    {
        private readonly ClimbContext context;
        private readonly ICdnService cdnService;
        private readonly LeagueUserService leagueUserService;

        public LeagueUsersController(ClimbContext context, ICdnService cdnService, IUserService userService, UserManager<ApplicationUser> userManager)
            : base(userService, userManager)
        {
            this.context = context;
            this.cdnService = cdnService;
            leagueUserService = new LeagueUserService(context);
        }

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
            return Ok(new {leagueUserID = id, rankDelta = rankDifference });
        }

        public async Task<string> FavoriteCharacter(int id)
        {
            var character = await leagueUserService.GetFavoriteCharacter(id);
            return character?.Name ?? "None";
        }

        #region API
        [HttpPost]
        public async Task<IActionResult> UploadProfilePic(int id, IFormFile file)
        {
            var leagueUser = await context.LeagueUser.SingleOrDefaultAsync(lu => lu.ID == id);
            if(leagueUser == null)
            {
                return NotFound($"No league user with ID '{id}' found.");
            }

            leagueUser.ProfilePicKey = await cdnService.UploadProfilePic(file);
            context.Update(leagueUser);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(UsersController.Account), "Users");
        }
        #endregion
    }
}