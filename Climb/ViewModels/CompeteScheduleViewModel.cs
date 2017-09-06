using System.Collections.Generic;
using System.Collections.ObjectModel;
using Climb.Models;

namespace Climb.ViewModels
{
    public class CompeteScheduleViewModel
    {
        public readonly League selectedLeague;
        public readonly Season selectedSeason;
        public readonly ReadOnlyCollection<League> leagues;
        public readonly ReadOnlyCollection<Season> seasons;
        public readonly LeagueUser leagueUser;

        public CompeteScheduleViewModel(League selectedLeague, Season selectedSeason, IList<League> leagues, IList<Season> seasons, LeagueUser leagueUser)
        {
            this.selectedLeague = selectedLeague;
            this.selectedSeason = selectedSeason;
            this.leagueUser = leagueUser;
            this.leagues = new ReadOnlyCollection<League>(leagues);
            this.seasons = new ReadOnlyCollection<Season>(seasons);
        }
    }
}
