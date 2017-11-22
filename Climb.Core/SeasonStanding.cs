using System;
using System.Collections.Generic;

namespace Climb.Core
{
    public class SeasonStanding : IComparable<SeasonStanding>
    {
        public readonly int leagueUserID;
        public readonly int elo;
        private readonly Dictionary<int, int> beatenOpponents = new Dictionary<int, int>();
        public int wins;
        public int losses;
        public decimal tieBreaker;

        public SeasonStanding(int leagueUserID, int elo)
        {
            this.leagueUserID = leagueUserID;
            this.elo = elo;
        }

        public int CompareTo(SeasonStanding other)
        {
            int seasonPointsCompare = GetSeasonPoints().CompareTo(other.GetSeasonPoints());
            return seasonPointsCompare != 0 ? seasonPointsCompare : tieBreaker.CompareTo(other.tieBreaker);
        }

        public void BeatOpponent(int opponentID)
        {
            if (beatenOpponents.ContainsKey(opponentID))
            {
                beatenOpponents[opponentID]++;
            }
            else
            {
                beatenOpponents[opponentID] = 1;
            }
        }

        public int GetWinsAgainstOpponent(int opponentID)
        {
            return beatenOpponents.TryGetValue(opponentID, out var timesWon) ? timesWon : 0;
        }

        public int GetSeasonPoints()
        {
            const int winningPoints = 2;
            const int losingPoints = 1;

            return wins * winningPoints + losses * losingPoints;
        }
    }
}