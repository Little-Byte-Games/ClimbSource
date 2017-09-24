using System.Collections.Generic;
using System.Linq;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels
{
    public class LeagueHomeViewModel
    {
        public readonly League league;
        public readonly IConfiguration configuration;
        public readonly IEnumerable<Set> recentSets;
        public readonly Season selectedSeason;

        public LeagueHomeViewModel(League league, IConfiguration configuration, IEnumerable<Set> recentSets, int? seasonID)
        {
            this.league = league;
            this.configuration = configuration;
            this.recentSets = recentSets;
            selectedSeason = seasonID == null ? null : league.Seasons.Single(s => s.ID == seasonID);
        }
    }
}
