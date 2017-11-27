using Climb.Core;
using Climb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Set = Climb.Models.Set;

namespace Climb.Services
{
    public class SetService : ISetService
    {
        private readonly ClimbContext context;
        private readonly ISeasonService seasonService;

        public SetService(ClimbContext context, ISeasonService seasonService)
        {
            this.context = context;
            this.seasonService = seasonService;
        }

        public async Task Put(Set set, IList<Match> matches)
        {
            context.Update(set);

            set.UpdatedDate = DateTime.UtcNow;
            await UpdateMatches(set, matches, context);
            UpdateScore(set);
            UpdateElo(set);
            await UpdateRank(set, context);

            if (!set.IsExhibition)
            {
                Debug.Assert(set.SeasonID != null, "set.SeasonID != null");
                await seasonService.UpdateStandings(set.SeasonID.Value);
            }

            await context.SaveChangesAsync();
        }

        private static async Task UpdateMatches(Set set, IEnumerable<Match> matches, ClimbContext context)
        {
            context.RemoveRange(set.Matches);
            set.Matches.Clear();
            await context.SaveChangesAsync();

            foreach (var match in matches)
            {
                set.Matches.Add(match);
            }
            await context.AddRangeAsync(set.Matches);
        }

        private static void UpdateScore(Set set)
        {
            set.Player1Score = 0;
            set.Player2Score = 0;

            foreach (var match in set.Matches)
            {
                if (match.Player1Score > 0 && match.Player1Score > match.Player2Score)
                {
                    ++set.Player1Score;
                }
                else if (match.Player2Score > 0 && match.Player2Score > match.Player1Score)
                {
                    ++set.Player2Score;
                }
            }
        }

        private static void UpdateElo(Set set)
        {
            const int startingElo = 2000;
            set.Player1.Elo = set.Player1.Elo == 0 ? startingElo : set.Player1.Elo;
            set.Player2.Elo = set.Player2.Elo == 0 ? startingElo : set.Player2.Elo;

            var p1Wins = set.Matches.Count(m => m.Player1Score > m.Player2Score);
            var p2Wins = set.Matches.Count(m => m.Player2Score > m.Player1Score);
            var player1Won = p1Wins >= p2Wins;
            var newElo = PlayerScoreCalculator.CalculateElo(set.Player1.Elo, set.Player2.Elo, player1Won);
            set.Player1.Elo = newElo.Item1;
            set.Player2.Elo = newElo.Item2;
        }

        private static async Task UpdateRank(Set set, ClimbContext context)
        {
            var users = await context.LeagueUser.Where(lu => lu.LeagueID == set.LeagueID).ToListAsync();
            users.Sort();
            var rank = 0;
            var lastElo = -1;
            for (var i = 0; i < users.Count; i++)
            {
                LeagueUser member = users[i];
                if (member.Elo != lastElo)
                {
                    lastElo = member.Elo;
                    rank = i + 1;
                }
                member.Rank = rank;
            }
            context.UpdateRange(users);
        }
    }
}