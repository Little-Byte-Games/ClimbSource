using System.Collections.Generic;
using Climb.Core;
using Climb.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Set = Climb.Models.Set;

namespace Climb.ViewModels.Seasons
{
    public class HomeViewModel : BaseViewModel
    {
        public readonly Season season;
        public readonly LeagueUser leagueUser;
        public readonly ReadOnlyCollection<(LeagueUserSeason user, int possiblePoints)> standings;
        public readonly IEnumerable<Set> recentSets;

        public string BracketUrl => $"http://challonge.com/{season.ChallongeUrl}.svg";

        private HomeViewModel(User user, Season season, LeagueUser leagueUser, ReadOnlyCollection<(LeagueUserSeason user, int possiblePoints)> standings, IEnumerable<Set> recentSets)
            : base(user)
        {
            this.season = season;
            this.leagueUser = leagueUser;
            this.standings = standings;
            this.recentSets = recentSets;
        }

        public static HomeViewModel Create(User user, Season season)
        {
            var leagueUser = user.LeagueUsers.FirstOrDefault(l => l.LeagueID == season.LeagueID);

            var standings = new ReadOnlyCollection<(LeagueUserSeason user, int possiblePoints)>(season.Participants
                .ToList()
                .OrderBy(p => p.Standing)
                .Select(lus => (lus, season.Sets
                    .Count(s => !s.IsComplete && s.IsPlaying(lus.LeagueUserID)) * SeasonStanding.WinningPoints + lus.Points))
                .ToList());

            var recentSets = season.Sets.Where(s => s.IsComplete);

            var viewModel = new HomeViewModel(user, season, leagueUser, standings, recentSets);
            return viewModel;
        }
    }
}