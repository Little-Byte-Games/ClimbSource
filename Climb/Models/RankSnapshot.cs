using System;

namespace Climb.Models
{
    public class RankSnapshot : IComparable<RankSnapshot>
    {
        public int ID { get; set; }
        public int LeagueUserID { get; set; }
        public int Rank { get; set; }
        public int DeltaRank { get; set; }
        public int Elo { get; set; }
        public int DeltaElo { get; set; }
        public DateTime CreatedDate { get; set; }

        public LeagueUser LeagueUser { get; set; }

        public int CompareTo(RankSnapshot other)
        {
            if(ReferenceEquals(this, other))
                return 0;
            if(ReferenceEquals(null, other))
                return 1;
            return CreatedDate.CompareTo(other.CreatedDate);
        }
    }
}
