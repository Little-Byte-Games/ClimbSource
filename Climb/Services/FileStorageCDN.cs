using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Climb.Services
{
    public class FileStorageCdn : CdnService
    {
        private const string Cdn = @"temp\cdn";
        private readonly string localCdnPath;

        public FileStorageCdn() : base(@"temp\cdn")
        {
            localCdnPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Cdn);
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
    }
}