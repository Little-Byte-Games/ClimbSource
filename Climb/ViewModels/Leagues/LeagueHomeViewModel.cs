using Climb.Models;

namespace Climb.ViewModels
{
    public class LeagueHomeViewModel : BaseViewModel
    {
        public readonly League league;
        public readonly bool isAdmin;

        public LeagueHomeViewModel(User user, League league)
            : base(user)
        {
            this.league = league;

#if DEBUG
            isAdmin = true;
#else
            isAdmin = league.AdminID == user.ID;
#endif
        }
    }
}