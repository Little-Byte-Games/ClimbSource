using Climb.Models;

namespace Climb.ViewModels.Sets
{
    public class SetFightViewModel
    {
        public readonly Set set;
        public readonly Match[] matches;

        private SetFightViewModel(Set set, Match[] matches)
        {
            this.set = set;
            this.matches = matches;
        }

        public static SetFightViewModel Create(Set set, int maxMatchCount)
        {
            var matches = new Match[maxMatchCount];
            foreach(var match in set.Matches)
            {
                matches[match.Index] = match;
            }

            return new SetFightViewModel(set, matches);
        }
    }
}
