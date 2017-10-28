using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Climb.Models
{
    public class Game
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public HashSet<Character> Characters { get; set; }
        public HashSet<Stage> Stages { get; set; }
        public HashSet<League> Leagues { get; set; }

        public async Task<Dictionary<Character, decimal>> GetCharacterUsagePercentagesAsync(ClimbContext context)
        {
            var characterCounts = new Dictionary<Character, int>();
            foreach(var character in Characters)
            {
                    characterCounts.Add(character, 0);
            }

            var matches = await context.Match
                .Include(m => m.Set).ThenInclude(s => s.League).ThenInclude(l => l.Game)
                .Where(m => m.Set.League.GameID == ID && !m.Set.IsBye && m.Set.IsComplete)
                .ToArrayAsync();

            foreach(var match in matches)
            {
                ++characterCounts[match.Player1Character];
                ++characterCounts[match.Player2Character];
            }

            var matchCount = matches.Length * 2m;
            return characterCounts.ToDictionary(characterCount => characterCount.Key, characterCount => characterCount.Value / matchCount);
        }
    }
}