using Climb.Models;
using System.Linq;

namespace Climb.ViewModels.Sets
{
    public class SetFightViewModel
    {
        public readonly Set set;
        public readonly Match[] matches;
        public readonly bool canSubmit;
        public readonly Set lastSetP1;
        public readonly Set lastSetP2;

        public string RankP1 => set.Player1.IsNew ? "-" : "#" + set.Player1.Rank;
        public string RankP2 => set.Player2.IsNew ? "-" : "#" + set.Player2.Rank;

        private SetFightViewModel(Set set, Match[] matches, bool canSubmit, Set lastSetP1, Set lastSetP2)
        {
            this.set = set;
            this.matches = matches;
            this.canSubmit = canSubmit;
            this.lastSetP1 = lastSetP1;
            this.lastSetP2 = lastSetP2;

#if DEBUG
            this.canSubmit = true;
#endif
        }

        public static SetFightViewModel Create(Set set, int maxMatchCount, User viewer)
        {
            Set lastSetP1 = null;
            Set lastSetP2 = null;
            if(!set.IsComplete)
            {
                var leagueSets = set.Player1.League.Sets.Where(s => s.IsComplete).OrderByDescending(s => s.UpdatedDate);
                foreach(var leagueSet in leagueSets)
                {
                    if(lastSetP1 == null && leagueSet.IsPlaying(set.Player1))
                    {
                        lastSetP1 = leagueSet;
                    }

                    if(lastSetP2 == null && leagueSet.IsPlaying(set.Player2))
                    {
                        lastSetP2 = leagueSet;
                    }

                    if(lastSetP1 != null && lastSetP2 != null)
                    {
                        break;
                    }
                }
            }

            var matches = new Match[maxMatchCount];
            foreach(var match in set.Matches)
            {
                matches[match.Index] = match;
            }

            var canSubmit = !set.IsLocked && viewer.LeagueUsers.Any(set.IsPlaying);

            return new SetFightViewModel(set, matches, canSubmit, lastSetP1, lastSetP2);
        }
    }
}