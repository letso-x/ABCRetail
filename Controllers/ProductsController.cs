using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TableStorageService _tableStorage;

        public ProductsController(TableStorageService tableStorage)
        {
            _tableStorage = tableStorage;
        }

        // READ
        public async Task<IActionResult> Index()
        {
            var products =
                await _tableStorage.GetProductsAsync();

            return View(products);
        }

        // CREATE - display form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - save form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            product.PartitionKey = "products";
            product.RowKey = Guid.NewGuid().ToString();

            await _tableStorage.AddProductAsync(product);

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(
            string partitionKey,
            string rowKey)
        {
            await _tableStorage.DeleteProductAsync(
                partitionKey,
                rowKey);

            return RedirectToAction(nameof(Index));
        }
    }
}