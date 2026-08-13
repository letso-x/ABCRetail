using Azure.Storage.Blobs;

namespace ABCRetail.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("AzureTableStorage")!;

            _container = new BlobContainerClient(
                connectionString,
                "product-images");

            _container.CreateIfNotExists();
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file supplied.");

            string fileName =
                Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            BlobClient blob =
                _container.GetBlobClient(fileName);

            using Stream stream = file.OpenReadStream();

            await blob.UploadAsync(stream, overwrite: true);

            return blob.Uri.ToString();
        }
    }
}