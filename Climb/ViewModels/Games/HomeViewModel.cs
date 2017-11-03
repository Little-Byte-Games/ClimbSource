using Climb.Models;

namespace Climb.ViewModels.Games
{
    public class HomeViewModel : BaseViewModel
    {
        public readonly Game game;

        public HomeViewModel(User user, Game game)
            : base(user)
        {
            this.game = game;
        }
    }
}
