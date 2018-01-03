using Newtonsoft.Json;

namespace Climb.Models
{
    public class MatchCharacter
    {
        public int MatchID { get; set; }
        public int CharacterID { get; set; }

        [JsonIgnore]
        public Match Match { get; set; }
        [JsonIgnore]
        public Character Character { get; set; }
    }
}