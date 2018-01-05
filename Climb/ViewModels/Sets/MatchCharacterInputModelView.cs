using Climb.Models;
using System.Collections.Generic;

namespace Climb.ViewModels.Sets
{
    public class MatchCharacterInputModelView
    {
        public readonly int index;
        public readonly int characterID;
        public readonly List<Character> characters;
        public readonly int matchIndex;
        public readonly int leagueUserID;

        public MatchCharacterInputModelView(int index, int characterID, List<Character> characters, int matchIndex, int leagueUserID)
        {
            this.index = index;
            this.characterID = characterID;
            this.characters = characters;
            this.matchIndex = matchIndex;
            this.leagueUserID = leagueUserID;
        }
    }
}