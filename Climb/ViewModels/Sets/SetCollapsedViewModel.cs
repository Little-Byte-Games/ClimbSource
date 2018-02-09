using Climb.Models;

namespace Climb.ViewModels
{
    public class SetCollapsedViewModel
    {
        public readonly Set set;
        public readonly LeagueUser user;
        public readonly string returnUrl;
        public readonly string buttonLabel;
        public readonly bool showButton;

        public SetCollapsedViewModel(Set set, LeagueUser user, string returnUrl = null)
        {
            this.set = set;
            this.user = user;
            this.returnUrl = returnUrl;

            if(set.IsPlaying(user?.ID))
            {
                showButton = true;
                buttonLabel = set.IsLocked ? "Details" : set.IsComplete ? "Edit" : "Fight";
            }
            else if(set.IsComplete)
            {
                showButton = true;
                buttonLabel = "Details";
            }

            buttonLabel = set.IsPlaying(user) ? set.IsComplete ? "Edit" : "Fight" : "Details";
        }
    }
}
