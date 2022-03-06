namespace EKOLidl.Services.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using EKOLidl.Services.Data.Services.Interfaces;
    using Microsoft.AspNetCore.Http;

    public class AzureBlobStorageService : IStorageService
    {
        private readonly BlobContainerClient blobContainerClient;

        public AzureBlobStorageService(BlobContainerClient blobContainerClient)
        {
            this.blobContainerClient = blobContainerClient ?? throw new ArgumentNullException(nameof(blobContainerClient));
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            string result = string.Empty;

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            using (Stream stream = file.OpenReadStream())
            {
                await this.blobContainerClient.UploadBlobAsync(blobName, stream);
                result = this.blobContainerClient.Uri.AbsoluteUri;
            }

            return result;
        }

        public async Task<IEnumerable<string>> UploadFilesAsync(IEnumerable<IFormFile> files)
        {
            List<string> filesUrls = new List<string>();

            foreach (var file in files)
            {
                var uploadedFileUri = await this.UploadFileAsync(file);
                filesUrls.Add(uploadedFileUri.ToString());
            }

            return filesUrls;
        }
    }
}
