using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TableStorageService _tableStorage;
        private readonly BlobStorageService _blobStorage;
        private readonly FileStorageService _fileStorage;

        public ProductsController(TableStorageService tableStorage, BlobStorageService blobStorage, FileStorageService fileStorage)
        {
            _tableStorage = tableStorage;
            _blobStorage = blobStorage;
           _fileStorage = fileStorage;
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
        public async Task<IActionResult> Create(
    Product product,
    IFormFile image)
        {
            string fileName = $"ProductLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // 1. Upload image to Azure Blob Storage
            if (image != null && image.Length > 0)
            {
                string imageUrl =
                    await _blobStorage.UploadAsync(image);

                product.ImageUrl = imageUrl;
                product.ImageFileName = image.FileName;
            }

            // 2. Store product in Azure Table Storage
            product.PartitionKey = "Products";
            product.RowKey = Guid.NewGuid().ToString();

            await _tableStorage.AddProductAsync(product);

            // 3. Create log in Azure Files
            await _fileStorage.CreateLogFileAsync(
                fileName,
                $"Product '{product.Name}' was created. " +
                $"Image: {product.ImageFileName}");

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