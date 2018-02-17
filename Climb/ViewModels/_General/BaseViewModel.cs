using Climb.Models;
using System.Collections.Generic;

namespace Climb.ViewModels
{
    public abstract class BaseViewModel
    {
        public readonly User user;
        public readonly HashSet<Season> activeSeasons;

        protected BaseViewModel(User user)
        {
            this.user = user;

            activeSeasons = new HashSet<Season>();
            foreach(var leagueUser in user.LeagueUsers)
            {
                foreach(var leagueUserSeason in leagueUser.Seasons)
                {
                    if(!leagueUserSeason.Season.IsComplete)
                    {
                        activeSeasons.Add(leagueUserSeason.Season);
                    }
                }
            }
        }
    }
}