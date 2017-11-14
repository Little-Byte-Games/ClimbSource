using System.Collections.Generic;

namespace Climb.Models
{
    public class Division
    {
        public int ID { get; set; }
        public int SeasonID { get; set; }
        public int Index { get; set; }

        public Season Season { get; set; }
        public HashSet<LeagueUser> Players { get; set; }
        public HashSet<Set> Sets { get; set; }
    }
}
