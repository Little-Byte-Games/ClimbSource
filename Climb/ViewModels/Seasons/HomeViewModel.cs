using Climb.Models;
using System.Collections.Generic;
using System.Linq;
using Set = Climb.Models.Set;

namespace Climb.ViewModels.Seasons
{
    public class HomeViewModel : BaseViewModel
    {
        public readonly Season season;
        public readonly LeagueUser leagueUser;
        public readonly IEnumerable<Set> recentSets;

        public string BracketUrl => $"http://challonge.com/{season.ChallongeUrl}.svg";

        private HomeViewModel(User user, Season season, LeagueUser leagueUser, IEnumerable<Set> recentSets)
            : base(user)
        {
            this.season = season;
            this.leagueUser = leagueUser;
            this.recentSets = recentSets;
        }

        public static HomeViewModel Create(User user, Season season)
        {
            var leagueUser = user.LeagueUsers.FirstOrDefault(l => l.LeagueID == season.LeagueID);

            var recentSets = season.Sets.Where(s => s.IsComplete).OrderByDescending(s => s.UpdatedDate);

            var viewModel = new HomeViewModel(user, season, leagueUser, recentSets);
            return viewModel;
        }
    }
}