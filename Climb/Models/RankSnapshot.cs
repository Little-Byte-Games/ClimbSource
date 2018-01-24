using System;

namespace Climb.Models
{
    public class RankSnapshot
    {
        public int ID { get; set; }
        public int LeagueUserID { get; set; }
        public int Rank { get; set; }
        public int DeltaRank { get; set; }
        public int Points { get; set; }
        public int DeltaPoints { get; set; }
        public DateTime CreatedDate { get; set; }

        public LeagueUser LeagueUser { get; set; }

        public string DisplayDeltaRank => DeltaRank.ToString("-#;+#;0");
        public string DisplayDeltaPoints => DeltaRank.ToString("+#;-#;0");
    }
}
