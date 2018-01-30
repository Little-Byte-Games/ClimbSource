using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Climb.Models;

namespace Climb.Services
{
    public abstract class CdnService
    {
        public enum ImageTypes
        {
            ProfilePic,
            ProfileBanner,
            CharacterPic,
        }

        public readonly ReadOnlyDictionary<ImageTypes, ImageRules> imageData;

        protected abstract string Root { get; }

        protected CdnService()
        {
            var imageDataDict = new Dictionary<ImageTypes, ImageRules>
            {
                {ImageTypes.ProfilePic, new ImageRules(100 * 1024, 150, 150, "profile-pictures", LeagueUser.MissingPic)},
                {ImageTypes.ProfileBanner, new ImageRules(3 * 1024 * 1024, 1500, 300, "profile-banners", "/images/user/Banner_Smoke.png")},
                {ImageTypes.CharacterPic, new ImageRules(20 * 1024, 64, 64, "characters")},
            };
            imageData = new ReadOnlyDictionary<ImageTypes, ImageRules>(imageDataDict);
        }

        protected abstract Task UploadImageInternal(IFormFile imageFile, string folder, string fileKey);

        public string GetImageUrl(ImageTypes imageType, string imageKey)
        {
            var imageTypeData = imageData[imageType];
            return string.IsNullOrWhiteSpace(imageKey) ? imageTypeData.missingUrl : $"{Root}/{imageTypeData.folder}/{imageKey}";
        }

        public async Task<string> UploadImage(ImageTypes imageType, IFormFile imageFile)
        {
            if(!IsValid(imageType, imageFile))
            {
                throw new ArgumentException($"Fill size {imageFile.Length:N0}B exceeds limit {imageData[imageType].maxFileSize:N0}B.");
            }

            var fileKey = GenerateFileKey(imageFile);
            await UploadImageInternal(imageFile, imageData[imageType].folder, fileKey);
            return fileKey;
        }

        private bool IsValid(ImageTypes imageType, IFormFile file)
        {
            return file.Length <= imageData[imageType].maxFileSize;
        }

        private static string GenerateFileKey(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = Path.GetInvalidFileNameChars().Aggregate(Path.GetFileNameWithoutExtension(file.FileName), (current, c) => current.Replace(c, '_'));
            fileName = fileName.Replace(".", "");
            var fileKey = $"{fileName}_{Guid.NewGuid()}{fileExtension}";
            return fileKey;
        }
    }
}