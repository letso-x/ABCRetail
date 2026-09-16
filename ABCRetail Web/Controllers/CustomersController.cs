using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class CustomersController : Controller
    {
        private readonly TableStorageService _tableStorage;
        private readonly FileStorageService _fileStorage;

        public CustomersController(TableStorageService tableStorage, FileStorageService fileStorage)
        {
            _tableStorage = tableStorage;
            _fileStorage = fileStorage;
        }

        // READ
        public async Task<IActionResult> Index()
        {
            var customers =
                await _tableStorage.GetCustomersAsync();

            return View(customers);
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
        public async Task<IActionResult> Create(Customer customer)
        {
            string fileName =$"CustomerLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            customer.PartitionKey = "Customers";
            customer.RowKey = Guid.NewGuid().ToString();

            await _tableStorage.AddCustomerAsync(customer);
            await _fileStorage.CreateLogFileAsync(
                     fileName,
                    $"Customer '{customer.Name}' was created. " +
                    $"Email: {customer.Email}");

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(
            string partitionKey,
            string rowKey)
        {
            await _tableStorage.DeleteCustomerAsync(
                partitionKey,
                rowKey);

            return RedirectToAction(nameof(Index));
        }
    }
}