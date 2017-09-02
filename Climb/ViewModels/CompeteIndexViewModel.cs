using Climb.Models;
using System.Collections.ObjectModel;

namespace Climb.ViewModels
{
    public class CompeteIndexViewModel
    {
        public readonly User user;
        public readonly ReadOnlyCollection<User> users;
        public readonly ReadOnlyCollection<Set> sets;

        public CompeteIndexViewModel(User user, ReadOnlyCollection<User> users, ReadOnlyCollection<Set> sets)
        {
            this.users = users;
            this.sets = sets;
            this.user = user;
        }
    }
}
