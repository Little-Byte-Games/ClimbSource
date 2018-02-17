namespace Climb.ViewModels.Sets
{
    public struct MatchScoreInputViewModel
    {
        public readonly int score;
        public readonly int index;
        public readonly int playerNumber;
        public readonly int maxPoints;

        public MatchScoreInputViewModel(int score, int index, int playerNumber, int maxPoints)
        {
            this.score = score;
            this.index = index;
            this.playerNumber = playerNumber;
            this.maxPoints = maxPoints;
        }
    }
}