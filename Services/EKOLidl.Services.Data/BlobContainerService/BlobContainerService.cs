namespace EKOLidl.Services.Data.BlobContainerService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Microsoft.AspNetCore.Http;

    public class BlobContainerService : IBlobContainerService
    {
        private readonly BlobContainerClient blobContainerClient;

        public BlobContainerService(string connectionString, string containerName)
        {
            this.blobContainerClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<IEnumerable<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string filesPath)
        {
            List<string> filesUrls = new List<string>();

            try
            {
                foreach (var file in files)
                {
                    string fullFilePath = filesPath + "/" + file.FileName;

                    using (FileStream fileStream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    BlobClient blobClient = this.blobContainerClient.GetBlobClient(file.FileName);

                    using (var fileStream = System.IO.File.OpenRead(fullFilePath))
                    {
                        await blobClient.UploadAsync(fileStream);
                    }

                    filesUrls.Add(blobClient.Uri.AbsoluteUri);
                }
            }
            catch
            {
                return null;
            }

            return filesUrls;
        }
    }
}
