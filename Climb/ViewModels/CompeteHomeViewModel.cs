using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Climb.Models;

namespace Climb.ViewModels
{
    public class CompeteHomeViewModel
    {
        public class LeagueUserSet
        {
            public readonly LeagueUser leagueUser;
            public readonly Set set;

            public LeagueUserSet(LeagueUser leagueUser, Set set)
            {
                this.leagueUser = leagueUser;
                this.set = set;
            }
        }

        public readonly User user;
        public readonly User viewingUser;
        public readonly HashSet<LeagueUser> possibleExhibitions;
        public readonly ReadOnlyCollection<LeagueUserSet> overdueSets;
        public readonly ReadOnlyCollection<LeagueUserSet> availableSets;

        public bool IsHome => user == viewingUser;

        public CompeteHomeViewModel(User user, User viewingUser, HashSet<LeagueUser> possibleExhibitions, IList<LeagueUserSet> overdueSets, IList<LeagueUserSet> availableSets)
        {
            this.user = user;
            this.viewingUser = viewingUser;
            this.possibleExhibitions = possibleExhibitions;
            this.overdueSets = new ReadOnlyCollection<LeagueUserSet>(overdueSets);
            this.availableSets = new ReadOnlyCollection<LeagueUserSet>(availableSets);
        }

        public static CompeteHomeViewModel Create(User user, User viewingUser)
        {
            DateTime now = DateTime.Now;
            List<LeagueUserSet> overdueSets = new List<LeagueUserSet>();
            List<LeagueUserSet> availableSets = new List<LeagueUserSet>();
            foreach (var leagueUser in user.LeagueUsers.Where(lu => !lu.HasLeft))
            {
                var season = leagueUser.League.Seasons.FirstOrDefault(s => !s.IsComplete && s.Sets.Count > 0);
                if(season == null)
                {
                    continue;
                }
                var sets = season.Sets.Where(s => !s.IsComplete && s.IsPlaying(leagueUser));
                foreach(var set in sets)
                {
                    if(set.DueDate < now)
                    {
                        var leagueUserSet = new LeagueUserSet(leagueUser, set);
                        overdueSets.Add(leagueUserSet);
                    }
                    else if (availableSets.All(s => s.leagueUser != leagueUser))
                    {
                        var leagueUserSet = new LeagueUserSet(leagueUser, set);
                        availableSets.Add(leagueUserSet);
                    }
                }
            }

            var isHome = user == viewingUser;
            HashSet<LeagueUser> possibleExhibitions = null;
            if(!isHome)
            {
                possibleExhibitions = new HashSet<LeagueUser>(user.LeagueUsers.Where(lu => viewingUser.LeagueUsers.Any(vlu => vlu.LeagueID == lu.LeagueID)));
            }

            return new CompeteHomeViewModel(user, viewingUser, possibleExhibitions, overdueSets, availableSets);
        }

        public IEnumerable<RankSnapshot> GetSortedRankSnapshots()
        {
            return user.LeagueUsers.SelectMany(lu => lu.RankSnapshots).OrderByDescending(rs => rs.CreatedDate);
        }
    }
}
