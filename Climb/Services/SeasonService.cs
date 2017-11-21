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
    public class SeasonService : ISeasonService
    {
        private readonly ClimbContext context;

        public SeasonService(ClimbContext context)
        {
            this.context = context;
        }

        public async Task<Season> Create(League league, DateTime? startDate = null)
        {
            var season = new Season {
                Index = league.Seasons.Count,
                LeagueID = league.ID,
                //Participants = new HashSet<LeagueUserSeason>(),
                StartDate = startDate ?? DateTime.UtcNow.AddDays(7)
            };
            await context.AddAsync(season);
            await context.SaveChangesAsync();
            return season;
        }

        public async Task Join(Season season, LeagueUser leagueUser)
        {
            var leagueUserSeason = new LeagueUserSeason { Season = season, LeagueUser = leagueUser };
            await context.AddAsync(leagueUserSeason);

            season.Participants.Add(leagueUserSeason);
            context.Update(season);
            await context.SaveChangesAsync();
        }

        public async Task JoinAll(Season season)
        {
            season.Participants = new HashSet<LeagueUserSeason>(season.League.Members.Select(m => new LeagueUserSeason
            {
                SeasonID = season.ID,
                LeagueUserID = m.ID
            }));
            context.Update(season);
            await context.SaveChangesAsync();
        }

        public async Task Start(Season season)
        {
            if (season.Sets != null)
            {
                context.RemoveRange(season.Sets);
            }
            season.Sets = new HashSet<Set>();

            var participants = season.Participants.Select(lus => lus.LeagueUser.ID).ToList();
            var rounds = ScheduleGenerator.Generate(10, participants, season.StartDate, true);
            foreach (var round in rounds)
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

            context.UpdateRange(season.Sets);
            context.Update(season);
            await context.SaveChangesAsync();
        }

        public async Task UpdateStandings(int seasonID)
        {
            const int winningPoints = 2;
            const int losingPoints = 1;

            var season = await context.Season
                .Include(s => s.Participants)
                .Include(s => s.Sets)
                .SingleOrDefaultAsync(s => s.ID == seasonID);

            var points = new Dictionary<int, int>();
            foreach (var participant in season.Participants)
            {
                points.Add(participant.LeagueUserID, 0);
            }

            foreach(var set in season.Sets)
            {
                if(set.IsComplete)
                {
                    Debug.Assert(set.WinnerID != null, "set.WinnerID != null");
                    points[set.WinnerID.Value] += winningPoints;

                    if(!set.IsBye)
                    {
                        Debug.Assert(set.LoserID != null, "set.LoserID != null");
                        points[set.LoserID.Value] += losingPoints;
                    }
                }
            }
            var sortedPoints = points.OrderByDescending(e => e.Value);

            // TODO: Tie breakers.

            var placing = 1;
            var lastPoints = -1;
            foreach(var participant in sortedPoints)
            {
                var leagueUserSeason = season.Participants.First(p => p.LeagueUserID == participant.Key);
                leagueUserSeason.Standing = placing;
                leagueUserSeason.Points = participant.Value;
                if(lastPoints != participant.Value)
                {
                    ++placing;
                    lastPoints = participant.Value;
                }
            }

            context.UpdateRange(season.Participants);
            await context.SaveChangesAsync();
        }

        public async Task End(int seasonID)
        {
            var season = await context.Season
                .Include(s => s.Sets)
                .SingleOrDefaultAsync(s => s.ID == seasonID);

            foreach(var set in season.Sets)
            {
                if(!set.IsComplete)
                {
                    context.Remove(set);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
