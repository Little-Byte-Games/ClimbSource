using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3.Transfer;
using Climb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Climb.Services
{
    public class CDNService : ICDNService
    {
        private readonly string accessKey;
        private readonly string secretKey;
        private readonly string rootUrl;
        private readonly string bucketName;
        private readonly string profilePics;
        private readonly string characterPics;

        public CDNService(IConfiguration configuration)
        {
            var awsSection = configuration.GetSection("AWS");

            accessKey = awsSection["AccessKey"];
            secretKey = awsSection["SecretKey"];

            rootUrl = awsSection["RootUrl"];
            bucketName = awsSection["Bucket"];
            rootUrl += "/" + bucketName;

            profilePics = awsSection["ProfilePics"];
            characterPics = awsSection["CharacterPics"];
        }

        public string GetProfilePic(LeagueUser leagueUser)
        {
            return string.IsNullOrWhiteSpace(leagueUser.ProfilePicKey) ? LeagueUser.MissingPic : string.Join("/", rootUrl, profilePics, leagueUser.ProfilePicKey);
        }

        public async Task<string> UploadProfilePic(IFormFile file)
        {
            var fileKey = GenerateFileKey(file);
            await UploadFile(file, profilePics, fileKey);
            return fileKey;
        }

        public string GetCharacterPic(Character character)
        {
            return string.Join("/", rootUrl, characterPics, character.PicKey);
        }

        public async Task<string> UploadCharacterPic(IFormFile file)
        {
            var fileKey = GenerateFileKey(file);
            await UploadFile(file, characterPics, fileKey);
            return fileKey;
        }

        private static string GenerateFileKey(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            var fileKey = $"{Guid.NewGuid()}_{DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture)}{fileExtension}";
            fileKey = fileKey.ToLowerInvariant();
            return fileKey;
        }

        private async Task UploadFile(IFormFile file, string folder, string fileKey)
        {
            var transfer = new TransferUtility(accessKey, secretKey, RegionEndpoint.USEast1);
            await transfer.UploadAsync(file.OpenReadStream(), bucketName + "/" + folder, fileKey);
        }
    }
}
