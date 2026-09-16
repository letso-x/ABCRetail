
using Azure;
using Azure.Data.Tables;
using ABCRetail.Models;

namespace ABCRetail.Services
    {
        public class TableStorageService
        {
            private readonly TableClient _customerTable;
            private readonly TableClient _productTable;

            public TableStorageService(IConfiguration configuration)
            {
                string connectionString =
                    configuration.GetConnectionString("AzureTableStorage")!;

                _customerTable = new TableClient(
                    connectionString,
                    "Customers");

                _productTable = new TableClient(
                    connectionString,
                    "Products");

                _customerTable.CreateIfNotExists();
                _productTable.CreateIfNotExists();
            }

            // CREATE
            public async Task AddCustomerAsync(Customer customer)
            {
                await _customerTable.AddEntityAsync(customer);
            }
         public async Task AddProductAsync(Product product)
        {
            await _productTable.AddEntityAsync(product);
        }

        // READ
        public async Task<List<Customer>> GetCustomersAsync()
            {
                var customers = new List<Customer>();

                await foreach (Customer customer in
                    _customerTable.QueryAsync<Customer>())
                {
                    customers.Add(customer);
                }

                return customers;
            }
        public async Task<List<Product>> GetProductsAsync()
        {
            var products = new List<Product>();

            await foreach (Product product in
                _productTable.QueryAsync<Product>())
            {
                products.Add(product);
            }

            return products;
        }
        // UPDATE
        public async Task UpdateCustomerAsync(Customer customer)
            {
                await _customerTable.UpdateEntityAsync(
                    customer,
                    customer.ETag,
                    TableUpdateMode.Replace);
            }
        public async Task UpdateProductAsync(Product product)
        {
            await _productTable.UpdateEntityAsync(
                product,
                product.ETag,
                TableUpdateMode.Replace);
        }

        // DELETE
        public async Task DeleteCustomerAsync(
                string partitionKey,
                string rowKey)
            {
                await _customerTable.DeleteEntityAsync(
                    partitionKey,
                    rowKey);
            }
        public async Task DeleteProductAsync(
               string partitionKey,
               string rowKey)
        {
            await _productTable.DeleteEntityAsync(
                partitionKey,
                rowKey);
        }
    }
    }


