using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AzureFunctionsClient _functions;

        public OrdersController(AzureFunctionsClient functions)
        {
            _functions = functions;
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
            await _functions.SendOrderAsync(order);

            // Create order log in Azure Files
            await _functions.UploadFileAsync(
                 $"OrderLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                $"Order {order.OrderId} was submitted for " +
                $"Customer {order.CustomerId}, " +
                $"Product {order.ProductId}, " +
                $"Quantity {order.Quantity}.");

            return RedirectToAction(nameof(Create));
        }
    }
}