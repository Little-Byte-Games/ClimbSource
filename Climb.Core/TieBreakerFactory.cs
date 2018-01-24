namespace Climb.Core
{
    public static class TieBreakerFactory
    {
        public static TieBreaker Create()
        {
            var tieBreaker = new TieBreaker();
            tieBreaker.AddTieBreakAttempt(new TiedWinsTieBreak());
            tieBreaker.AddTieBreakAttempt(new TotalWinsTieBreak());
            tieBreaker.AddTieBreakAttempt(new PointsTieBreak());
            return tieBreaker;
        }
    }
}