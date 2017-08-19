﻿using System;
using System.Collections.Generic;

namespace Climb.Models
{
    public class Season
    {
        public int ID { get; set; }
        public int LeagueID { get; set; }
        public int Index { get; set; }
        public DateTime StartDate { get; set; }
        public HashSet<LeagueUser> Participants { get; set; }

        public League League { get; set; }
    }
}
