﻿using Climb.Controllers;
using Climb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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
            Debug.Assert(!set.IsBye, "Can't submit a bye set.");

            context.Update(set);

            if(set.UpdatedDate == null)
            {
                ++set.Player1.SetsPlayed;
                ++set.Player2.SetsPlayed;
            }

            set.UpdatedDate = DateTime.UtcNow;
            await UpdateMatches(set, matches, context);
            UpdateSetScore(set);

            if (!set.IsExhibition)
            {
                Debug.Assert(set.SeasonID != null, "set.SeasonID != null");
                await seasonService.UpdateStandings(set.SeasonID.Value);
            }

            await context.SaveChangesAsync();
        }

        private static async Task UpdateMatches(Set set, IEnumerable<Match> matches, ClimbContext context)
        {
            context.RemoveRange(set.Matches.SelectMany(m => m.MatchCharacters));
            context.RemoveRange(set.Matches);
            set.Matches.Clear();
            await context.SaveChangesAsync();

            foreach (var match in matches)
            {
                set.Matches.Add(match);
            }
            await context.AddRangeAsync(set.Matches);
        }

        private static void UpdateSetScore(Set set)
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

        public string GetSetUrl(Set set, IUrlHelper urlHelper)
        {
            return urlHelper.Action(new UrlActionContext {Controller = "Sets", Action = nameof(SetsController.Fight), Values = new {id = set.ID}, Protocol = "https"});
        }
    }
}