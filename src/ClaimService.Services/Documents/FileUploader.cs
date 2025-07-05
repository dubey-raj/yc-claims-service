namespace ClaimService.Services.Documents
{
    using Amazon.S3;
    using Amazon.S3.Transfer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class S3FileUploader : IFileUploader
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        /// <summary>
        /// Upload a file to s3 bucket
        /// </summary>
        /// <param name="s3Client"></param>
        /// <param name="configuration"></param>
        public S3FileUploader(IAmazonS3 s3Client, IConfiguration configuration)
        {
            _s3Client = s3Client;
            _bucketName = configuration["BucketName"] ?? throw new ArgumentNullException("Bucket name can not be null");
        }

        ///<inheritdoc/>
        public async Task<List<string>> UploadFilesAsync(string folderName, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                throw new ArgumentException("No files provided for upload.");

            var uploadedUrls = new List<string>();
            var transferUtility = new TransferUtility(_s3Client);

            foreach (var file in files)
            {
                var key = $"{folderName}/{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
                
                using (var stream = file.OpenReadStream())
                {
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = _bucketName,
                        Key = key,
                        InputStream = stream,
                        ContentType = file.ContentType,
                    };

                    await transferUtility.UploadAsync(uploadRequest);
                    uploadedUrls.Add(key);
                }
            }

            return uploadedUrls;
        }
    }
}
