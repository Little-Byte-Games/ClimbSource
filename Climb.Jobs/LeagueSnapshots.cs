using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Climb.Jobs
{
    public static class LeagueSnapshots
    {
        [FunctionName("LeagueSnapshots")]
        public static async Task Run([TimerTrigger("0 0 10 * * SUN")]TimerInfo myTimer, TraceWriter log)
        {
            HttpClient client = new HttpClient();
            var key = Environment.GetEnvironmentVariable("SecrectKey");
            client.DefaultRequestHeaders.Add("key", key);
            var uri = new Uri("https://localhost:44304/Leagues/TakeAllRankSnapshots");

            var response = await client.PostAsync(uri, null);
            if(response.IsSuccessStatusCode)
            {
                log.Info($"Snapshots taken at: {DateTime.Now}");
            }
            else
            {
                log.Error($"Snapshots failed at: {DateTime.Now} because {response.ReasonPhrase}");
            }
        }
    }
}
