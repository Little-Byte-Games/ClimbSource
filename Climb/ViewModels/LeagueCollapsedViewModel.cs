using Climb.Models;

namespace Climb.ViewModels
{
    public class LeagueCollapsedViewModel
    {
        public readonly League league;
        public readonly League.Membership membership;

        public LeagueCollapsedViewModel(League league, League.Membership membership)
        {
            this.league = league;
            this.membership = membership;
        }
    }
}
