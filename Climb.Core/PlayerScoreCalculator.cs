using System;

namespace Climb.Core
{
    public static class PlayerScoreCalculator
    {
        public static (int, int) CalculateElo(int player1Elo, int player2Elo, bool player1Won, int kFactor = 32)
        {
            var transformedP1 = GetTransformedScore(player1Elo);
            var transformedP2 = GetTransformedScore(player2Elo);

            var expectedP1 = transformedP1 / (transformedP1 + transformedP2);
            var expectedP2 = transformedP2 / (transformedP1 + transformedP2);

            var scoreP1 = player1Won ? 1 : 0;
            var scoreP2 = !player1Won ? 1 : 0;

            var eloP1 = player1Elo + kFactor * (scoreP1 - expectedP1);
            var eloP2 = player2Elo + kFactor * (scoreP2 - expectedP2);

            return ((int)eloP1, (int)eloP2);
        }

        private static double GetTransformedScore(int winnerScore)
        {
            return Math.Pow(10, (double)winnerScore / 400);
        }
    }
}
