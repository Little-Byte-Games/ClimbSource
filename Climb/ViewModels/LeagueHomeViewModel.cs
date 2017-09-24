using System.Collections.Generic;
using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels
{
    public class LeagueHomeViewModel
    {
        public readonly League league;
        public readonly IConfiguration configuration;
        public readonly IEnumerable<Set> recentSets;

        public LeagueHomeViewModel(League league, IConfiguration configuration, IEnumerable<Set> recentSets)
        {
            this.league = league;
            this.configuration = configuration;
            this.recentSets = recentSets;
        }
    }
}
