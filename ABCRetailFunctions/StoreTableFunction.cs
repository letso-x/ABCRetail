using ABCRetailFunctions.Models;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace ABCRetail.Functions
{
    public class StoreTableFunction
    {
        private readonly string _connectionString;

        public StoreTableFunction()
        {
            _connectionString =
                Environment.GetEnvironmentVariable("ABCStorageConnection")
                ?? throw new InvalidOperationException(
                    "ABCStorageConnection is not configured.");
        }

        [Function("StoreTableFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            HttpRequestData req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var request = JsonSerializer.Deserialize<TableRequest>(
                requestBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (request == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request.");
                return badResponse;
            }

            if (request.Type.Equals("customer", StringComparison.OrdinalIgnoreCase))
            {
                var customer = JsonSerializer.Deserialize<Customer>(
                    requestBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (customer == null)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Invalid customer.");
                    return badResponse;
                }

                var tableClient = new TableClient(
                    _connectionString,
                    "Customers");

                await tableClient.CreateIfNotExistsAsync();
                await tableClient.AddEntityAsync(customer);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync(
                    $"Customer {customer.Name} stored successfully.");

                return response;
            }

            if (request.Type.Equals("product", StringComparison.OrdinalIgnoreCase))
            {
                var product = JsonSerializer.Deserialize<Product>(
                    requestBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (product == null)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Invalid product.");
                    return badResponse;
                }

                var tableClient = new TableClient(
                    _connectionString,
                    "Products");

                await tableClient.CreateIfNotExistsAsync();
                await tableClient.AddEntityAsync(product);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync(
                    $"Product {product.Name} stored successfully.");

                return response;
            }

            var invalidTypeResponse =
                req.CreateResponse(HttpStatusCode.BadRequest);

            await invalidTypeResponse.WriteStringAsync(
                "Type must be 'customer' or 'product'.");

            return invalidTypeResponse;
        }
    }

    public class TableRequest
    {
        public string Type { get; set; } = "";
    }
}