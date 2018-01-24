using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Climb.Models;

namespace Climb.ViewModels
{
    public class CompeteLeaguesViewModel : BaseViewModel
    {
        public readonly ReadOnlyCollection<League> memberOf;
        public readonly ReadOnlyCollection<League> nonmemberOf;
        public readonly ReadOnlyCollection<League> adminOf;
        public readonly ReadOnlyCollection<Game> games;

        private CompeteLeaguesViewModel(User user, IList<League> memberOf, IList<League> nonmemberOf, IList<League> adminOf, IList<Game> games)
            : base(user)
        {
            this.memberOf = new ReadOnlyCollection<League>(memberOf);
            this.nonmemberOf = new ReadOnlyCollection<League>(nonmemberOf);
            this.adminOf = new ReadOnlyCollection<League>(adminOf);
            this.games = new ReadOnlyCollection<Game>(games);
        }

        public static CompeteLeaguesViewModel Create(User user, HashSet<LeagueUser> leagueUsers, IEnumerable<League> leagues, IList<Game> games)
        {
            List<League> memberOf = new List<League>();
            List<League> nonmemberOf = new List<League>();
            List<League> adminOf = new List<League>();

            foreach(var league in leagues)
            {
                if(league.AdminID == user.ID)
                {
                    adminOf.Add(league);
                }

                var leagueUser = leagueUsers.FirstOrDefault(lu => lu.LeagueID == league.ID);
                if(leagueUser == null || leagueUser.HasLeft)
                {
                    nonmemberOf.Add(league);
                }
                else
                {
                    memberOf.Add(league);
                }
            }

            return new CompeteLeaguesViewModel(user, memberOf, nonmemberOf, adminOf, games);
        }
    }
}