using Climb.Models;
using Microsoft.Extensions.Configuration;

namespace Climb.ViewModels
{
    public class LeagueHomeViewModel
    {
        public readonly League league;
        public readonly IConfiguration configuration;

        public LeagueHomeViewModel(League league, IConfiguration configuration)
        {
            this.league = league;
            this.configuration = configuration;
        }
    }
}
