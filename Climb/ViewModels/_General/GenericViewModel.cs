using Climb.Models;

namespace Climb.ViewModels
{
    public class GenericViewModel<T> : BaseViewModel
    {
        public readonly T data;

        public GenericViewModel(User user, T data)
            : base(user)
        {
            this.data = data;
        }
    }
}
