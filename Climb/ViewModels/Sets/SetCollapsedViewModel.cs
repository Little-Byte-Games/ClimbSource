using Climb.Models;

namespace Climb.ViewModels
{
    public class SetCollapsedViewModel
    {
        public readonly Set set;
        public readonly LeagueUser user;

        public SetCollapsedViewModel(Set set, LeagueUser user)
        {
            this.set = set;
            this.user = user;
        }
    }
}
