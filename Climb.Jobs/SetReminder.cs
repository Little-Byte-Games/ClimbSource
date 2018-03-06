using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Climb.Jobs
{
    public static class SetReminder
    {
        [FunctionName("SetReminder")]
        public static async Task Run([TimerTrigger("0 0 18 * * MON,WED,FRI")]TimerInfo myTimer, TraceWriter log)
        {
            HttpClient client = new HttpClient();
            var key = Environment.GetEnvironmentVariable("SecretKey");
            client.DefaultRequestHeaders.Add("key", key);

            var domain = Environment.GetEnvironmentVariable("Domain");
            var uri = new Uri($"{domain}/Leagues/SendSetReminders");

            var response = await client.PostAsync(uri, null);
            if(response.IsSuccessStatusCode)
            {
                log.Info($"Set reminders sent at: {DateTime.Now}");
            }
            else
            {
                log.Error($"Set reminders at: {DateTime.Now} because {response.ReasonPhrase}");
            }
        }
    }
}
