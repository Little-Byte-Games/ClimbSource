using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Climb.Models
{
    public class Set
    {
        public int ID { get; set; }
        public int Player1ID { get; set; }
        public int Player2ID { get; set; }
        public DateTime UpdatedDate { get; set; }

        //[ForeignKey("Player1ID")]
        [InverseProperty("P1Sets")]
        public User Player1 { get; set; }
        //[ForeignKey("Player2ID")]
        [InverseProperty("P2Sets")]
        public User Player2 { get; set; }
    }
}
