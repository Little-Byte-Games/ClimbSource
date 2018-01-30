using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
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
        private readonly IAmazonS3 client;
        
        protected override string Root { get; }

        public S3Cdn(IConfiguration configuration, IHostingEnvironment environment)
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

            Root = $"https://s3.amazonaws.com/{bucketName}/{this.environment}";

            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            client = new AmazonS3Client(credentials, RegionEndpoint.USEast1);
        }


        protected override async Task UploadImageInternal(IFormFile imageFile, string folder, string fileKey)
        {
            var transfer = new TransferUtility(accessKey, secretKey, RegionEndpoint.USEast1);
            await transfer.UploadAsync(imageFile.OpenReadStream(), string.Join("/", bucketName, environment, folder), fileKey);
        }

        public override async Task DeleteImage(ImageTypes imageType, string fileKey)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = $"{environment}/{imageData[imageType].folder}/{fileKey}"
            };
            await client.DeleteObjectAsync(deleteRequest);
        }
    }
}
