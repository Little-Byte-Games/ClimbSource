using Climb.Models;
using System.Linq;

namespace Climb.ViewModels.Sets
{
    public class SetFightViewModel
    {
        public readonly Set set;
        public readonly Match[] matches;
        public readonly bool canSubmit;

        public string RankP1 => set.Player1.IsNew ? "-" : "#" + set.Player1.Rank;
        public string RankP2 => set.Player2.IsNew ? "-" : "#" + set.Player2.Rank;

        private SetFightViewModel(Set set, Match[] matches, bool canSubmit)
        {
            this.set = set;
            this.matches = matches;
            this.canSubmit = canSubmit;
        }

        public static SetFightViewModel Create(Set set, int maxMatchCount, User viewer)
        {
            var matches = new Match[maxMatchCount];
            foreach(var match in set.Matches)
            {
                matches[match.Index] = match;
            }

            var canSubmit = !set.IsLocked && viewer.LeagueUsers.Any(set.IsPlaying);

            return new SetFightViewModel(set, matches, canSubmit);
        }
    }
}