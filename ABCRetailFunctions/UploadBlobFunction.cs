
using System.Net;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ABCRetailFunctions
{
    public class UploadBlobFunction
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public UploadBlobFunction()
        {
            _connectionString =
                Environment.GetEnvironmentVariable("ABCStorageConnection")
                ?? throw new InvalidOperationException(
                    "ABCStorageConnection is not configured.");

            _containerName =
                Environment.GetEnvironmentVariable("ABCBlobContainer")
                ?? throw new InvalidOperationException(
                    "ABCBlobContainer is not configured.");
        }

        [Function("UploadBlobFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "uploadblob/{fileName}")]
            HttpRequestData req,
            string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                var badResponse =
                    req.CreateResponse(HttpStatusCode.BadRequest);

                await badResponse.WriteStringAsync(
                    "A file name is required.");

                return badResponse;
            }

            var blobServiceClient =
                new BlobServiceClient(_connectionString);

            var containerClient =
                blobServiceClient.GetBlobContainerClient(_containerName);

            await containerClient.CreateIfNotExistsAsync();

            var blobClient =
                containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(
                req.Body,
                overwrite: true);

            var response =
                req.CreateResponse(HttpStatusCode.OK);

            await response.WriteStringAsync(
                $"File '{fileName}' uploaded successfully.");

            return response;
        }
    }
}