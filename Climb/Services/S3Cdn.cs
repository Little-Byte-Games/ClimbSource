using Amazon;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Climb.Services
{
    public class S3Cdn : CdnService
    {
        private readonly string accessKey;
        private readonly string secretKey;
        private readonly string environment;
        private readonly string bucketName;

        public S3Cdn(IConfiguration configuration, IHostingEnvironment environment) : base("https://s3.amazonaws.com")
        {
            var awsSection = configuration.GetSection("AWS");

            accessKey = awsSection["AccessKey"];
            secretKey = awsSection["SecretKey"];
            bucketName = awsSection["Bucket"];

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
        }


        protected override async Task UploadImageInternal(IFormFile imageFile, string folder, string fileKey)
        {
            var transfer = new TransferUtility(accessKey, secretKey, RegionEndpoint.USEast1);
            await transfer.UploadAsync(imageFile.OpenReadStream(), string.Join("/", bucketName, environment, folder), fileKey);
        }
    }
}
