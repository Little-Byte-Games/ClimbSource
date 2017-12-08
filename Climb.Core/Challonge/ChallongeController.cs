using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Climb.Core.Challonge
{
    public static class ChallongeController
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<CreatedTournament> CreateTournament(string apiKey, string tournamentName, IEnumerable<(int id, string username, string displayName)> participants)
        {
            var tournamentUrl = DateTime.Now.Ticks.ToString();
            HttpResponseMessage response;
            try
            {
                client.Timeout = TimeSpan.FromSeconds(1);
                var url = $"https://api.challonge.com/v1/tournaments.json?api_key={apiKey}";
                var objectMessage = new
                {
                    tournament = new
                    {
                        url = tournamentUrl,
                        name = tournamentName
                    }
                };
                response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(objectMessage), Encoding.UTF8, "application/json"));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Challonge error!\n{exception}");
                throw;
            }

            if(response == null)
            {
                throw new Exception("Null response");
            }

            var content = await response.Content.ReadAsStringAsync();
            if(!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            TournamentCreateResponse createResponse = JsonConvert.DeserializeObject<TournamentCreateResponse>(content);

            var participantIDs = new Dictionary<int, int>();
            foreach(var participant in participants)
            {
                var participantID = await CreateParticipant(apiKey, createResponse.tournament.id, participant.username, participant.displayName);
                participantIDs.Add(participant.id, participantID);
            }

            var createdTournament = new CreatedTournament(createResponse.tournament.id, tournamentUrl, participantIDs);
            return createdTournament;
        }

        private static async Task<int> CreateParticipant(string apiKey, int tournamentID, string username, string displayName)
        {
            HttpResponseMessage response;
            try
            {
                var url = $"https://api.challonge.com/v1/tournaments/{tournamentID}/participants.json?api_key={apiKey}";
                var objectMessage = new
                {
                    participant = new
                    {
                        name = displayName,
                        challonge_username = username,
                    }
                };
                response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(objectMessage), Encoding.UTF8, "application/json"));
            }
            catch(Exception exception)
            {
                Console.WriteLine($"Challonge error!\n{exception}");
                throw;
            }

            if (response == null)
            {
                throw new Exception("Null response");
            }

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            ParticipantCreateResponse participantCreateResponse = JsonConvert.DeserializeObject<ParticipantCreateResponse>(content);
            return participantCreateResponse.participant.id;
        }
    }
}
