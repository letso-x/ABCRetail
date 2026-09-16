using System.Net;
using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ABCRetailFunctions
{
    public class UploadFileFunction
    {
        private readonly string _connectionString;
        private readonly string _shareName;

        public UploadFileFunction()
        {
            _connectionString =
                Environment.GetEnvironmentVariable("ABCStorageConnection")
                ?? throw new InvalidOperationException(
                    "ABCStorageConnection is not configured.");

            _shareName =
                Environment.GetEnvironmentVariable("ABCFileShareName")
                ?? throw new InvalidOperationException(
                    "ABCFileShareName is not configured.");
        }

        [Function("UploadFileFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "uploadfile/{fileName}")]
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

            using var memoryStream = new MemoryStream();

            await req.Body.CopyToAsync(memoryStream);

            if (memoryStream.Length == 0)
            {
                var badResponse =
                    req.CreateResponse(HttpStatusCode.BadRequest);

                await badResponse.WriteStringAsync(
                    "The uploaded file is empty.");

                return badResponse;
            }

            memoryStream.Position = 0;

            var shareClient =
                new ShareClient(
                    _connectionString,
                    _shareName);

            // Get the root directory.
            // We do NOT create it because the root already exists.
            var directoryClient =
                shareClient.GetRootDirectoryClient();

            // Create the file with the correct size.
            var fileClient =
                directoryClient.GetFileClient(fileName);

            await fileClient.CreateAsync(memoryStream.Length);

            // Upload the file contents.
            await fileClient.UploadRangeAsync(
                new Azure.HttpRange(
                    0,
                    memoryStream.Length),
                memoryStream);

            var response =
                req.CreateResponse(HttpStatusCode.OK);

            await response.WriteStringAsync(
                $"File '{fileName}' uploaded successfully to Azure Files.");

            return response;
        }
    }
}