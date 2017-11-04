using System.Collections.Generic;
using Climb.Models;

namespace Climb.ViewModels
{
    public class AvailableSetsViewModel : BaseViewModel
    {
        public readonly List<Set> sets;

        public AvailableSetsViewModel(User user, List<Set> sets)
            : base(user)
        {
            this.sets = sets;
        }
    }
}