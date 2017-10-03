using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Climb.Models
{
    public class Set
    {
        public int ID { get; set; }
        public int? SeasonID { get; set; }
        public int LeagueID { get; set; }
        public int? Player1ID { get; set; }
        public int? Player2ID { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Player1Score { get; set; }
        public int? Player2Score { get; set; }

        [InverseProperty("P1Sets")]
        [ForeignKey("Player1ID")]
        public LeagueUser Player1 { get; set; }
        [InverseProperty("P2Sets")]
        [ForeignKey("Player2ID")]
        public LeagueUser Player2 { get; set; }
        public ICollection<Match> Matches { get; set; }
        public Season Season { get; set; }

        public League League { get; set; }

        public bool IsComplete => Player1Score != null || Player2Score != null;
        public string Player1Name => Player1?.User?.Username ?? "BYE";
        public string Player2Name => Player2?.User?.Username ?? "BYE";
        public bool IsBye => Player1ID == null || Player2ID == null;
        public bool IsExhibition => SeasonID == null;

        public bool IsPlaying(LeagueUser leagueUser)
        {
            return leagueUser != null && (Player1 == leagueUser || Player2 == leagueUser);
        }
    }
}
