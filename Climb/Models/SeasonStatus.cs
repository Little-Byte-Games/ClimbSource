namespace Climb.Models
{
    public class SeasonStatus
    {
        public readonly int totalSetCount;
        public readonly int overdueCount;
        public readonly int availableCount;
        public readonly int completedCount;

        public SeasonStatus(int totalSetCount, int overdueCount, int availableCount, int completedCount)
        {
            this.totalSetCount = totalSetCount;
            this.overdueCount = overdueCount;
            this.availableCount = availableCount;
            this.completedCount = completedCount;
        }
    }
}