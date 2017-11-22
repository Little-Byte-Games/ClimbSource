using System.Collections.Generic;
using System.Linq;

namespace Climb.Core
{
    public class TiedWinsTieBreak : TieBreakAttempt
    {
        public override int GetUserScore(IList<SeasonStanding> standingData, SeasonStanding current)
        {
            return standingData.Count(data => current.GetWinsAgainstOpponent(data.leagueUserID) > 0);
        }
    }
}