using Climb.Core;
using Climb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Climb.Controllers.SeasonsController;
using Set = Climb.Models.Set;

namespace Climb.Services
{
    public class SeasonService : ISeasonService
    {
        private readonly ClimbContext context;

        public SeasonService(ClimbContext context)
        {
            this.context = context;
        }

        public async Task<Season> Create(League league, DateTime? startDate = null)
        {
            var season = new Season
            {
                Index = league.Seasons.Count,
                LeagueID = league.ID,
                StartDate = startDate ?? DateTime.UtcNow.AddDays(7)
            };
            await context.AddAsync(season);
            await context.SaveChangesAsync();
            return season;
        }

        [Obsolete("All participants are added through season creation currently.")]
        public async Task Join(Season season, LeagueUser leagueUser)
        {
            var leagueUserSeason = new LeagueUserSeason { Season = season, LeagueUser = leagueUser };
            await context.AddAsync(leagueUserSeason);

            season.Participants.Add(leagueUserSeason);
            context.Update(season);
            await context.SaveChangesAsync();
        }

        public async Task Start(Season season, IEnumerable<Participant> participants)
        {
            if (season.Sets != null)
            {
                context.RemoveRange(season.Sets);
            }
            season.Sets = new HashSet<Set>();
            HashSet<int>[] divisions = FormatDivisionData(participants);

            var rounds = ScheduleGenerator.GenerateWithDivisions(divisions, season.StartDate);
            foreach (var round in rounds.SelectMany(r => r))
            {
                AddRound(season, round);
            }

            context.UpdateRange(season.Sets);
            context.Update(season);
            await context.SaveChangesAsync();
        }

        private static void AddRound(Season season, Round round)
        {
            foreach (var setData in round.sets)
            {
                var byePlayer = 0;

                int? player1 = setData.player1;
                if (player1 == ScheduleGenerator.Bye)
                {
                    player1 = null;
                    byePlayer = 1;
                }
                int? player2 = setData.player2;
                if (player2 == ScheduleGenerator.Bye)
                {
                    player2 = null;
                    byePlayer = 2;
                }

                int? p1Score = null;
                int? p2Score = null;
                if (byePlayer != 0)
                {
                    p1Score = byePlayer == 1 ? -1 : 0;
                    p2Score = byePlayer == 2 ? -1 : 0;
                }

                var set = new Set
                {
                    SeasonID = season.ID,
                    LeagueID = season.LeagueID,
                    DueDate = round.dueDate,
                    Player1ID = player1,
                    Player2ID = player2,
                    Player1Score = p1Score,
                    Player2Score = p2Score,
                };

                season.Sets.Add(set);
            }
        }

        private static HashSet<int>[] FormatDivisionData(IEnumerable<Participant> participants)
        {
            var divisionCount = participants.Max(p => p.DivisionIndex) + 1;
            var divisions = new HashSet<int>[divisionCount];
            for (int i = 0; i < divisions.Length; i++)
            {
                divisions[i] = new HashSet<int>();
            }

            foreach (var participant in participants.Where(p => p.DivisionIndex >= 0))
            {
                divisions[participant.DivisionIndex].Add(participant.UserID);
            }

            return divisions;
        }
    }
}
