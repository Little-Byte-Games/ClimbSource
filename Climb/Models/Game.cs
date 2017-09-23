using System.Collections.Generic;

namespace Climb.Models
{
    public class Game
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public HashSet<Character> Characters { get; set; }
    }
}
