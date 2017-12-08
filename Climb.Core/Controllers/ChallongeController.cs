using Climb.Core.Challonge;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Climb.Core.Controllers
{
    public static class ChallongeController
    {
        private static readonly HttpClient client = new HttpClient();

        // TODO: Add participants.
        public static async Task<(int id, string tournamentUrl)> CreateTournament(string apiKey, string leagueName, string tournamentName, IEnumerable<string> participants)
        {
            var tournamentUrl = DateTime.Now.Ticks.ToString();
            HttpResponseMessage response = null;
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
                Console.WriteLine($"CHallonge error!\n{exception}");
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

            TournamentCreateResponse tournamentCreateResponse = JsonConvert.DeserializeObject<TournamentCreateResponse>(content);
            return (tournamentCreateResponse.tournament.id, tournamentUrl);
        }
    }
}
