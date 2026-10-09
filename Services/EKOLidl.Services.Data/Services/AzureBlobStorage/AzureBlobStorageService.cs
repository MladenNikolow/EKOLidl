namespace EKOLidl.Services.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
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
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            BlobClient blobClient = this.blobContainerClient.GetBlobClient(Guid.NewGuid() + extension);

            await using (Stream stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = ContentTypeFor(extension) },
                });
            }

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<IEnumerable<string>> UploadFilesAsync(IEnumerable<IFormFile> files)
        {
            List<string> filesUrls = new List<string>();

            foreach (var file in files ?? Array.Empty<IFormFile>())
            {
                filesUrls.Add(await this.UploadFileAsync(file));
            }

            return filesUrls;
        }

        public async Task DeleteFilesAsync(IEnumerable<string> fileUrls)
        {
            foreach (string url in fileUrls ?? Array.Empty<string>())
            {
                // Only delete blobs that live in this container.
                if (!url.StartsWith(this.blobContainerClient.Uri.AbsoluteUri + "/", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string blobName = Uri.UnescapeDataString(url.Substring(this.blobContainerClient.Uri.AbsoluteUri.Length + 1));
                await this.blobContainerClient.DeleteBlobIfExistsAsync(blobName);
            }
        }

        private static string ContentTypeFor(string extension) => extension switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "image/jpeg",
        };
    }
}
