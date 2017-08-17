using System.Collections.Generic;

namespace Climb.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }

        public ICollection<Set> P1Sets { get; set; }
        public ICollection<Set> P2Sets { get; set; }
    }
}
