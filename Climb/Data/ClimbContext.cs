using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        }
    }
}