using System;
using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services
{
    public interface ISeasonService
    {
        Task<Season> Create(League league, DateTime? startDate = null);
        Task<LeagueUserSeason> Join(Season season, int leagueUserID);
        Task JoinAll(Season season);
        Task Start(Season season);
        Task UpdateStandings(int seasonID);
        Task End(int seasonID);
        Task CreateTournament(int seasonID);
    }
}