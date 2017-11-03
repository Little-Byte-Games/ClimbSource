using System;
using System.Collections.Generic;
using Climb.Models;
using System.Linq;
using Set = Climb.Models.Set;

namespace Climb.Controllers
{
    public partial class SeasonsController
    {
        public class SeasonStartViewModel
        {
            public readonly Season season;
            public readonly SortedDictionary<DateTime, HashSet<Set>> sets;

            public SeasonStartViewModel(Season season)
            {
                this.season = season;

                sets = new SortedDictionary<DateTime, HashSet<Set>>();
                
                foreach (var set in season.Sets)
                {
                    //set.Player1 = season.Participants.FirstOrDefault(p => p.ID == set.Player1ID);
                    //set.Player2 = season.Participants.FirstOrDefault(p => p.ID == set.Player2ID);
                    if(!sets.TryGetValue(set.DueDate, out var round))
                    {
                        round = new HashSet<Set>();
                        sets.Add(set.DueDate, round);
                    }
                    round.Add(set);
                }
            }
        }
    }
}
