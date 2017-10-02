using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Climb.Azure
{
    public static class AutoSnapshotter
    {
        [FunctionName("AutoSnapshotter")]
        public static void Run([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
