using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Climb.Consts;
using Climb.Models;
using Microsoft.AspNetCore.Http;

namespace Climb.Services
{
    public class FileStorageCdn : ICdnService
    {
        private const string Cdn = @"temp\cdn";
        private readonly string localCdnPath;

        public int MaxFileSize => 15 * 1024;

        public FileStorageCdn()
        {
            localCdnPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Cdn);
            Directory.CreateDirectory(localCdnPath);
            Directory.CreateDirectory(Path.Combine(localCdnPath, CdnConsts.ProfilePics));
            Directory.CreateDirectory(Path.Combine(localCdnPath, CdnConsts.CharacterIcons));
        }

        public string GetProfilePic(User user)
        {
            return string.IsNullOrWhiteSpace(user.ProfilePicKey) ? LeagueUser.MissingPic : "/" + Path.Combine(Cdn, CdnConsts.ProfilePics, user.ProfilePicKey);
        }

        public string GetProfilePic(LeagueUser leagueUser)
        {
            string profilePicKey;
            if(!string.IsNullOrWhiteSpace(leagueUser.ProfilePicKey))
            {
                profilePicKey = leagueUser.ProfilePicKey;
            }
            else if(!string.IsNullOrWhiteSpace(leagueUser.User.ProfilePicKey))
            {
                profilePicKey = leagueUser.User.ProfilePicKey;
            }
            else
            {
                return LeagueUser.MissingPic;
            }

            return $"/{Cdn}/{CdnConsts.ProfilePics}/{profilePicKey}";
        }

        public async Task<string> UploadProfilePic(IFormFile file)
        {
            return await UploadFile(file, CdnConsts.ProfilePics);
        }

        public string GetCharacterPic(Character character)
        {
            return "/" + Path.Combine(Cdn, CdnConsts.CharacterIcons, character.PicKey);
        }

        public async Task<string> UploadCharacterPic(IFormFile file)
        {
            return await UploadFile(file, CdnConsts.CharacterIcons);
        }

        private async Task<string> UploadFile(IFormFile file, string folderName)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            var fileKey = Path.GetInvalidFileNameChars().Aggregate(Path.GetFileNameWithoutExtension(file.FileName), (current, c) => current.Replace(c, '_')) + fileExtension;
            var filePath = Path.Combine(localCdnPath, folderName, fileKey);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return fileKey;
        }
    }
}
