﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Climb.Models
{
    public class Game
    {
        private class CharacterPercentages
        {
            public decimal matches;
            public decimal wins;
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public HashSet<Character> Characters { get; set; }
        public HashSet<Stage> Stages { get; set; }
        public HashSet<League> Leagues { get; set; }

        public async Task<Dictionary<Character, (decimal , decimal)>> GetCharacterUsagePercentagesAsync(ClimbContext context)
        {
            var characterCounts = new Dictionary<Character, CharacterPercentages>();
            foreach(var character in Characters)
            {
                    characterCounts.Add(character, new CharacterPercentages());
            }

            var matches = await context.Match
                .Include(m => m.Set).ThenInclude(s => s.League).ThenInclude(l => l.Game)
                .Where(m => m.Set.League.GameID == ID && !m.Set.IsBye && m.Set.IsComplete)
                .ToArrayAsync();

            foreach(var match in matches)
            {
                if(match.Player1CharacterID == null || match.Player2CharacterID == null)
                {
                    continue;
                }

                ++characterCounts[match.Player1Character].matches;
                ++characterCounts[match.Player2Character].matches;
                characterCounts[match.WinningCharacter].wins += match.IsDitto ? 2 : 1;
            }

            var matchCount = matches.Length * 2m;
            var dictionary = new Dictionary<Character, (decimal, decimal)>();
            foreach(var characterCount in characterCounts)
            {
                var usage = matchCount > 0 ? characterCount.Value.matches / matchCount : 0;
                var win = characterCount.Value.matches > 0 ? characterCount.Value.wins / characterCount.Value.matches : 0;

                dictionary.Add(characterCount.Key, (usage, win));
            }
            return dictionary;
        }
    }
}