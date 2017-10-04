using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Climb.Azure
{
    public static class AutoSnapshotter
    {
        [FunctionName("AutoSnapshotter")]
        public static async void Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("key", "steve");
            var uri = new Uri("https://localhost:44357/Leagues/TakeAllRankSnapshots");

            await client.PostAsync(uri, null);
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
