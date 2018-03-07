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
        public readonly IEnumerable<Set> overdueSets;
        public readonly IEnumerable<Set> availableSets;

        public string BracketUrl => $"http://challonge.com/{season.ChallongeUrl}.svg";

        private HomeViewModel(User user, Season season, LeagueUser leagueUser, IEnumerable<Set> recentSets, IEnumerable<Set> overdueSets, IEnumerable<Set> availableSets)
            : base(user)
        {
            this.season = season;
            this.leagueUser = leagueUser;
            this.recentSets = recentSets;
            this.overdueSets = overdueSets;
            this.availableSets = availableSets;
        }

        public static HomeViewModel Create(User user, Season season)
        {
            var leagueUser = user.LeagueUsers.FirstOrDefault(l => l.LeagueID == season.LeagueID);

            var recentSets = season.Sets.Where(s => s.IsComplete).OrderByDescending(s => s.UpdatedDate);
            var overdueSets = season.GetOverdueSets().Where(s => s.IsPlaying(leagueUser?.ID));
            var availableSets = season.GetAvailableSets().Where(s => s.IsPlaying(leagueUser?.ID));

            var viewModel = new HomeViewModel(user, season, leagueUser, recentSets, overdueSets, availableSets);
            return viewModel;
        }
    }
}