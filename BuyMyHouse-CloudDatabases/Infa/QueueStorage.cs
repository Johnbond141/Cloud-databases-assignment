using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infa
{
    public class QueueStorage
    {
        public CloudQueueClient _queueClient { get; set; }
        private string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        public QueueStorage()
        {

        }

        public async Task CreateQueueMessage(string queueName, string message)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
                _queueClient = cloudStorageAccount.CreateCloudQueueClient();
                CloudQueue queue = _queueClient.GetQueueReference(queueName);
                await queue.CreateIfNotExistsAsync();

                CloudQueueMessage item = new CloudQueueMessage(message);
                await queue.AddMessageAsync(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n\n");
                Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
            }
        }


    }
}
