using System.Threading.Tasks;
using Climb.Models;
using Microsoft.AspNetCore.Http;

namespace Climb.Services
{
    public interface ICdnService
    {
        string GetProfilePic(LeagueUser leagueUser);
        Task<string> UploadProfilePic(IFormFile file);

        string GetCharacterPic(Character character);
        Task<string> UploadCharacterPic(IFormFile file);
    }
}