using Climb.Core;
using Climb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Set = Climb.Models.Set;

namespace Climb.Services
{
    public class LeagueService : ILeagueService
    {
        private readonly ClimbContext context;
        private readonly string apiKey;

        public LeagueService(ClimbContext context, IConfiguration configuration)
        {
            this.context = context;
            apiKey = configuration.GetSection("Slack")["Key"];
        }

        public async Task<LeagueUser> JoinLeague(User user, League league)
        {
            var leagueUser = await context.LeagueUser.SingleOrDefaultAsync(u => u.LeagueID == league.ID && u.UserID == user.ID);
            if (leagueUser != null)
            {
                leagueUser.HasLeft = false;
                context.Update(leagueUser);
            }
            else
            {
                leagueUser = new LeagueUser
                {
                    Elo = 2000,
                    League = league,
                    User = user
                };
                await context.AddAsync(leagueUser);
            }

            await context.SaveChangesAsync();
            return leagueUser;
        }

        public async Task<HashSet<RankSnapshot>> TakeSnapshot(League league)
        {
            var memberEloDeltas = new Dictionary<int, int>();
            var unlockedSets = league.Sets.Where(s => !s.IsLocked && s.IsComplete).ToList();
            context.UpdateRange(unlockedSets);
            context.UpdateRange(league.Members);

            CalculateEloDeltas(memberEloDeltas, unlockedSets);
            AssignElo(league, memberEloDeltas);
            UpdateRanks(league.Members.ToList());
            HashSet<RankSnapshot> rankSnapshots = await CreateSnapshots(league);

            UpdateKing(league);

            await context.SaveChangesAsync();

            return rankSnapshots;
        }

        private static void UpdateKing(League league)
        {
            var currentKing = league.Members.First(lu => lu.Rank == 1);
            if(currentKing.ID != league.KingID)
            {
                league.KingID = currentKing.ID;
                league.KingReignStart = DateTime.Now;
            }
        }

        public async Task SendSnapshotUpdate(HashSet<RankSnapshot> rankSnapshots, League league)
        {
            var orderedSnapshots = rankSnapshots.OrderBy(lu => lu.Rank);
            var message = new StringBuilder();
            message.AppendLine($"*{league.Name} PR*");
            foreach (var snapshot in orderedSnapshots)
            {
                message.AppendLine($"{snapshot.Rank} [{snapshot.DisplayDeltaRank}] {snapshot.LeagueUser.GetSlackName}");
            }
            await SlackController.SendGroupMessage(apiKey, message.ToString());
        }

        public async Task SendSetReminders(League league)
        {
            var currentSeason = league.CurrentSeason;
            if(currentSeason == null)
            {
                return;
            }

            var nextSets = new HashSet<Set>();
            foreach(var set in currentSeason.Sets.Where(s => !s.IsComplete))
            {
                if(!nextSets.Any(s => s.IsPlaying(set.Player1ID) || s.IsPlaying(set.Player2ID)))
                {
                    nextSets.Add(set);
                }
            }

            var message = new StringBuilder();
            message.AppendLine($"*{currentSeason.League.Name}:{currentSeason.DisplayName} Sets*");
            foreach (var set in nextSets)
            {
                message.AppendLine($"{set.Player1.GetSlackName} v {set.Player2.GetSlackName}");
            }
            await SlackController.SendGroupMessage(apiKey, message.ToString());
        }

        private static void CalculateEloDeltas(IDictionary<int, int> memberEloDeltas, List<Set> unlockedSets)
        {
            foreach (var set in unlockedSets)
            {
                var player1Won = set.WinnerID == set.Player1ID;
                var newElo = PlayerScoreCalculator.CalculateElo(set.Player1.Elo, set.Player2.Elo, player1Won);
                AccumulateEloDelta(memberEloDeltas, set.Player1, newElo.Item1);
                AccumulateEloDelta(memberEloDeltas, set.Player2, newElo.Item2);

                set.IsLocked = true;
            }
        }

        private static void AssignElo(League league, IReadOnlyDictionary<int, int> memberEloDeltas)
        {
            foreach (var member in league.Members)
            {
                if (memberEloDeltas.TryGetValue(member.ID, out var eloDelta))
                {
                    member.Elo += eloDelta;
                }
            }
        }

        private async Task<HashSet<RankSnapshot>> CreateSnapshots(League league)
        {
            var createdDate = DateTime.Now;
            var rankSnapshots = new HashSet<RankSnapshot>();
            foreach (var member in league.Members)
            {
                RankSnapshot lastSnapshot = null;
                if (member.RankSnapshots?.Count > 0)
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
            return rankSnapshots;
        }

        private static void AccumulateEloDelta(IDictionary<int, int> memberEloDeltas, LeagueUser member, int newElo)
        {
            if (!memberEloDeltas.ContainsKey(member.ID))
            {
                memberEloDeltas.Add(member.ID, 0);
            }

            var deltaElo = newElo - member.Elo;
            memberEloDeltas[member.ID] += deltaElo;
        }

        private static void UpdateRanks(List<LeagueUser> members)
        {
            members.Sort();
            var rank = 0;
            var lastElo = -1;
            for (var i = 0; i < members.Count; i++)
            {
                LeagueUser member = members[i];
                if (member.Elo != lastElo)
                {
                    lastElo = member.Elo;
                    rank = i + 1;
                }
                member.Rank = rank;
            }
        }
    }
}