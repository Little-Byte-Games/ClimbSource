using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Climb.Services
{
    public class FileStorageCdn : CdnService
    {
        private const string Cdn = @"temp\cdn";
        private readonly string localCdnPath;

        protected override string Root { get; }

        public FileStorageCdn()
        {
            localCdnPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Cdn);
            Root = @"\temp\cdn";
        }

        protected override async Task UploadImageInternal(IFormFile imageFile, string folder, string fileKey)
        {
            var folderPath = Path.Combine(localCdnPath, folder);
            var filePath = Path.Combine(folderPath, fileKey);

            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using(var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
        }

        public override Task DeleteImage(ImageTypes imageType, string fileKey)
        {
            var folder = imageData[imageType].folder;
            var folderPath = Path.Combine(localCdnPath, folder);
            var filePath = Path.Combine(folderPath, fileKey);

            File.Delete(filePath);
            return Task.CompletedTask;
        }
    }
}