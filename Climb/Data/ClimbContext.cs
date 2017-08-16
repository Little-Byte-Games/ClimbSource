using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Climb.Models;

namespace Climb.Models
{
    public class ClimbContext : DbContext
    {
        public ClimbContext (DbContextOptions<ClimbContext> options)
            : base(options)
        {
        }

        public DbSet<Climb.Models.User> User { get; set; }

        public DbSet<Climb.Models.Game> Game { get; set; }

        public DbSet<Climb.Models.League> League { get; set; }

        public DbSet<Climb.Models.Season> Season { get; set; }
    }
}
