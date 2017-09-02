using NUnit.Framework;

namespace Climb.Core.Tests
{
    [TestFixture]
    public class PlayerScoreCalculatorTests
    {
        [TestCase(2000, 2000, 16)]
        public void CalculateElo_P1Wins_P1GetsPoints(int p1Elo, int p2Elo, int expectedDifference)
        {
            var newScores = PlayerScoreCalculator.CalculateElo(p1Elo, p2Elo, true);

            Assert.AreEqual(p1Elo + expectedDifference, newScores.Item1);
            Assert.AreEqual(p2Elo - expectedDifference, newScores.Item2);
        }
    }
}
