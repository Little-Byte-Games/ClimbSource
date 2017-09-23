using System.Collections.Generic;
using Climb.Models;

namespace Climb.ViewModels
{
    public class AvailableSetsViewData
    {
        public readonly User user;
        public readonly List<Set> sets;

        public AvailableSetsViewData(User user, List<Set> sets)
        {
            this.user = user;
            this.sets = sets;
        }
    }
}