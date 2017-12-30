using System.Collections.Generic;

namespace Climb.Core
{
    public class EloTieBreak : TieBreakAttempt
    {
        public override int GetUserScore(IList<SeasonStanding> standingData, SeasonStanding current)
        {
            return current.elo;
        }
    }
}