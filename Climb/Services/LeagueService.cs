using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Climb.Models;
using Microsoft.EntityFrameworkCore;

namespace Climb.Services
{
    public class LeagueService
    {
        public readonly DbSet<League> leagues;

        public LeagueService(DbSet<League> leagues)
        {
            this.leagues = leagues;
        }


    }
}
