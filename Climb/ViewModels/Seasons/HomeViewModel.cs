using System.Collections.ObjectModel;
using System.Linq;
using Climb.Models;

namespace Climb.ViewModels.Seasons
{
    public class HomeViewModel : BaseViewModel
    {
        public readonly Season season;
        public readonly LeagueUser leagueUser;
        public readonly ReadOnlyCollection<LeagueUserSeason> standings;

        public string BracketUrl => $"http://challonge.com/{season.ChallongeUrl}.svg";

        protected HomeViewModel(User user, Season season, LeagueUser leagueUser, ReadOnlyCollection<LeagueUserSeason> standings)
            : base(user)
        {
            this.season = season;
            this.leagueUser = leagueUser;
            this.standings = standings;
        }

        public static HomeViewModel Create(User user, Season season)
        {
            var leagueUser = user.LeagueUsers.FirstOrDefault(l => l.LeagueID == season.LeagueID);

            var standings = new ReadOnlyCollection<LeagueUserSeason>(season.Participants.ToList().OrderBy(p => p.Standing).ToList());

            var viewModel = new HomeViewModel(user, season, leagueUser, standings);
            return viewModel;
        }
    }
}
