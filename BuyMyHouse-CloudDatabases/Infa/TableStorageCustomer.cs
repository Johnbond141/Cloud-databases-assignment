using Domain.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infa
{
    public class TableStorageCustomer : ITableStorage<Customer>
    {
        private string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        public CloudTable _cloudTable { get; set; }
        public TableStorageCustomer()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            _cloudTable = cloudTableClient.GetTableReference("Customers");
        }

        public async Task<Customer> CreateEntity(Customer c)
        {
            Customer customerResult = new Customer();
            try
            {
                TableOperation tableOperation = TableOperation.Insert(c);
                TableResult result = await _cloudTable.ExecuteAsync(tableOperation);
                customerResult = result.Result as Customer;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
            }
            return customerResult;
        }

        public async Task<bool> DeleteEntity(string pk, string rk)
        {
            var tableEntity = await GetEntityByPartitionKeyAndRowKey(pk, rk);
            if (tableEntity is not null)
            {
                TableOperation tableOperation = TableOperation.Delete(tableEntity);
                await _cloudTable.ExecuteAsync(tableOperation);
                return true;
            }
            return false;

        }

        public async Task<Customer> GetEntityByPartitionKeyAndRowKey(string pk, string rk)
        {
            try
            {
                TableQuery<Customer> query = new TableQuery<Customer>().Where
                    (TableQuery.CombineFilters
                    (TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, pk), TableOperators.And, TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rk)));

                var customer = await _cloudTable.ExecuteQuerySegmentedAsync(query, null);

                return customer.Results.FirstOrDefault();

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public async Task<bool> UpdateEntity(Customer c)
        {
            if (c is not null)
            {
                TableOperation tableOperation = TableOperation.Replace(c);
                await _cloudTable.ExecuteAsync(tableOperation);
                return true;
            }
            return false;
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();

            TableQuery<Customer> query = new TableQuery<Customer>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThan, "0"));

            foreach (Customer customer in await _cloudTable.ExecuteQuerySegmentedAsync(query, null))
            {
                customers.Add(customer);
            }
            return customers;
        }
    }
}
