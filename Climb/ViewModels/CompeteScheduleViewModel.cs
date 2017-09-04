using System.Collections.Generic;
using System.Collections.ObjectModel;
using Climb.Models;

namespace Climb.ViewModels
{
    public class CompeteScheduleViewModel
    {
        public readonly int leagueID;
        public readonly int seasonID;
        public readonly ReadOnlyCollection<League> leagues;
        public readonly ReadOnlyCollection<Season> seasons;

        public CompeteScheduleViewModel(IList<League> leagues, IList<Season> seasons, int leagueID, int seasonID)
        {
            this.leagueID = leagueID;
            this.seasonID = seasonID;
            this.leagues = new ReadOnlyCollection<League>(leagues);
            this.seasons = new ReadOnlyCollection<Season>(seasons);
        }
    }
}
