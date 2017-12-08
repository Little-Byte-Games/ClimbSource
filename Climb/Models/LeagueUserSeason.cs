using Newtonsoft.Json;

namespace Climb.Models
{
    public class LeagueUserSeason
    {
        public int LeagueUserID { get; set; }
        public int SeasonID { get; set; }
        public int Standing { get; set; }
        public int Points { get; set; }
        public int ChallongeID { get; set; }

        [JsonIgnore]
        public LeagueUser LeagueUser { get; set; }
        [JsonIgnore]
        public Season Season { get; set; }
    }
}