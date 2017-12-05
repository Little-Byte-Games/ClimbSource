using Climb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Climb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ClimbContext context, IHostingEnvironment environment, bool delete)
        {
            if(delete)
            {
                context.Database.EnsureDeleted();
            }
            context.Database.EnsureCreated();

            if(context.Game.Any())
            {
                return;
            }

            LoadFromFile(context, context.Game, "Games");
            LoadFromFile(context, context.Character, "Characters");
            LoadFromFile(context, context.Stage, "Stages");

            if(environment.IsDevelopment())
            {
                var users = CreateDevUsers(context);
                CreateDevLeagues(context, users);
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

        private static void CreateDevLeagues(ClimbContext context, IReadOnlyList<User> users)
        {
            var leagues = new[]
            {
                new League {Admin = users[0], Name = "Popcap Smash", GameID = 0},
                new League {Admin = users[2], Name = "Local SFV", GameID = 1},
            };
            context.League.AddRange(leagues);
            context.SaveChanges();

            foreach(var league in leagues)
            {
                foreach(var user in users)
                {
                    var leagueUser = new LeagueUser {User = user, League = league};
                    context.LeagueUser.Add(leagueUser);
                }
            }
            context.SaveChanges();
        }
    }
}