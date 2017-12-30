using Climb.Core;
using Climb.Core.Challonge;
using Climb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Climb.Consts;
using Set = Climb.Models.Set;

namespace Climb.Services
{
    public class SeasonService : ISeasonService
    {
        private readonly ClimbContext context;
        private readonly string challongeKey;

        public SeasonService(ClimbContext context, IConfiguration configuration)
        {
            this.context = context;

            challongeKey = configuration.GetSection("Challonge")["Key"];
        }

        public async Task<Season> Create(League league, DateTime? startDate = null)
        {
            var season = new Season {
                Index = league.Seasons.Count,
                LeagueID = league.ID,
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
            var rounds = ScheduleGenerator.Generate(participants.Count - 1, participants, season.StartDate, true);
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

            foreach(var participant in season.Participants)
            {
                UpdatePotentialMaxPoints(participant);
            }

            await context.SaveChangesAsync();
        }

        public async Task UpdateStandings(int seasonID)
        {
            var season = await context.Season
                .Include(s => s.Participants).ThenInclude(lus => lus.LeagueUser)
                .Include(s => s.Sets)
                .SingleOrDefaultAsync(s => s.ID == seasonID);

            var points = new Dictionary<int, SeasonStanding>();
            foreach (var participant in season.Participants)
            {
                var leagueUserID = participant.LeagueUserID;
                points.Add(leagueUserID, new SeasonStanding(leagueUserID, participant.LeagueUser.Elo));
            }

            foreach(var set in season.Sets)
            {
                if(set.IsComplete)
                {
                    Debug.Assert(set.WinnerID != null, "set.WinnerID != null");
                    points[set.WinnerID.Value].wins++;

                    if(!set.IsBye)
                    {
                        Debug.Assert(set.LoserID != null, "set.LoserID != null");
                        points[set.LoserID.Value].losses++;
                        points[set.WinnerID.Value].BeatOpponent(set.LoserID.Value);
                    }
                }
            }

            var tieBreaker = TieBreakerFactory.Create();

            var standingData = points.Values.ToList();
            while(standingData.Count > 0)
            {
                var ties = new List<SeasonStanding>();
                var currentPoints = standingData[0].GetSeasonPoints();
                for (int i = standingData.Count - 1; i >= 0; --i)
                {
                    if(standingData[i].GetSeasonPoints() == currentPoints)
                    {
                        ties.Add(standingData[i]);
                        standingData.RemoveAt(i);
                    }
                }

                if (ties.Count > 0)
                {
                    tieBreaker.Break(ties);
                }
            }

            var sortedPoints = points.OrderByDescending(p => p.Value);

            var placing = 1;
            var lastTieBreaker = -1m;
            foreach(var participant in sortedPoints)
            {
                var leagueUserSeason = season.Participants.First(p => p.LeagueUserID == participant.Key);
                leagueUserSeason.Standing = placing;
                leagueUserSeason.Points = participant.Value.GetSeasonPoints();
                UpdatePotentialMaxPoints(leagueUserSeason);

                if (FeatureToggles.Challonge)
                {
                    await ChallongeController.UpdateBracket(challongeKey, season.ChallongeID, leagueUserSeason.ChallongeID, placing); 
                }

                if(lastTieBreaker != participant.Value.tieBreaker)
                {
                    ++placing;
                    lastTieBreaker = participant.Key;
                }
            }

            context.UpdateRange(season.Participants);
            await context.SaveChangesAsync();
        }

        private void UpdatePotentialMaxPoints(LeagueUserSeason participant)
        {
            var remainingSets = participant.Season.Sets.Count(s => !s.IsComplete && s.IsPlaying(participant.LeagueUserID));
            participant.PotentialMaxPoints = remainingSets * SeasonStanding.WinningPoints + participant.Points;
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

        public async Task CreateTournament(int seasonID)
        {
            var season = await context.Season
                .Include(s => s.League)
                .Include(l => l.Participants).ThenInclude(lus => lus.LeagueUser).ThenInclude(lu => lu.User)
                .SingleOrDefaultAsync(s => s.ID == seasonID);

            var tournament = await ChallongeController.CreateTournament(challongeKey, $"{season.League.Name}: {season.DisplayName}", season.Participants.Select(lus => (lus.LeagueUserID, lus.LeagueUser.ChallongeUsername, lus.LeagueUser.User.Username)));
            season.ChallongeID = tournament.tournamentID;
            season.ChallongeUrl = tournament.tournamentUrl;

            context.UpdateRange(season.Participants);
            foreach(var participant in season.Participants)
            {
                var id = tournament.participantIDs[participant.LeagueUserID];
                participant.ChallongeID = id;
            }

            context.Update(season);
            await context.SaveChangesAsync();
        }
    }
}
