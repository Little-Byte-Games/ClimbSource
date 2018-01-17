using Climb.Models;

namespace Climb.ViewModels.Sets
{
    public class SetFightViewModel
    {
        public readonly Set set;
        public readonly Match[] matches;

        public string RankP1 => set.Player1.IsNew ? "-" : "#" + set.Player1.Rank;
        public string RankP2 => set.Player2.IsNew ? "-" : "#" + set.Player2.Rank;

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
