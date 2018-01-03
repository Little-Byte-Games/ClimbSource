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
            var matches = await context.Match
                .Include(m => m.Set)
                .Include(m => m.MatchCharacters)
                .Where(m => (m.Set.Player1ID == id || m.Set.Player2ID == id) && m.Set.IsComplete && !m.Set.IsBye && m.Set.UpdatedDate >= date).Select(m => m).ToArrayAsync();

            foreach (var match in matches)
            {
                var character = match.Set.Player1ID == id ? match.Player1Character : match.Player2Character;

                if(character == null)
                {
                    continue;
                }

                if (characterUsage.ContainsKey(character))
                {
                    ++characterUsage[character];
                }
                else
                {
                    characterUsage.Add(character, 1);
                }
            }

            return characterUsage.OrderBy(x => x.Value).Take(count).Select(x => x.Key);
        }
    }
}
