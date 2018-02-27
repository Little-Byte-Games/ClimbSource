using Climb.Models;

namespace Climb.ViewModels.Games
{
    public class HomeViewModel : BaseViewModel
    {
        public readonly Game game;
        public readonly bool isAdmin;

        public HomeViewModel(User user, Game game)
            : base(user)
        {
            this.game = game;

#if DEBUG
            isAdmin = true;
#else
            isAdmin = false;
#endif
        }
    }
}
