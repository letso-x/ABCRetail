using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class OrdersController : Controller
    {
        private readonly QueueStorageService _queue;

        public OrdersController(
            QueueStorageService queue)
        {
            _queue = queue;
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

            await _queue.SendOrderAsync(order);

            return RedirectToAction(nameof(Create));
        }
    }
}