using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IBlobStorage
    {
        public CloudBlobContainer GetContainerReference(string containerName);
        public Task<bool> UploadPdf(string pdfRefName, Stream pdf);
        public Task<string> GetPdf(string imageReferenceName);
        public Task<bool> UploadImage(string imageReferenceName, Stream image);
        public Task<string> GetImage(string imageReferenceName);

    }
}
