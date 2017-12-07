using System.Collections.Generic;
using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services
{
    public interface ILeagueService
    {
        Task<LeagueUser> JoinLeague(User user, League league);
        Task<HashSet<RankSnapshot>> TakeSnapshot(League league);
        Task SendSnapshotUpdate(HashSet<RankSnapshot> snapshots, League league);
    }
}