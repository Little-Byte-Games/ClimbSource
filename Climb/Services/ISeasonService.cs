using System;
using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services
{
    public interface ISeasonService
    {
        Task<Season> Create(League league, DateTime? startDate = null);
        Task Join(Season season, LeagueUser leagueUser);
        Task Start(Season season);
    }
}