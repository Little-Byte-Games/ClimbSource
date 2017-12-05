using Amazon;
using Amazon.S3.Transfer;
using Climb.Consts;
using Climb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Climb.Services
{
    public class CdnService : ICdnService
    {
        private readonly string accessKey;
        private readonly string secretKey;
        private readonly string rootUrl;
        private readonly string environment;
        private readonly string bucketName;
        private readonly string profilePics;
        private readonly string characterPics;

        public CdnService(IConfiguration configuration, IHostingEnvironment environment)
        {
            var awsSection = configuration.GetSection("AWS");

            accessKey = awsSection["AccessKey"];
            secretKey = awsSection["SecretKey"];

            rootUrl = CdnConsts.RootUrl;
            bucketName = CdnConsts.AppBucket;
            rootUrl += "/" + bucketName;

            if(environment.IsDevelopment())
            {
                this.environment = "dev";
            }
            else if(environment.IsProduction())
            {
                this.environment = "rel";
            }
            else
            {
                throw new NotSupportedException($"Environment {environment.EnvironmentName} does not have a corresponding CDN bucket.");
            }
            rootUrl += "/" + this.environment;

            profilePics = CdnConsts.ProfilePics;
            characterPics = CdnConsts.CharacterIcons;
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
            await transfer.UploadAsync(file.OpenReadStream(), string.Join("/", bucketName, environment, folder), fileKey);
        }
    }
}
