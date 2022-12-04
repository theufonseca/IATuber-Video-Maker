using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.ExternalServices.Storage
{
    public class GoogleCloudStorage
    {
        private readonly GoogleCredential googleCredential;
        private readonly StorageClient storageClient;
        private readonly string bucketName;

        public GoogleCloudStorage(IConfiguration configuration)
        {
            googleCredential = GoogleCredential.FromFile(
                configuration.GetSection("GoogleCloudStorage:CredentialFile").Value);
            storageClient = StorageClient.Create(googleCredential);
            bucketName = configuration.GetSection("GoogleCloudStorage:BucketName").Value!;
        }

        public async Task<string> UploadFileAsync(string fileName, MemoryStream stream)
        {
            var result = await storageClient.UploadObjectAsync(bucketName, fileName, null, stream);
            return result.MediaLink;
        }

        public async Task DeleteFileAsync(string fileName)
            => await storageClient.DeleteObjectAsync(bucketName, fileName);
    }
}
