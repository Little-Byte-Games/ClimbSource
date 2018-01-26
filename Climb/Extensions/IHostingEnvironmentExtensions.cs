using Microsoft.AspNetCore.Hosting;

namespace Climb.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IHostingEnvironmentExtensions
    {
        public static bool IsMemoryDB(this IHostingEnvironment environment)
        {
            return environment.EnvironmentName == "MemoryDB";
        }
    }
}