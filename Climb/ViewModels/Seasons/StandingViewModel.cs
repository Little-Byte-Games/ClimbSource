using Climb.Models;

namespace Climb.ViewModels.Seasons
{
    public class StandingViewModel
    {
        public readonly LeagueUserSeason participant;
        public readonly bool showSeason;

        public StandingViewModel(LeagueUserSeason participant, bool showSeason)
        {
            this.participant = participant;
            this.showSeason = showSeason;
        }
    }
}