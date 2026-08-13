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

        public async Task UploadAsync(
            IFormFile file)
        {
            if (file == null || file.Length == 0)
                return;

            BlobClient blob =
                _container.GetBlobClient(file.FileName);

            using Stream stream = file.OpenReadStream();

            await blob.UploadAsync(
                stream,
                overwrite: true);
        }
    }
}