using System;

namespace Climb.Models
{
    public class RankSnapshot
    {
        public int ID { get; set; }
        public int LeagueUserID { get; set; }
        public int Rank { get; set; }
        public int DeltaRank { get; set; }
        public int Elo { get; set; }
        public int DeltaElo { get; set; }
        public DateTime CreatedDate { get; set; }

        public LeagueUser LeagueUser { get; set; }
    }
}
