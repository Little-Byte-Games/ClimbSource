using System;
using Climb.Models;

namespace Climb.ViewModels.Matches
{
    public class MatchFormViewModel
    {
        public const int NotSelectedValue = -1;
        public const string NotSelected = "---";

        public readonly int index;
        public readonly int player1Score;
        public readonly int player2Score;
        [Obsolete]
        public readonly int player1Character;
        [Obsolete]
        public readonly int player2Character;
        public readonly int stage;
        public readonly Set set;

        public MatchFormViewModel(int index, Set set, Match match)
        {
            this.index = index;
            this.set = set;
            player1Score = match?.Player1Score ?? 0;
            player2Score = match?.Player2Score ?? 0;
            player1Character = NotSelectedValue;
            player2Character = NotSelectedValue;
            stage = match?.StageID ?? NotSelectedValue;
        }
    }
}