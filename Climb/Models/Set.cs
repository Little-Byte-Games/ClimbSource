using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Climb.Models
{
    public class Set
    {
        public int ID { get; set; }
        public int Player1ID { get; set; }
        public int Player2ID { get; set; }
        public DateTime UpdatedDate { get; set; }

        [InverseProperty("P1Sets")]
        public User Player1 { get; set; }
        [InverseProperty("P2Sets")]
        public User Player2 { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
