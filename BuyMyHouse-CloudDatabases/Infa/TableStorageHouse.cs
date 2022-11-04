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
    public class TableStorageHouse : ITableStorage<House>
    {
        private string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        public CloudTable _cloudTable { get; set; }

        public TableStorageHouse()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
            _cloudTable = tableClient.GetTableReference("Houses");
        }

        public async Task<List<House>> GetHouses(double min, double max)
        {
            try
            {
                TableQuery<House> query = new TableQuery<House>()
                    .Where(TableQuery.CombineFilters(TableQuery.GenerateFilterConditionForDouble("Mortgage", QueryComparisons.GreaterThanOrEqual, min),
                        TableOperators.And, TableQuery.GenerateFilterConditionForDouble("Mortgage", QueryComparisons.LessThanOrEqual, max)));

                var House = await _cloudTable.ExecuteQuerySegmentedAsync(query, null);

                return House.Results;

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public async Task<House> CreateEntity(House h)
        {
            House HouseResult = new House();
            try
            {
                TableOperation tableOperation = TableOperation.Insert(h);
                TableResult result = await _cloudTable.ExecuteAsync(tableOperation);
                HouseResult = result.Result as House;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
            }
            return HouseResult;
        }

        public async Task<House> GetEntityByPartitionKeyAndRowKey(string pk, string rk)
        {
            try
            {
                TableQuery<House> query = new TableQuery<House>().Where(TableQuery.CombineFilters
                    (TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, pk), TableOperators.And, TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rk)));

                var House = await _cloudTable.ExecuteQuerySegmentedAsync(query, null);

                return House.Results.FirstOrDefault();

            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        public async Task<bool> UpdateEntity(House h)
        {
            if (h is not null)
            {
                TableOperation tableOperation = TableOperation.Replace(h);
                await _cloudTable.ExecuteAsync(tableOperation);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteEntity(string pk, string rk)
        {
            var entity = await GetEntityByPartitionKeyAndRowKey(pk, rk);
            if (entity is not null)
            {
                TableOperation tableOperation = TableOperation.Delete(entity);
                await _cloudTable.ExecuteAsync(tableOperation);
                return true;
            }
            return false;
        }
    }
}
