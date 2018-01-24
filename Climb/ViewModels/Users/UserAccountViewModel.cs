using Climb.Models;

namespace Climb.ViewModels.Users
{
    public class UserAccountViewModel : BaseViewModel
    {
        public readonly ApplicationUser appUser;

        public UserAccountViewModel(User user, ApplicationUser appUser)
            : base(user)
        {
            this.appUser = appUser;
        }
    }
}
