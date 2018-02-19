using Climb.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public MatchFormViewModel(int index, Set set, int matchID,
            int player1Score, int player2Score,
            List<int> player1Characters, List<int> player2Characters,
            int stage)
        {
            this.index = index;
            this.set = set;
            this.matchID = matchID;
            this.player1Score = player1Score;
            this.player2Score = player2Score;
            this.player1Characters = player1Characters;
            this.player2Characters = player2Characters;
            this.stage = stage;
        }

        public static MatchFormViewModel Create(int index, Set set, Match match, Set lastSetP1, Set lastSetP2)
        {
            Debug.Assert(set.Player1ID != null, "set.Player1ID != null");
            Debug.Assert(set.Player2ID != null, "set.Player2ID != null");

            var player1Characters = new List<int>();
            var player2Characters = new List<int>();

            if(match != null)
            {
                foreach(var matchCharacter in match.MatchCharacters)
                {
                    var characterList = matchCharacter.LeagueUserID == set.Player1ID.Value ? player1Characters : player2Characters;
                    characterList.Add(matchCharacter.CharacterID);
                }
            }
            else
            {
                GetLastUsedCharacters(lastSetP1, player1Characters, set.Player1ID.Value);
                GetLastUsedCharacters(lastSetP2, player2Characters, set.Player2ID.Value);
            }

            FillMissingCharacterSlots(set, player1Characters);
            FillMissingCharacterSlots(set, player2Characters);

            var matchID = match?.ID ?? 0;
            var player1Score = match?.Player1Score ?? 0;
            var player2Score = match?.Player2Score ?? 0;
            var stage = match?.StageID ?? NotSelectedValue;

            return new MatchFormViewModel(index, set, matchID, player1Score, player2Score, player1Characters, player2Characters, stage);
        }

        private static void GetLastUsedCharacters(Set lastSet, ICollection<int> playerCharacters, int playerID)
        {
            if(lastSet != null)
            {
                Debug.Assert(lastSet.IsComplete, "Only send completed sets to match form to get last used characters.");
                foreach(var matchCharacter in lastSet.Matches.First().MatchCharacters)
                {
                    if(matchCharacter.LeagueUserID == playerID)
                    {
                        playerCharacters.Add(matchCharacter.CharacterID);
                    }
                }
            }
        }

        private static void FillMissingCharacterSlots(Set set, ICollection<int> playerCharacters)
        {
            while(playerCharacters.Count < set.League.Game.CharactersPerMatch)
            {
                playerCharacters.Add(NotSelectedValue);
            }
        }
    }
}