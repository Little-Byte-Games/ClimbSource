using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Climb.Models
{
    public class Set
    {
        public int ID { get; set; }
        public int? Player1ID { get; set; }
        public int? Player2ID { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [InverseProperty("P1Sets")]
        [ForeignKey("Player1ID")]
        public LeagueUser Player1 { get; set; }
        [InverseProperty("P2Sets")]
        [ForeignKey("Player2ID")]
        public LeagueUser Player2 { get; set; }
        public ICollection<Match> Matches { get; set; }
        public Season Season { get; set; }

        public bool IsComplete => UpdatedDate != null && UpdatedDate <= DateTime.Now;
        public string Player1Name => Player1?.User?.Username ?? "BYE";
        public string Player2Name => Player2?.User?.Username ?? "BYE";
    }
}
