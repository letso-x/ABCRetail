using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class OrdersController : Controller
    {
        private readonly QueueStorageService _queue;
        private readonly FileStorageService _fileStorage;

        public OrdersController(
            QueueStorageService queue, FileStorageService fileStorage)
        {
            _queue = queue;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            order.OrderId = Guid.NewGuid().ToString();
            order.OrderDate = DateTime.Now;
            // Send order to Azure Queue
            await _queue.SendOrderAsync(order);

            // Create order log in Azure Files
            await _fileStorage.CreateLogFileAsync(
                 $"OrderLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                $"Order {order.OrderId} was submitted for " +
                $"Customer {order.CustomerId}, " +
                $"Product {order.ProductId}, " +
                $"Quantity {order.Quantity}.");

            return RedirectToAction(nameof(Create));
        }
    }
}