using System.Collections.Generic;
using Climb.Models;

namespace Climb.ViewModels
{
    public class LeagueJoinViewModel
    {
        public readonly League league;
        public readonly IEnumerable<User> potentialMembers;
        public readonly IEnumerable<LeagueUser> members;

        public LeagueJoinViewModel(League league, IEnumerable<User> potentialMembers, IEnumerable<LeagueUser> members)
        {
            this.league = league;
            this.potentialMembers = potentialMembers ?? new List<User>();
            this.members = members ?? new List<LeagueUser>();
        }
    }
}