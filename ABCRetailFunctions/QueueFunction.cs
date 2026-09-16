
using System.Net;
using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ABCRetailFunctions
{
    public class QueueFunction
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public QueueFunction()
        {
            _connectionString =
                Environment.GetEnvironmentVariable("ABCStorageConnection")
                ?? throw new InvalidOperationException(
                    "ABCStorageConnection is not configured.");

            _queueName =
                Environment.GetEnvironmentVariable("ABCQueueName")
                ?? throw new InvalidOperationException(
                    "ABCQueueName is not configured.");
        }

        [Function("QueueFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                "post",
                Route = "queue")]
            HttpRequestData req)
        {
            var queueClient =
                new QueueClient(_connectionString, _queueName);

            await queueClient.CreateIfNotExistsAsync();

            if (req.Method.Equals("POST",
                StringComparison.OrdinalIgnoreCase))
            {
                var body =
                    await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(body))
                {
                    var badResponse =
                        req.CreateResponse(HttpStatusCode.BadRequest);

                    await badResponse.WriteStringAsync(
                        "Provide transaction information in the request body.");

                    return badResponse;
                }

                await queueClient.SendMessageAsync(body);

                var response =
                    req.CreateResponse(HttpStatusCode.OK);

                await response.WriteStringAsync(
                    "Transaction message added to the queue.");

                return response;
            }

            // GET peeks at the next message without removing it.
            var peekResult = await queueClient.PeekMessageAsync();

            var getResponse =
                req.CreateResponse(HttpStatusCode.OK);

            if (peekResult.Value == null)
            {
                await getResponse.WriteStringAsync(
                    "The queue is currently empty.");
            }
            else
            {
                await getResponse.WriteStringAsync(
                    peekResult.Value.MessageText);
            }

            return getResponse;
        }
    }
}