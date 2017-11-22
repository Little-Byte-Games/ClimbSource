using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Climb.Core.Tests
{
    [TestClass]
    public class TieBreakerTests
    {
        [TestMethod]
        public void FirstRoundWinnerCantBeOvertaken()
        {
            var seasonStandings = new List<SeasonStanding>();
            var playerA = new SeasonStanding(0, 1000);
            playerA.BeatOpponent(1);
            playerA.losses += 4;
            seasonStandings.Add(playerA);
            var playerB = new SeasonStanding(1, 2000);
            playerA.BeatOpponent(2);
            playerA.BeatOpponent(3);
            seasonStandings.Add(playerB);
            var playerC = new SeasonStanding(2, 2100);
            playerA.BeatOpponent(3);
            playerA.BeatOpponent(4);
            seasonStandings.Add(playerC);

            var tieBreaker = TieBreakerFactory.Create();
            tieBreaker.Break(seasonStandings);

            Assert.IsTrue(playerA.tieBreaker > playerB.tieBreaker, $"{playerA.tieBreaker} vs {playerB.tieBreaker}");
            Assert.IsTrue(playerA.tieBreaker > playerC.tieBreaker, $"{playerA.tieBreaker} vs {playerC.tieBreaker}");
        }
    }
}
