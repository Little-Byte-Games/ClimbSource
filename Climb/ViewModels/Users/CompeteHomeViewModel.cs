using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Climb.Models;

namespace Climb.ViewModels
{
    public class CompeteHomeViewModel : BaseViewModel
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

        public readonly User homeUser;
        public readonly HashSet<LeagueUser> possibleExhibitions;
        public readonly ReadOnlyCollection<LeagueUserSet> overdueSets;
        public readonly ReadOnlyCollection<LeagueUserSet> availableSets;
        public readonly ReadOnlyCollection<LeagueUserSeason> seasons;

        public bool IsHome => user == homeUser;

        private CompeteHomeViewModel(User user, User homeUser, HashSet<LeagueUser> possibleExhibitions, IList<LeagueUserSet> overdueSets, IList<LeagueUserSet> availableSets, IList<LeagueUserSeason> seasons) : base(user)
        {
            this.homeUser = homeUser;
            this.possibleExhibitions = possibleExhibitions;
            this.seasons = new ReadOnlyCollection<LeagueUserSeason>(seasons);
            this.overdueSets = new ReadOnlyCollection<LeagueUserSet>(overdueSets);
            this.availableSets = new ReadOnlyCollection<LeagueUserSet>(availableSets);
        }

        public static CompeteHomeViewModel Create(User user, User homeUser)
        {
            var now = DateTime.Now;
            var overdueSets = new List<LeagueUserSet>();
            var availableSets = new List<LeagueUserSet>();
            var seasonUsers = new List<LeagueUserSeason>();
            foreach (var leagueUser in homeUser.LeagueUsers.Where(lu => !lu.HasLeft))
            {
                var seasonUser = leagueUser.Seasons.FirstOrDefault(s => !s.Season.IsComplete && s.Season.Sets.Count > 0);
                if(seasonUser == null)
                {
                    continue;
                }
                seasonUsers.Add(seasonUser);

                var sets = seasonUser.Season.Sets.Where(s => !s.IsComplete && s.IsPlaying(leagueUser));
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

            var isHome = user == homeUser;
            HashSet<LeagueUser> possibleExhibitions = null;
            if(!isHome)
            {
                possibleExhibitions = new HashSet<LeagueUser>(user.LeagueUsers.Where(lu => homeUser.LeagueUsers.Any(vlu => vlu.LeagueID == lu.LeagueID)));
            }

            return new CompeteHomeViewModel(user, homeUser, possibleExhibitions, overdueSets, availableSets, seasonUsers);
        }

        public IEnumerable<RankSnapshot> GetSortedRankSnapshots()
        {
            return user.LeagueUsers.SelectMany(lu => lu.RankSnapshots).OrderByDescending(rs => rs.CreatedDate);
        }
    }
}
