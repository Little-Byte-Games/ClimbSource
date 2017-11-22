using System;
using System.Collections.Generic;

namespace Climb.Core
{
    public class TieBreakerScore : IComparable<TieBreakerScore>
    {
        public readonly SeasonStanding seasonStanding;
        public readonly int roundPoints;

        public TieBreakerScore(SeasonStanding seasonStanding, int roundPoints)
        {
            this.seasonStanding = seasonStanding;
            this.roundPoints = roundPoints;
        }

        public int CompareTo(TieBreakerScore other) => roundPoints.CompareTo(other.roundPoints);
    }

    public abstract class TieBreakAttempt
    {
        public abstract int GetUserScore(IList<SeasonStanding> standingData, SeasonStanding current);

        public List<TieBreakerScore> Evaluate(IList<SeasonStanding> standingData)
        {
            var scores = new List<TieBreakerScore>();

            foreach(var data in standingData)
            {
                var score = GetUserScore(standingData, data);
                scores.Add(new TieBreakerScore(data, score));
            }

            return scores;
        }
    }
}