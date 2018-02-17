using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Climb.Extensions;

namespace Climb.Models
{
    public class Season
    {
        public int ID { get; set; }
        public int LeagueID { get; set; }
        public int Index { get; set; }
        public DateTime StartDate { get; set; }
        public int ChallongeID { get; set; }
        public string ChallongeUrl { get; set; }
        public bool IsComplete { get; set; }

        [JsonIgnore]
        public League League { get; set; }
        [JsonIgnore]
        public HashSet<LeagueUserSeason> Participants { get; set; }
        [JsonIgnore]
        public HashSet<Set> Sets { get; set; }

        public string DisplayName => $"Season {(Index + 1).ToRomainNumeral()}";

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

        public IEnumerable<Set> GetOverdueSets()
        {
            return Sets.Where(s => !s.IsComplete && s.DueDate < DateTime.Now);
        }

        public IEnumerable<Set> GetAvailableSets()
        {
            return Sets.Where(s => !s.IsComplete && s.DueDate >= DateTime.Now).OrderBy(s => s.DueDate);
        }
    }
}
