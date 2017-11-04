﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using Climb.Models;

namespace Climb.ViewModels
{
    public class CompeteScheduleViewModel : BaseViewModel
    {
        public readonly League selectedLeague;
        public readonly Season selectedSeason;
        public readonly ReadOnlyCollection<League> leagues;
        public readonly ReadOnlyCollection<Season> seasons;
        public readonly LeagueUser leagueUser;

        public CompeteScheduleViewModel(User user, League selectedLeague, Season selectedSeason, IList<League> leagues, IList<Season> seasons, LeagueUser leagueUser)
            : base(user)
        {
            this.selectedLeague = selectedLeague;
            this.selectedSeason = selectedSeason;
            this.leagueUser = leagueUser;
            this.leagues = new ReadOnlyCollection<League>(leagues ?? new List<League>());
            this.seasons = new ReadOnlyCollection<Season>(seasons ?? new List<Season>());
        }
    }
}
