using Climb.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Climb.Services
{
    // TODO: Make abstract class.
    public interface ICdnService
    {
        int MaxFileSize { get; }

        string GetProfilePic(IProfile profile);
        Task<string> UploadProfilePic(IFormFile file);

        string GetCharacterPic(Character character);
        Task<string> UploadCharacterPic(IFormFile file);
    }
}