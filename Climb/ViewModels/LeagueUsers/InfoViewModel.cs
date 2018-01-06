using System.Linq;
using Climb.Models;
using Climb.Services;

namespace Climb.ViewModels.LeagueUsers
{
    public class InfoViewModel
    {
        public readonly LeagueUser leagueUser;
        public readonly LeagueUser viewingLeagueUser;
        public readonly string profilePicUrl;

        private InfoViewModel(LeagueUser leagueUser, LeagueUser viewingLeagueUser, string profilePicUrl)
        {
            this.leagueUser = leagueUser;
            this.viewingLeagueUser = viewingLeagueUser;
            this.profilePicUrl = profilePicUrl;
        }

        public static InfoViewModel Create(LeagueUser leagueUser, User viewingUser, ICdnService cdnService)
        {
            var viewingLeagueUser = viewingUser.LeagueUsers.FirstOrDefault(lu => lu.LeagueID == leagueUser.LeagueID);
            var profilePicUrl = cdnService.GetProfilePic(leagueUser);
            return new InfoViewModel(leagueUser, viewingLeagueUser, profilePicUrl);
        }
    }
}