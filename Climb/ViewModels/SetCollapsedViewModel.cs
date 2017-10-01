using Climb.Models;

namespace Climb.ViewModels
{
    public class SetCollapsedViewModel
    {
        public readonly Set set;
        public readonly LeagueUser user;

        public string SeasonName => set.Season?.DisplayName ?? "Exhibition";

        public SetCollapsedViewModel(Set set, LeagueUser user)
        {
            this.set = set;
            this.user = user;
        }
    }
}
