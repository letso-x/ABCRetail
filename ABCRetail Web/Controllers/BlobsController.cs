using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class BlobsController : Controller
    {
        private readonly AzureFunctionsClient _functions;

        public BlobsController(AzureFunctionsClient functions)
        {
            _functions = functions;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            await _functions.UploadBlobAsync(file);

            return RedirectToAction(nameof(Upload));
        }
    }
}