namespace Climb.Models
{
    public class League
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int GameID { get; set; }
        public int UserID { get; set; }

        public Game Game { get; set; }
        public User User { get; set; }
    }
}
