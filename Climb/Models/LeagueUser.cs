﻿using System.Collections.Generic;

namespace Climb.Models
{
    public class LeagueUser
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int LeagueID { get; set; }
        public int Elo { get; set; }
        public string ProfilePicKey { get; set; }
        public bool HasLeft { get; set; }

        public User User { get; set; }
        public League League { get; set; }
        public HashSet<Set> P1Sets { get; set; }
        public HashSet<Set> P2Sets { get; set; }
    }
}
