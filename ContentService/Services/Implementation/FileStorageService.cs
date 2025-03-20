using ContentService.Services.Interface;
using Google.Cloud.Storage.V1;

namespace ContentService.Services.Implementation
{
    public class FileStorageService : IFileStorageService
    {
       // private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        //StorageClient storageClient,

        public FileStorageService( IConfiguration configuration)
        {
           // _storageClient = storageClient;
            _bucketName = configuration["GCS:BucketName"];
            if (string.IsNullOrWhiteSpace(_bucketName))
            {
                throw new Exception("GCS bucket name is not configured.");
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Invalid file.");

            var fileExtension = Path.GetExtension(file.FileName);
            var objectKey = $"{folderName}/{Guid.NewGuid()}{fileExtension}";

            using (var stream = file.OpenReadStream())
            {
                //await _storageClient.UploadObjectAsync(
                //    bucket: _bucketName,
                //    objectName: objectKey,
                //    contentType: file.ContentType,
                //    source: stream
                //);
            }

            var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{objectKey}";
            return publicUrl;
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return;

            var prefix = $"https://storage.googleapis.com/{_bucketName}/";
            if (!fileUrl.StartsWith(prefix))
                throw new Exception("Invalid file URL.");

            var objectKey = fileUrl.Substring(prefix.Length);
            //await _storageClient.DeleteObjectAsync(_bucketName, objectKey);
        }
    }
}
