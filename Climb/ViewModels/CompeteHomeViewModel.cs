using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Climb.Models;

namespace Climb.ViewModels
{
    public class CompeteHomeViewModel
    {
        public class LeagueInfo
        {
            public readonly LeagueUser leagueUser;
            public readonly Set nextSet;

            public LeagueInfo(LeagueUser leagueUser, Set nextSet)
            {
                this.leagueUser = leagueUser;
                this.nextSet = nextSet;
            }
        }

        public readonly User user;
        public readonly ReadOnlyCollection<LeagueInfo> leagueInfos;

        public CompeteHomeViewModel(User user, IList<LeagueInfo> leagueInfos)
        {
            this.user = user;
            this.leagueInfos = new ReadOnlyCollection<LeagueInfo>(leagueInfos);
        }

        public static CompeteHomeViewModel Create(User user)
        {
            List<LeagueInfo> leagueInfos = new List<LeagueInfo>();
            foreach (var leagueUser in user.LeagueUsers.Where(lu => !lu.HasLeft))
            {
                var season = leagueUser.League.Seasons.FirstOrDefault(s => !s.IsComplete && s.Sets.Count > 0);
                if(season == null)
                {
                    continue;
                }
                var sets = season.Sets.Where(s => !s.IsComplete && s.IsPlaying(leagueUser));
                Set set = null;
                if(sets.Any())
                {
                    set = sets.Aggregate((c, d) => c.DueDate < d.DueDate ? c : d);
                }

                LeagueInfo leagueInfo = new LeagueInfo(leagueUser, set);
                leagueInfos.Add(leagueInfo);
            }

            return new CompeteHomeViewModel(user, leagueInfos);
        }
    }
}
