﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Climb.Models
{
    public class LeagueUser : IComparable<LeagueUser>
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int LeagueID { get; set; }
        public int Elo { get; set; }
        public string ProfilePicKey { get; set; }
        public bool HasLeft { get; set; }
        public int Rank { get; set; }

        public User User { get; set; }
        public League League { get; set; }
        public HashSet<LeagueUserSeason> Seasons { get; set; }
        public HashSet<Set> P1Sets { get; set; }
        public HashSet<Set> P2Sets { get; set; }
        public HashSet<RankSnapshot> RankSnapshots { get; set; }

        public int CompareTo(LeagueUser other)
        {
            return other.Elo.CompareTo(Elo);
        }

        public string GetProfilePicUrl(IConfiguration configuration)
        {
            var profilePicUrl = @"https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcQpAB85y5CpzuT3QLLh7dVkrSIWRwQ8gANIH2OHeqph6k2Caa2UFnHgKbwu";
            if (ProfilePicKey != null)
            {
                var awsSection = configuration.GetSection("AWS");
                var root = awsSection["RootUrl"];
                var bucket = awsSection["Bucket"];
                profilePicUrl = string.Join("/", root, bucket, ProfilePicKey);
            }
            return profilePicUrl;
        }
    }
}
