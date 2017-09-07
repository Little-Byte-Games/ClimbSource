using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Climb.Models
{
    public class League
    {
        public enum Membership
        {
            NonMember,
            Member,
            Admin,
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int GameID { get; set; }
        public int AdminID { get; set; }

        public Game Game { get; set; }
        [ForeignKey(nameof(AdminID))]
        public User Admin { get; set; }
        public HashSet<LeagueUser> Members { get; set; }
        public HashSet<Season> Seasons { get; set; }
    }
}
