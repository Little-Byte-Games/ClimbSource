using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Climb.Models
{
    public class ClimbContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<RankEvent> RankEvent { get; set; }
        public DbSet<LeagueUser> LeagueUser { get; set; }
        public DbSet<LeagueUserSeason> LeagueUserSeason { get; set; }
        public DbSet<RankSnapshot> RankSnapshot { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach(var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<LeagueUserSeason>()
                .HasKey(lus => new { lus.LeagueUserID, lus.SeasonID });

            modelBuilder.Entity<LeagueUserSeason>()
                .HasOne(lus => lus.LeagueUser)
                .WithMany(lu => lu.Seasons)
                .HasForeignKey(bc => bc.LeagueUserID);

            modelBuilder.Entity<LeagueUserSeason>()
                .HasOne(lus => lus.Season)
                .WithMany(s => s.Participants)
                .HasForeignKey(lus => lus.SeasonID);
        }
    }
}