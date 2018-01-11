using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        private readonly ReadOnlyDictionary<ImageTypes, ImageRules> imageData;

        protected CdnService()
        {
            var imageDataDict = new Dictionary<ImageTypes, ImageRules>
            {
                {ImageTypes.ProfilePic, new ImageRules(15 * 1024, 60, 60)},
                {ImageTypes.ProfileBanner, new ImageRules(3 * 1024 * 1024, 1500, 300)},
                {ImageTypes.ProfilePic, new ImageRules(10 * 1024, 60, 60)},
            };
            imageData = new ReadOnlyDictionary<ImageTypes, ImageRules>(imageDataDict);
        }

        protected abstract Task UploadImageInternal(ImageTypes imageType, IFormFile imageFile, string fileKey);

        public string GetImageUrl(ImageTypes imageType, string imageKey)
        {
        }

        public async Task<string> UploadImage(ImageTypes imageType, IFormFile imageFile)
        {
            if(!IsValid(imageType, imageFile))
            {
                throw new ArgumentException($"Fill size {imageFile.Length:N0}B exceeds limit {imageData[imageType].maxFileSize:N0}B.");
            }

            var fileKey = GenerateFileKey(imageFile);
            await UploadImageInternal(imageType, imageFile, fileKey);
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