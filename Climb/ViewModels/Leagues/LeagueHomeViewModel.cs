﻿using Climb.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Climb.ViewModels
{
    public class LeagueHomeViewModel : BaseViewModel
    {
        public readonly League league;
        public readonly IConfiguration configuration;
        public readonly IEnumerable<Set> recentSets;
        public readonly LeagueUser king;
        public readonly IEnumerable<LeagueUser> nonkingMembers;
        public readonly IEnumerable<LeagueUser> newMembers;
        public readonly string reign;
        public readonly bool canCreateSeason;
        public readonly LeagueUser viewingLeagueUser;
        public readonly IEnumerable<Set> overdueSets;
        public readonly IEnumerable<Set> availableSets;

        public LeagueHomeViewModel(User user, League league, IConfiguration configuration, IEnumerable<Set> recentSets)
            : base(user)
        {
            this.league = league;
            this.configuration = configuration;
            this.recentSets = recentSets;
            king = league.KingID != null ? league.Members.FirstOrDefault(lu => lu.ID == league.KingID) : null;
            nonkingMembers = league.Members.Where(lu => lu.ID != league.KingID && !lu.IsNew && !lu.HasLeft).OrderBy(lu => lu.Rank);
            newMembers = league.Members.Where(lu => lu.IsNew && !lu.HasLeft);
            viewingLeagueUser = league.Members.FirstOrDefault(lu => lu.UserID == user.ID);

#if DEBUG
            canCreateSeason = true;
#else
            canCreateSeason = league.AdminID == user.ID;
#endif

            var weeks = league.GetKingReignWeeks();
            if(weeks > 0)
            {
                reign = $"reign of {weeks} week{(weeks > 1 ? "s" : "")} began on {league.KingReignStart.ToShortDateString()}";
            }
            else
            {
                reign = $"new reign started on {league.KingReignStart.ToShortDateString()}";
            }

            if(league.CurrentSeason != null && viewingLeagueUser != null)
            {
                overdueSets = league.CurrentSeason.GetOverdueSets().Where(s => s.IsPlaying(viewingLeagueUser.ID));
                availableSets = league.CurrentSeason.GetAvailableSets().Where(s => s.IsPlaying(viewingLeagueUser.ID));
            }
        }
    }
}