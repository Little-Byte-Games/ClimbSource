using Climb.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Climb.ViewModels
{
    public class LeaguePowerRankViewModel : BaseViewModel
    {
        public readonly League league;
        public readonly IConfiguration configuration;
        public readonly IEnumerable<Set> recentSets;
        public readonly Season selectedSeason;
        public readonly LeagueUser king;
        public readonly IEnumerable<LeagueUser> nonkingMembers;
        public readonly IEnumerable<LeagueUser> newMembers;
        public readonly string reign;
        public readonly LeagueUser viewingLeagueUser;
        public readonly bool isAdmin;

        public LeaguePowerRankViewModel(User user, League league, IConfiguration configuration)
            : base(user)
        {
            this.league = league;
            this.configuration = configuration;
            king = league.KingID != null ? league.Members.FirstOrDefault(lu => lu.ID == league.KingID) : null;
            nonkingMembers = league.Members.Where(lu => lu.ID != league.KingID && !lu.IsNew && !lu.HasLeft).OrderBy(lu => lu.Rank);
            newMembers = league.Members.Where(lu => lu.IsNew && !lu.HasLeft);
            viewingLeagueUser = league.Members.FirstOrDefault(lu => lu.UserID == user.ID);

#if DEBUG
            isAdmin = true;
#else
            isAdmin = league.AdminID == user.ID;
#endif

            var currentSeason = league.CurrentSeason;
            int? seasonID = null;
            if(currentSeason != null)
            {
                seasonID = currentSeason.ID;
            }
            else if(league.Seasons?.Count > 0)
            {
                seasonID = league.Seasons.Last().ID;
            }

            selectedSeason = seasonID == null ? null : league.Seasons.Single(s => s.ID == seasonID);

            var weeks = league.GetKingReignWeeks();
            if(weeks > 0)
            {
                reign = $"reign of {weeks} week{(weeks > 1 ? "s" : "")} began on {league.KingReignStart.ToShortDateString()}";
            }
            else
            {
                reign = $"new reign started on {league.KingReignStart.ToShortDateString()}";
            }

            recentSets = currentSeason?.Sets.Where(s => s.IsComplete).OrderByDescending(s => s.UpdatedDate);
        }
    }
}