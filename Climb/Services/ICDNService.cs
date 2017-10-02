using System.Threading.Tasks;
using Climb.Models;
using Microsoft.AspNetCore.Http;

namespace Climb.Services
{
    public interface ICDNService
    {
        string GetProfilePic(LeagueUser leagueUser);
        Task<string> UploadProfilePic(IFormFile file);
    }
}