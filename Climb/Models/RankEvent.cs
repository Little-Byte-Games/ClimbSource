namespace Climb.Models
{
    public class RankEvent
    {
        public int ID { get; set; }
        public int SetID { get; set; }
        public int LeagueID { get; set; }
        public int Elo { get; set; }
        public int Rank { get; set; }

        public Set Set { get; set; }
        public League League { get; set; }
    }
}
