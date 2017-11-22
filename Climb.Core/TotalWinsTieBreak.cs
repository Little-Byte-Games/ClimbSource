using System.Collections.Generic;

namespace Climb.Core
{
    public class TotalWinsTieBreak : TieBreakAttempt
    {
        public override int GetUserScore(IList<SeasonStanding> standingData, SeasonStanding current)
        {
            return current.wins;
        }
    }
}