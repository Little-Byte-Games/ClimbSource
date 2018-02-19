using Climb.Models;
using System.Linq;

namespace Climb.ViewModels
{
    public class SetCollapsedViewModel
    {
        public readonly Set set;
        public readonly LeagueUser user;
        public readonly string returnUrl;
        public readonly string buttonLabel;
        public readonly bool showButton;

        private SetCollapsedViewModel(Set set, LeagueUser user, bool showButton, string buttonLabel, string returnUrl = null)
        {
            this.set = set;
            this.user = user;
            this.returnUrl = returnUrl;
            this.showButton = showButton;
            this.buttonLabel = buttonLabel;
        }

        public static SetCollapsedViewModel Create(Set set, User user, string returnUrl = null)
        {
            var leagueUser = user.LeagueUsers.FirstOrDefault(lu => lu.LeagueID == set.LeagueID);
            return Create(set, leagueUser, returnUrl);
        }

        public static SetCollapsedViewModel Create(Set set, LeagueUser leagueUser, string returnUrl = null)
        {
            bool showButton = false;
            string buttonLabel = "Details";

            if(set.IsPlaying(leagueUser?.ID))
            {
                showButton = true;
                buttonLabel = set.IsLocked ? "Details" : set.IsComplete ? "Edit" : "Fight";
            }
            else if(set.IsComplete)
            {
                showButton = true;
                buttonLabel = "Details";
            }

            return new SetCollapsedViewModel(set, leagueUser, showButton, buttonLabel, returnUrl);
        }
    }
}