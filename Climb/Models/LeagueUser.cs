using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Climb.Models
{
    public class LeagueUser : IComparable<LeagueUser>, IProfile
    {
        public enum Trend
        {
            BigUp,
            SmallUp,
            None,
            SmallDown,
            BigDown,
        }

        private const int StartingPoints = 2000;
        public const string MissingPic = @"https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcQpAB85y5CpzuT3QLLh7dVkrSIWRwQ8gANIH2OHeqph6k2Caa2UFnHgKbwu";

        [HiddenInput]
        public int ID { get; set; }
        public int UserID { get; set; }
        public int LeagueID { get; set; }
        [Required]
        [Display(Name = "Display Name")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [RegularExpression(@"((\S+)( *))*", ErrorMessage = "{0} needs at least 1 non-whitespace character.")]
        public string DisplayName { get; set; }
        public int Points { get; set; } = StartingPoints;
        public string ProfilePicKey { get; set; }
        public bool HasLeft { get; set; }
        public int Rank { get; set; }
        public bool IsNew { get; set; } = true;
        [StringLength(20, MinimumLength = 1, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [RegularExpression(@"((\S+)( *))*", ErrorMessage = "SlackUsername needs at least 1 non-whitespace character.")]
        public string SlackUsername { get; set; }
        [StringLength(20, MinimumLength = 1, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [RegularExpression(@"((\S+)( *))*", ErrorMessage = "ChallongeUsername needs at least 1 non-whitespace character.")]
        public string ChallongeUsername { get; set; }

        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public League League { get; set; }
        [JsonIgnore]
        public HashSet<LeagueUserSeason> Seasons { get; set; }
        [JsonIgnore]
        public HashSet<Set> P1Sets { get; set; }
        [JsonIgnore]
        public HashSet<Set> P2Sets { get; set; }
        [JsonIgnore]
        public HashSet<RankSnapshot> RankSnapshots { get; set; }

        [JsonIgnore]
        public string GetSlackName => string.IsNullOrWhiteSpace(SlackUsername) ? DisplayName : $"<@{SlackUsername}>({DisplayName})";

        public int CompareTo(LeagueUser other)
        {
            return other.Points.CompareTo(Points);
        }

        public int GetRankTrendDelta()
        {
            const int trendMonths = 1;
            const int requiredMonthsBeforeTrend = 4;

            var trendStart = DateTime.Today.AddMonths(-trendMonths);
            var trendSnapshots = RankSnapshots
                .Where(rs => rs.CreatedDate >= trendStart)
                .OrderByDescending(rs => rs.CreatedDate)
                .ToList();
            if (trendSnapshots.Count < requiredMonthsBeforeTrend)
            {
                return 0;
            }

            var startSnapshot = trendSnapshots.Last();
            var endSnapshot = trendSnapshots.First();

            return endSnapshot.Rank - startSnapshot.Rank;
        }

        public Trend GetRankTrend()
        {
            var rankDelta = GetRankTrendDelta();
            const int bigLimit = 2;
            Trend rankTrend;
            if (rankDelta >= bigLimit)
            {
                rankTrend = Trend.BigDown;
            }
            else if (rankDelta > 0)
            {
                rankTrend = Trend.SmallUp;
            }
            else if (rankDelta <= -bigLimit)
            {
                rankTrend = Trend.BigDown;
            }
            else if (rankDelta < 0)
            {
                rankTrend = Trend.SmallDown;
            }
            else
            {
                rankTrend = Trend.None;
            }

            return rankTrend;
        }
    }
}
