using TransactionCore.Services.Interface;

namespace TransactionCore.Services.Implementation
{
    public class FileStorageService : IFileStorageService
    {
        // private readonly StorageClient _storageClient;
        private readonly IConfiguration _configuration;

        //StorageClient storageClient,

        public FileStorageService(IConfiguration configuration)
        {
            // _storageClient = storageClient;
            _configuration = configuration;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Invalid file.");
            var bucketName = _configuration["GCS:BucketName"];
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new Exception("GCS bucket name is not configured.");

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

            var publicUrl = $"https://storage.googleapis.com/{bucketName}/{objectKey}";
            return publicUrl;
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
                return;

            var bucketName = _configuration["GCS:BucketName"];
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new Exception("GCS bucket name is not configured.");

            var prefix = $"https://storage.googleapis.com/{bucketName}/";
            if (!fileUrl.StartsWith(prefix))
                throw new Exception("Invalid file URL.");

            var objectKey = fileUrl.Substring(prefix.Length);
            //await _storageClient.DeleteObjectAsync(_bucketName, objectKey);
        }
    }
}
