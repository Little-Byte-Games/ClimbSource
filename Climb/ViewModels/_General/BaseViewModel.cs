using Climb.Models;

namespace Climb.ViewModels
{
    public abstract class BaseViewModel
    {
        public readonly User user;

        protected BaseViewModel(User user)
        {
            this.user = user;
        }
    }
}
