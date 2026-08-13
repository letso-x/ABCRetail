using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Files.Shares;

namespace ABCRetail.Services
{
    public class FileStorageService
    {
        private readonly ShareClient _shareClient;

        public FileStorageService(IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("AzureStorage");

            _shareClient = new ShareClient(
                connectionString,
                "application-logs");

            _shareClient.CreateIfNotExists();
        }

        public async Task CreateLogFileAsync(
            string fileName,
            string content)
        {
            var directoryClient =
                _shareClient.GetRootDirectoryClient();

            var fileClient =
                directoryClient.GetFileClient(fileName);

            byte[] contentBytes =
                System.Text.Encoding.UTF8.GetBytes(content);

            using var stream =
                new MemoryStream(contentBytes);

            await fileClient.CreateAsync(stream.Length);

            await fileClient.UploadAsync(stream);
        }
    }
}