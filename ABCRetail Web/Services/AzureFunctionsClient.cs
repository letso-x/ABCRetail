using System.Net.Http.Json;
using System.Text;
using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class AzureFunctionsClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _blobBaseUrl;

        public AzureFunctionsClient(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _blobBaseUrl = configuration["AzureStorage:BlobBaseUrl"]
                ?? throw new InvalidOperationException(
                    "AzureStorage:BlobBaseUrl is not configured.");
        }

        public Task StoreCustomerAsync(Customer customer)
        {
            return PostJsonAsync(
                "api/StoreTableFunction",
                new
                {
                    type = "customer",
                    customer.PartitionKey,
                    customer.RowKey,
                    customer.Name,
                    customer.Email,
                    customer.Phone
                });
        }

        public Task StoreProductAsync(Product product)
        {
            return PostJsonAsync(
                "api/StoreTableFunction",
                new
                {
                    type = "product",
                    product.PartitionKey,
                    product.RowKey,
                    product.Name,
                    product.Description,
                    product.Price,
                    product.ImageFileName,
                    product.ImageUrl
                });
        }

        public async Task<string> UploadBlobAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file supplied.");
            }

            string fileName =
                Guid.NewGuid() + Path.GetExtension(file.FileName);

            using Stream stream = file.OpenReadStream();
            using var content = new StreamContent(stream);

            if (!string.IsNullOrWhiteSpace(file.ContentType))
            {
                content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(
                        file.ContentType);
            }

            await PostContentAsync(
                $"api/uploadblob/{Uri.EscapeDataString(fileName)}",
                content);

            return $"{_blobBaseUrl.TrimEnd('/')}/{Uri.EscapeDataString(fileName)}";
        }

        public Task SendOrderAsync(Order order)
        {
            return PostJsonAsync("api/queue", order);
        }

        public async Task UploadFileAsync(string fileName, string content)
        {
            using var requestContent = new StringContent(
                content,
                Encoding.UTF8,
                "text/plain");

            await PostContentAsync(
                $"api/uploadfile/{Uri.EscapeDataString(fileName)}",
                requestContent);
        }

        private async Task PostJsonAsync(string relativeUrl, object value)
        {
            using var content = JsonContent.Create(value);
            await PostContentAsync(relativeUrl, content);
        }

        private async Task PostContentAsync(
            string relativeUrl,
            HttpContent content)
        {
            using HttpResponseMessage response =
                await _httpClient.PostAsync(relativeUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            string details = await response.Content.ReadAsStringAsync();
            string message = string.IsNullOrWhiteSpace(details)
                ? response.ReasonPhrase ?? "The Azure Function request failed."
                : details.Trim();

            throw new HttpRequestException(
                $"Azure Function request '{relativeUrl}' failed with " +
                $"status {(int)response.StatusCode} ({response.StatusCode}): " +
                message,
                null,
                response.StatusCode);
        }
    }
}
