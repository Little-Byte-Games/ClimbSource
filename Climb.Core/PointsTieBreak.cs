using System.Collections.Generic;

namespace Climb.Core
{
    public class PointsTieBreak : TieBreakAttempt
    {
        public override int GetUserScore(IList<SeasonStanding> standingData, SeasonStanding current)
        {
            return current.points;
        }
    }
}