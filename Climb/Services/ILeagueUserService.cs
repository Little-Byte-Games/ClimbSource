using Climb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climb.Services
{
    public interface ILeagueUserService
    {
        Task<IEnumerable<Character>> GetMostUsedCharacters(int id, int count);
        IEnumerable<Set> GetHistory(LeagueUser leagueUser, int count);
    }
}