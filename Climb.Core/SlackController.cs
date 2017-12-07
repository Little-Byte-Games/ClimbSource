﻿using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Climb.Core
{
    public static class SlackController
    {
        private const string ApiPath = @"https://hooks.slack.com/services/";
        private const string ConversationsPath = @"https://slack.com/api/conversations.open/";
        private static readonly HttpClient client = new HttpClient();

        public static async Task SendGroupMessage(string apiKey, string message)
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

        public static async Task SendConversationMessage(string apiKey, string message, params string[] users)
        {
            
        }
    }
}