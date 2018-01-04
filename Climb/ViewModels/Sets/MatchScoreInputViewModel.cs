namespace Climb.ViewModels.Sets
{
    public struct MatchScoreInputViewModel
    {
        public readonly int score;
        public readonly int index;
        public readonly int playerNumber;

        public MatchScoreInputViewModel(int score, int index, int playerNumber)
        {
            this.score = score;
            this.index = index;
            this.playerNumber = playerNumber;
        }
    }
}