using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services
{
    public interface ILeagueService
    {
        Task JoinLeague(User user, League league);
        Task<HashSet<RankSnapshot>> TakeSnapshot(League league);
    }
}