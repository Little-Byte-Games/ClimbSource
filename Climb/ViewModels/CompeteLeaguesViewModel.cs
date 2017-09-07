using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Climb.Models;

namespace Climb.ViewModels
{
    public class CompeteLeaguesViewModel
    {
        public readonly ReadOnlyCollection<League> memberOf;
        public readonly ReadOnlyCollection<League> nonmemberOf;
        public readonly ReadOnlyCollection<League> adminOf;

        private CompeteLeaguesViewModel(IList<League> memberOf, IList<League> nonmemberOf, IList<League> adminOf)
        {
            this.memberOf = new ReadOnlyCollection<League>(memberOf);
            this.nonmemberOf = new ReadOnlyCollection<League>(nonmemberOf);
            this.adminOf = new ReadOnlyCollection<League>(adminOf);
        }

        public static CompeteLeaguesViewModel Create(HashSet<LeagueUser> leagueUsers, IEnumerable<League> leagues)
        {
            List<League> memberOf = new List<League>();
            List<League> nonmemberOf = new List<League>();
            List<League> adminOf = new List<League>();

            foreach(var league in leagues)
            {
                var leagueUser = leagueUsers.FirstOrDefault(lu => lu.LeagueID == league.ID);
                if(leagueUser == null)
                {
                    nonmemberOf.Add(league);
                }
                else if(league.AdminID == leagueUser.UserID)
                {
                    adminOf.Add(league);
                }
                else
                {
                    memberOf.Add(league);
                }
            }

            return new CompeteLeaguesViewModel(memberOf, nonmemberOf, adminOf);
        }
    }
}
