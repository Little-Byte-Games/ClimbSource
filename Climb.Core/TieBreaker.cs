using System;
using System.Collections.Generic;

namespace Climb.Core
{
    public class TieBreaker
    {
        private readonly List<TieBreakAttempt> tiebreakAttempts = new List<TieBreakAttempt>();

        public void AddTieBreakAttempt(TieBreakAttempt tieBreakAttempt)
        {
            tiebreakAttempts.Add(tieBreakAttempt);
        }

        public void Break(IList<SeasonStanding> standingData)
        {
            var round = tiebreakAttempts.Count;
            foreach(var tiebreakAttempt in tiebreakAttempts)
            {
                var userScores = tiebreakAttempt.Evaluate(standingData);
                userScores.Sort();

                var lastScore = int.MinValue;
                var place = 0;
                foreach(var userScore in userScores)
                {
                    if(userScore.roundPoints != lastScore)
                    {
                        lastScore = userScore.roundPoints;
                        ++place;
                    }

                    var roundScore = (decimal)Math.Pow(place * 2, round);
                    userScore.seasonStanding.tieBreaker += roundScore;
                }

                --round;
            }
        }
    }
}
