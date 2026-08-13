using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class BlobsController : Controller
    {
        private readonly BlobStorageService _blobStorage;

        public BlobsController(
            BlobStorageService blobStorage)
        {
            _blobStorage = blobStorage;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            await _blobStorage.UploadAsync(file);

            return RedirectToAction(nameof(Upload));
        }
    }
}