using Climb.Models;
using System.Linq;

namespace Climb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ClimbContext context, bool delete)
        {
            if (delete)
            {
                context.Database.EnsureDeleted();
            }
            context.Database.EnsureCreated();

            if(context.User.Any())
            {
                return;
            }

            var users = new[]
            {
                new User {Username = "steve"},
                new User {Username = "sam"},
                new User {Username = "alex"},
                new User {Username = "aaron"},
                new User {Username = "roger"},
                new User {Username = "nick"},
                new User {Username = "matt"},
                new User {Username = "liam"},
            };
            context.User.AddRange(users);
            context.SaveChanges();

            var games = new[]
            {
                new Game{Name = "Smash Bros 4"},
                new Game{Name = "Street Fighter V"},
            };
            context.Game.AddRange(games);
            context.SaveChanges();

            var characters = new[]
            {
                new Character{Name = "Mario", Game = games[0], PicKey = "stock_90_mario_01.png"}, 
                new Character{Name = "Sonic", Game = games[0], PicKey = "stock_90_sonic_01.png"},
                new Character{Name = "Link", Game = games[0], PicKey = "stock_90_link_01.png"},

                //https://game.capcom.com/cfn/sfv/character
                new Character{Name = "Ryu", Game = games[1], PicKey = "ryu.png"},
                new Character{Name = "Ken", Game = games[1], PicKey = "ken.png"},
                new Character{Name = "Gief", Game = games[1], PicKey = "zgf.png"},
            };
            context.Character.AddRange(characters);
            context.SaveChanges();

            var stages = new[]
            {
                new Stage {Name = "Final Destination", Game = games[0]},
                new Stage {Name = "Smashville", Game = games[0]},
                new Stage {Name = "Town & City", Game = games[0]},
                new Stage {Name = "Lylat Cruise", Game = games[0]},
                new Stage {Name = "Dreamland", Game = games[0]},
                new Stage {Name = "Battlefield", Game = games[0]},

                new Stage {Name = "Suzaku Castle", Game = games[1]},
                new Stage {Name = "English Manor", Game = games[1]},
                new Stage {Name = "Forgotten Waterfall", Game = games[1]},
            };
            context.Stage.AddRange(stages);
            context.SaveChanges();

            var leagues = new[]
            {
                new League{Admin = users[0], Name = "Popcap Smash", Game = games[0]},
                new League{Admin = users[2], Name = "Local SFV", Game = games[1]},
            };
            context.League.AddRange(leagues);
            context.SaveChanges();

            foreach(var league in leagues)
            {
                foreach (var user in users)
                {
                    var leagueUser = new LeagueUser { User = user, League = league };
                    context.LeagueUser.Add(leagueUser);
                }
            }
            context.SaveChanges();
        }
    }
}