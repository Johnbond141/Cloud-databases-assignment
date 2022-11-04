using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infa
{
    public class BlobStorage : IBlobStorage
    {
        private string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private CloudStorageAccount cloudStorageAccount { get; set; }

        public BlobStorage()
        {
            cloudStorageAccount = GetCloudStorageAccount();
        }

        private CloudStorageAccount GetCloudStorageAccount()
        {
            if (cloudStorageAccount == null)
            {
                cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            }
            return cloudStorageAccount;
        }

        public CloudBlobContainer GetContainerReference(string containerName)
        {
            cloudStorageAccount = GetCloudStorageAccount();
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }
        public async Task<bool> UploadPdf(string pdfRefName, Stream pdf)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                await blobServiceClient.GetBlobContainerClient("pdf").CreateIfNotExistsAsync();
                var cloudBlobContainer = GetContainerReference("pdf");
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(pdfRefName);
                cloudBlockBlob.Properties.ContentType = "text/plain";
                await cloudBlockBlob.UploadFromStreamAsync(pdf);
                return true;
            }
            catch (StorageException ex)
            {
                throw new StorageException(ex.Message);
            }
        }

        public async Task<string> GetPdf(string pdfRefName)
        {
            try
            {
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("pdf");
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(pdfRefName);
                return cloudBlockBlob.Uri.ToString();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task<bool> UploadImage(string imageReferenceName, Stream image)
        {
            try
            {
                var cloudBlobContainer = GetContainerReference("images");
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageReferenceName);
                cloudBlockBlob.Properties.ContentType = "image/png";
                await cloudBlockBlob.UploadFromStreamAsync(image);
                return true;
            }
            catch (StorageException ex)
            {
                throw new StorageException(ex.Message);
            }
        }

        public async Task<string> GetImage(string imageReferenceName)
        {
            try
            {
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("images");
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageReferenceName);
                return cloudBlockBlob.Uri.ToString();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
        public async Task ClearMortgageApplications() {
            try
            {
                var cloudBlobContainer = GetContainerReference("pdf");
                await cloudBlobContainer.DeleteAsync();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}
