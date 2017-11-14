using System;
using System.Collections.Generic;
using System.Linq;

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

    public class Season
    {
        public int ID { get; set; }
        public int LeagueID { get; set; }
        public int Index { get; set; }
        public DateTime StartDate { get; set; }

        public League League { get; set; }
        public HashSet<LeagueUserSeason> Participants { get; set; }
        public HashSet<Set> Sets { get; set; }
        public HashSet<Division> Divisions { get; set; }

        public string DisplayName => $"Season {Index + 1}";
        public bool IsComplete => Sets != null && Sets.All(s => s.IsComplete);

        public SeasonStatus GetStatus()
        {
            int overdueCount = 0;
            int availableCount = 0;
            int completedCount = 0;

            var now = DateTime.Now;
            foreach(var set in Sets)
            {
                if(set.IsComplete)
                {
                    ++completedCount;
                }
                else if(set.DueDate < now)
                {
                    ++overdueCount;
                }
                else
                {
                    ++availableCount;
                }
            }

            return new SeasonStatus(Sets.Count, overdueCount, availableCount, completedCount);
        }
    }
}
