using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        public int KingID { get; set; }
        public DateTime KingReignStart { get; set; }

        public HashSet<LeagueUser> Members { get; set; }
        public HashSet<Season> Seasons { get; set; }
        public HashSet<Set> Sets { get; set; }

        public int MemberCount => Members?.Count(m => !m.HasLeft) ?? 0;
        public Season CurrentSeason => Seasons?.FirstOrDefault(s => !s.IsComplete && s.Sets.Count > 0);

        public int GetKingReignWeeks()
        {
            return (int)((DateTime.Now - KingReignStart).TotalDays / 7);
        }
    }
}