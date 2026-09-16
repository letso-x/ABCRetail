using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class LogsController : Controller
    {
        private readonly FileStorageService _fileStorage;

        public LogsController(FileStorageService fileStorage)
        {
            _fileStorage = fileStorage;
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

            await _fileStorage.CreateLogFileAsync(
                fileName,
                content);

            ViewBag.Message =
                $"Log file '{fileName}' created successfully.";

            return View();
        }
    }
}