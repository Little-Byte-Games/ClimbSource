using Climb.Models;
using Climb.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Climb.Responses.Sets
{
    public struct SetGetResponse
    {
        public struct GameResponse
        {
            public struct CharacterResponse
            {
                public readonly int id;
                public readonly string name;
                public readonly string pic;

                public CharacterResponse(int id, string name, string pic)
                {
                    this.id = id;
                    this.name = name;
                    this.pic = pic;
                }
            }

            public readonly string setRules;
            public readonly bool requiresStage;
            public readonly int charactersPerMatch;
            public readonly int maxMatchPoints;
            public readonly IEnumerable<CharacterResponse> characters;
            public readonly IEnumerable<Stage> stages;

            public GameResponse(Game game, CdnService cdn)
            {
                setRules = game.SetRules;
                requiresStage = game.RequireStage;
                charactersPerMatch = game.CharactersPerMatch;
                maxMatchPoints = game.MaxMatchPoints;
                characters = game.Characters.Select(c => new CharacterResponse(c.ID, c.Name, GetPicUrl(c, cdn)));
                stages = game.Stages;
            }

            private static string GetPicUrl(Character character, CdnService cdn)
            {
                return cdn.GetImageUrl(CdnService.ImageTypes.CharacterPic, character.PicKey);
            }
        }

        public readonly int id;
        public readonly int? seasonID;
        public readonly int leagueID;
        public readonly int gameID;
        public readonly int? player1ID;
        public readonly int? player2ID;
        public readonly string player1;
        public readonly string player2;
        public readonly DateTime dueDate;
        public readonly DateTime? updatedDate;
        public readonly int player1Score;
        public readonly int player2Score;
        public readonly bool isLocked;
        public readonly bool isDeactivated;
        public readonly IEnumerable<Match> matches;
        public readonly GameResponse game;

        public SetGetResponse(Set set, CdnService cdn)
        {
            id = set.ID;
            seasonID = set.SeasonID;
            leagueID = set.LeagueID;
            gameID = set.League.GameID;
            player1ID = set.Player1ID;
            player2ID = set.Player2ID;
            player1 = set.Player1.DisplayName;
            player2 = set.Player2.DisplayName;
            dueDate = set.DueDate;
            updatedDate = set.UpdatedDate;
            player1Score = set.Player1Score ?? 0;
            player2Score = set.Player2Score ?? 0;
            isLocked = set.IsLocked;
            isDeactivated = set.IsDeactivated;
            matches = set.Matches;

            game = new GameResponse(set.League.Game, cdn);
        }
    }
}