using Microsoft.EntityFrameworkCore;
using System.Linq;
using Climb.Models;

namespace Climb.Models
{
    public class ClimbContext : DbContext
    {
        public ClimbContext(DbContextOptions<ClimbContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<League> League { get; set; }
        public DbSet<Season> Season { get; set; }
        public DbSet<Set> Set { get; set; }
        public DbSet<Match> Match { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach(var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            //foreach(var property in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetProperties()))
            //{
            //    if(property.IsForeignKey() && property.IsNullable)
            //    {
            //        property.
            //    }
            //}
        }

        public DbSet<Climb.Models.RankEvent> RankEvent { get; set; }

        public DbSet<Climb.Models.LeagueUser> LeagueUser { get; set; }
    }
}