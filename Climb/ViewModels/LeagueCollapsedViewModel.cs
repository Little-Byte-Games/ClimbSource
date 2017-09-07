using Climb.Models;

namespace Climb.ViewModels
{
    public class LeagueCollapsedViewModel
    {
        public readonly League league;
        public readonly League.Membership membership;
        public readonly int userID;

        public LeagueCollapsedViewModel(League league, League.Membership membership, int userID)
        {
            this.league = league;
            this.membership = membership;
            this.userID = userID;
        }
    }
}
