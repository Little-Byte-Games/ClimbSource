using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Climb.Core
{
    public static class SlackController
    {
        private const string ApiPath = @"https://hooks.slack.com/services/";

        public static async Task SendGroupMessage(string apiKey, string message)
        {
            using(var client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(1);
                    var url = ApiPath + apiKey;
                    var objectMessage = new
                    {
                        text = message
                    };
                    await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(objectMessage), Encoding.UTF8, "application/json"));
                }
                catch(Exception exception)
                {
                    Console.WriteLine($"Slack error!\n{exception}");
                }
            }
        }
    }
}