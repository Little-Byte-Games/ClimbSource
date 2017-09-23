using System.Collections.Generic;

namespace Climb.Models
{
    public class Match
    {
        public int ID { get; set; }
        public int Index { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        //public int? Player1CharacterID { get; set; }
        //public int? Player2CharacterID { get; set; }

        public Set Set { get; set; }
        //public Character Player1Character { get; set; }
        //public Character Player2Character { get; set; }

        //public HashSet<Character> Characters => Set.Season.League.Game.Characters;
    }
}
