using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Climb.Models
{
    public class Set
    {
        public int ID { get; set; }
        public int? SeasonID { get; set; }
        public int LeagueID { get; set; }
        public int? Player1ID { get; set; }
        public int? Player2ID { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Player1Score { get; set; }
        public int? Player2Score { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeactivated { get; set; }

        [InverseProperty("P1Sets")]
        [ForeignKey("Player1ID")]
        public LeagueUser Player1 { get; set; }
        [InverseProperty("P2Sets")]
        [ForeignKey("Player2ID")]
        public LeagueUser Player2 { get; set; }
        public ICollection<Match> Matches { get; set; }
        [JsonIgnore]
        public Season Season { get; set; }
        [JsonIgnore]
        public League League { get; set; }

        public bool IsComplete => IsBye || (Player1Score != null && Player2Score != null);
        [JsonIgnore]
        public string Player1Name => Player1?.User?.Username ?? "BYE";
        [JsonIgnore]
        public string Player2Name => Player2?.User?.Username ?? "BYE";
        public bool IsBye => Player1ID == null || Player2ID == null;
        public bool IsExhibition => SeasonID == null;
        [JsonIgnore]
        public int? WinnerID => Player1Score > Player2Score ? Player1ID : Player1Score < Player2Score ? Player2ID : null;
        [JsonIgnore]
        public int? LoserID => Player1Score > Player2Score ? Player2ID : Player1Score < Player2Score ? Player1ID : null;

        public bool IsPlaying(int? leagueUserID)
        {
            return leagueUserID == Player1ID || leagueUserID == Player2ID;
        }

        public bool IsPlaying(LeagueUser leagueUser)
        {
            return leagueUser != null && (Player1 == leagueUser || Player2 == leagueUser);
        }

        public LeagueUser GetLeagueUser(User user)
        {
            return Player1?.UserID == user.ID ? Player1 : Player2;
        }

        public int GetOpponentID(int participantID)
        {
            Debug.Assert(IsPlaying(participantID), "Can't get opponent if participant is not even in the set.");
            Debug.Assert(Player2ID != null, nameof(Player2ID) + " != null");
            Debug.Assert(Player1ID != null, nameof(Player1ID) + " != null");
            return Player1ID == participantID ? Player2ID.Value : Player1ID.Value;
        }
    }
}
