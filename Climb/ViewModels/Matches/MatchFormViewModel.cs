using Climb.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace Climb.ViewModels.Matches
{
    public class MatchFormViewModel
    {
        public const int NotSelectedValue = -1;
        public const string NotSelected = "---";

        public readonly int matchID;
        public readonly int index;
        public readonly int player1Score;
        public readonly int player2Score;
        public readonly List<int> player1Characters;
        public readonly List<int> player2Characters;
        public readonly int stage;
        public readonly Set set;

        public MatchFormViewModel(int index, Set set, Match match)
        {
            Debug.Assert(set.Player1ID != null, "set.Player1ID != null");
            Debug.Assert(set.Player2ID != null, "set.Player2ID != null");

            matchID = match?.ID ?? 0;
            this.index = index;
            this.set = set;
            player1Score = match?.Player1Score ?? 0;
            player2Score = match?.Player2Score ?? 0;

            player1Characters = new List<int>();
            player2Characters = new List<int>();

            if(match != null)
            {
                foreach(var matchCharacter in match.MatchCharacters)
                {
                    var characterList = matchCharacter.LeagueUserID == set.Player1ID.Value ? player1Characters : player2Characters;
                    characterList.Add(matchCharacter.CharacterID);
                }
            }

            while(player1Characters.Count < set.League.Game.CharactersPerMatch)
            {
                player1Characters.Add(NotSelectedValue);
            }

            while(player2Characters.Count < set.League.Game.CharactersPerMatch)
            {
                player2Characters.Add(NotSelectedValue);
            }

            stage = match?.StageID ?? NotSelectedValue;
        }
    }
}