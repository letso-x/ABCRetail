using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class LogsController : Controller
    {
        private readonly AzureFunctionsClient _functions;

        public LogsController(AzureFunctionsClient functions)
        {
            _functions = functions;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            string fileName,
            string content)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                ModelState.AddModelError(
                    "fileName",
                    "File name is required.");

                return View();
            }

            if (!fileName.EndsWith(".txt",
                    StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".txt";
            }

            await _functions.UploadFileAsync(
                fileName,
                content);

            ViewBag.Message =
                $"Log file '{fileName}' created successfully.";

            return View();
        }
    }
}