namespace Climb.Models
{
    public class LeagueUser
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int LeagueID { get; set; }
        public int Elo { get; set; }
        public string ProfilePicKey { get; set; }

        public User User { get; set; }
        public League League { get; set; }
    }
}
