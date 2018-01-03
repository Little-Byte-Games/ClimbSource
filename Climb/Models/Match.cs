using Newtonsoft.Json;
using System.Collections.Generic;

namespace Climb.Models
{
    public class Match
    {
        public int ID { get; set; }
        public int Index { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public int? StageID { get; set; }

        [JsonIgnore]
        public Set Set { get; set; }
        [JsonIgnore]
        public Stage Stage { get; set; }

        public List<MatchCharacter> MatchCharacters { get; set; }

        public Character WinningCharacter => null;
        public bool IsDitto => false;

        public int Player1CharacterID => 0;
        public int Player2CharacterID => 0;
        public Character Player1Character => null;
        public Character Player2Character => null;
    }
}
