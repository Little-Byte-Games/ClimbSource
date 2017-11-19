namespace Climb.Models
{
    public class LeagueUserSeason
    {
        public int LeagueUserID { get; set; }
        public int SeasonID { get; set; }
        public int Standing { get; set; }

        public LeagueUser LeagueUser { get; set; }
        public Season Season { get; set; }
    }
}