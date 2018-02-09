using Climb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Climb.Services;

namespace Climb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ClimbContext context, IHostingEnvironment environment, ILeagueService leagueService, bool delete)
        {
            if(delete)
            {
                context.Database.EnsureDeleted();
            }

            if(context.Game.Any())
            {
                return;
            }

            LoadFromFile(context, context.Game, "Games");
            LoadFromFile(context, context.Character, "Characters");
            LoadFromFile(context, context.Stage, "Stages");

            if(!environment.IsProduction())
            {
                var users = CreateDevUsers(context);
                CreateDevLeagues(context, users, leagueService);
            }
        }

        private static void LoadFromFile<T>(DbContext context, DbSet<T> set, string filePath) where T : class
        {
            var data = File.ReadAllText($@".\Data\SeedData\{filePath}.json");
            var models = JsonConvert.DeserializeObject<List<T>>(data);
            set.AddRange(models);
            context.SaveChanges();
        }

        private static User[] CreateDevUsers(ClimbContext context)
        {
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
            return users;
        }

        private static void CreateDevLeagues(ClimbContext context, IReadOnlyList<User> users, ILeagueService leagueService)
        {
            var leagues = new[]
            {
                new League {Admin = users[0], Name = "Popcap Smash", GameID = 1},
                new League {Admin = users[1], Name = "Local SFV", GameID = 2},
                new League {Admin = users[2], Name = "Seattle Fighterz", GameID = 3},
            };
            context.League.AddRange(leagues);
            context.SaveChanges();

            foreach(var league in leagues)
            {
                foreach(var user in users)
                {
                    leagueService.JoinLeague(user, league).Wait();
                }
            }
            context.SaveChanges();
        }
    }
}