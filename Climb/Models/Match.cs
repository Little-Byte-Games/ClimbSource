﻿using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Climb.Models
{
    public class Match
    {
        public int ID { get; set; }
        public int Index { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public int? StageID { get; set; }

        [JsonIgnore]
        public Set Set { get; set; }
        [JsonIgnore]
        public Stage Stage { get; set; }

        public List<MatchCharacter> MatchCharacters { get; set; }

        public Character WinningCharacter => null;
        public bool IsDitto => false;

        [Obsolete]
        public int Player1CharacterID => 0;
        [Obsolete]
        public int Player2CharacterID => 0;
        [Obsolete]
        public Character Player1Character => null;
        [Obsolete]
        public Character Player2Character => null;
    }
}
