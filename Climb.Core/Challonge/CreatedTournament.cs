using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Climb.Core.Challonge
{
    public class CreatedTournament
    {
        public readonly int tournamentID;
        public readonly string tournamentUrl;
        public readonly ReadOnlyDictionary<int, int> participantIDs;

        public CreatedTournament(int tournamentID, string tournamentUrl, Dictionary<int, int> participantIDs)
        {
            this.tournamentID = tournamentID;
            this.tournamentUrl = tournamentUrl;
            this.participantIDs = new ReadOnlyDictionary<int, int>(participantIDs);
        }
    }
}
