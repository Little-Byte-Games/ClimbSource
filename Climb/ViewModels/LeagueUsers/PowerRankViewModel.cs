using Climb.Models;
using Climb.Services;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Climb.ViewModels.LeagueUsers
{
    public class PowerRankViewModel
    {
        private const int CharacterMax = 3;

        public readonly LeagueUser leagueUser;
        public readonly LeagueUser viewingLeagueUser;
        public readonly string profilePicUrl;
        public readonly ImmutableArray<string> characterPicUrls;
        public readonly bool showLeague;

        public bool ShowFightButton => false;
        public string Rank => leagueUser.IsNew ? "-" : leagueUser.Rank.ToString();

        private PowerRankViewModel(LeagueUser leagueUser, LeagueUser viewingLeagueUser, string profilePicUrl, IEnumerable<string> characterPicUrls, bool showLeague)
        {
            this.leagueUser = leagueUser;
            this.viewingLeagueUser = viewingLeagueUser;
            this.profilePicUrl = profilePicUrl;
            this.showLeague = showLeague;
            this.characterPicUrls = characterPicUrls.ToImmutableArray();
        }

        public static async Task<PowerRankViewModel> Create(LeagueUser leagueUser, User viewingUser, CdnService cdnService, ILeagueUserService leagueUserService, bool showLeague)
        {
            var viewingLeagueUser = viewingUser.LeagueUsers.FirstOrDefault(lu => lu.LeagueID == leagueUser.LeagueID);
            var profilePicUrl = cdnService.GetImageUrl(CdnService.ImageTypes.ProfilePic, leagueUser.ProfilePicKey);
            var characters = await leagueUserService.GetMostUsedCharacters(leagueUser.ID, CharacterMax);
            var characterPicUrls = characters.Select(c => cdnService.GetImageUrl(CdnService.ImageTypes.CharacterPic, c.PicKey));

            return new PowerRankViewModel(leagueUser, viewingLeagueUser, profilePicUrl, characterPicUrls, showLeague);
        }
    }
}