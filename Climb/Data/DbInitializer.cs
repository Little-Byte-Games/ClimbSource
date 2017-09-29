﻿using System;
using System.Linq;
using Climb.Models;

namespace Climb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ClimbContext context)
        {
            context.Database.EnsureDeleted();
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
                new Character{Name = "Mario", Game = games[0]}, 
                new Character{Name = "Sonic", Game = games[0]}, 
                new Character{Name = "Link", Game = games[0]}, 

                new Character{Name = "Ryu", Game = games[1]},
                new Character{Name = "Ken", Game = games[1]},
                new Character{Name = "Gief", Game = games[1]},
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
             
            foreach(var league in leagues)
            {
                var season = new Season { League = league, Index = 0, StartDate = DateTime.Today.AddDays(7) };
                context.Season.Add(season);
                context.SaveChanges();

                foreach (var member in league.Members)
                {
                    var participant = new LeagueUserSeason { Season = season, LeagueUser = member };
                    context.LeagueUserSeason.Add(participant);
                }
            }
            context.SaveChanges();
        }
    }
}