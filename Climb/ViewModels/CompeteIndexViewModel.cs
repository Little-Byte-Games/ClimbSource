using System.Collections.Generic;
using Climb.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace Climb.ViewModels
{
    public class CompeteIndexViewModel
    {
        public readonly User user;
        public readonly ReadOnlyCollection<User> users;
        public readonly ReadOnlyCollection<Set> pastSets;
        public readonly ReadOnlyCollection<Set> availableSets;

        public CompeteIndexViewModel(User user, ReadOnlyCollection<User> users, List<Set> sets)
        {
            this.users = users;
            this.user = user;

            pastSets = new ReadOnlyCollection<Set>(sets.Where(s => s.IsComplete).ToList());
            availableSets = new ReadOnlyCollection<Set>(sets.Where(s => !s.IsComplete).ToList());
        }
    }
}
