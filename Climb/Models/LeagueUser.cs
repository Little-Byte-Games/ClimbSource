﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Climb.Models
{
    public class LeagueUser : IComparable<LeagueUser>
    {
        public enum Trend
        {
            BigUp,
            SmallUp,
            None,
            SmallDown,
            BigDown,
        }

        public const string MissingPic = @"https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcQpAB85y5CpzuT3QLLh7dVkrSIWRwQ8gANIH2OHeqph6k2Caa2UFnHgKbwu";

        public int ID { get; set; }
        public int UserID { get; set; }
        public int LeagueID { get; set; }
        public int Elo { get; set; }
        public string ProfilePicKey { get; set; }
        public bool HasLeft { get; set; }
        public int Rank { get; set; }

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

        public int CompareTo(LeagueUser other)
        {
            return other.Elo.CompareTo(Elo);
        }

        public int GetRankTrendDelta()
        {
            const int trendMonths = 1;
            var trendStart = DateTime.Today.AddMonths(-trendMonths);
            var trendSnapshots = RankSnapshots.Where(rs => rs.CreatedDate >= trendStart).OrderByDescending(rs => rs.CreatedDate).ToList();
            if (trendSnapshots.Count < 2)
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
