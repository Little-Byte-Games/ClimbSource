using Climb.Models;

namespace Climb.ViewModels
{
    public class SetCollapsedViewModel
    {
        public readonly Set set;
        public readonly LeagueUser user;
        public readonly string returnUrl;

        public SetCollapsedViewModel(Set set, LeagueUser user, string returnUrl = null)
        {
            this.set = set;
            this.user = user;
            this.returnUrl = returnUrl;
        }
    }
}
