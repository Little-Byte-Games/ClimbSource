using Climb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Climb.Services
{
    public class LeagueUserService : ILeagueUserService
    {
        private readonly ClimbContext context;

        public LeagueUserService(ClimbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Character>> GetMostUsedCharacters(int id, int count)
        {
            var characterUsage = new Dictionary<Character, int>();

            const int days = 100;
            var date = DateTime.Today.Subtract(TimeSpan.FromDays(days));
            var characters = await context.MatchCharacters
                .Where(mc => mc.LeagueUserID == id && mc.Match.Set.IsComplete && !mc.Match.Set.IsBye && mc.Match.Set.UpdatedDate >= date)
                .Select(mc => mc.Character).ToArrayAsync();

            foreach (var character in characters)
            {
                if (characterUsage.ContainsKey(character))
                {
                    ++characterUsage[character];
                }
                else
                {
                    characterUsage.Add(character, 1);
                }
            }

            return characterUsage.OrderByDescending(x => x.Value).Take(count).Select(x => x.Key);
        }
    }
}
