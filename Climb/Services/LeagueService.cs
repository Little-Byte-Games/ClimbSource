using Climb.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Climb.Services
{
    public class LeagueService
    {
        private readonly ClimbContext context;

        public LeagueService(ClimbContext context)
        {
            this.context = context;
        }

        public async Task<HashSet<RankSnapshot>> TakeSnapshot(League league)
        {
            var createdDate = DateTime.Now;
            var rankSnapshots = new HashSet<RankSnapshot>();
            foreach(var member in league.Members)
            {
                RankSnapshot lastSnapshot = null;
                if(member.RankSnapshots?.Count > 0)
                {
                    lastSnapshot = member.RankSnapshots?.MaxBy(rs => rs.CreatedDate);
                }
                var rankDelta = member.Rank - (lastSnapshot?.Rank ?? 0);
                var eloDelta = member.Elo - (lastSnapshot?.Elo ?? 0);
                var rankSnapshot = new RankSnapshot
                {
                    LeagueUser = member,
                    Rank = member.Rank,
                    DeltaRank = rankDelta,
                    Elo = member.Elo,
                    DeltaElo = eloDelta,
                    CreatedDate = createdDate
                };
                rankSnapshots.Add(rankSnapshot);
            }
            await context.RankSnapshot.AddRangeAsync(rankSnapshots);
            await context.SaveChangesAsync();

            return rankSnapshots;
        }
    }
}