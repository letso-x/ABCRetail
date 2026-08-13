using Azure.Storage.Queues;
using System.Text.Json;

namespace ABCRetail.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _queue;

        public QueueStorageService(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("AzureTableStorage")!;

            _queue = new QueueClient(
                connectionString,
                "order-processing");

            _queue.CreateIfNotExists();
        }

        public async Task SendOrderAsync(
            object order)
        {
            string message =
                JsonSerializer.Serialize(order);

            await _queue.SendMessageAsync(message);
        }

        public async Task<string?> ReceiveOrderAsync()
        {
            var response =
                await _queue.ReceiveMessageAsync();

            if (response.Value == null)
                return null;

            string message =
                response.Value.MessageText;

            await _queue.DeleteMessageAsync(
                response.Value.MessageId,
                response.Value.PopReceipt);

            return message;
        }
    }
}