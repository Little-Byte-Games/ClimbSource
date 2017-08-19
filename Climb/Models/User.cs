using System.Collections.Generic;

namespace Climb.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }

        public HashSet<LeagueUser> LeagueUsers { get; set; }
    }
}
