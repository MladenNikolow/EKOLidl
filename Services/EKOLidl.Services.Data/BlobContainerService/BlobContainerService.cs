namespace EKOLidl.Services.Data.BlobContainerService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;
    using Microsoft.AspNetCore.Http;

    public class BlobContainerService : IBlobContainerService
    {
        private const int ImageMinimumBytes = 512;
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

                        // File.Delete(fullFilePath);
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

        public bool IsValidImage(IFormFile image)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (image.ContentType.ToLower() != "image/jpg" &&
                image.ContentType.ToLower() != "image/jpeg" &&
                image.ContentType.ToLower() != "image/pjpeg" &&
                image.ContentType.ToLower() != "image/gif" &&
                image.ContentType.ToLower() != "image/x-png" &&
                image.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(image.FileName).ToLower() != ".jpg"
                && Path.GetExtension(image.FileName).ToLower() != ".png"
                && Path.GetExtension(image.FileName).ToLower() != ".gif"
                && Path.GetExtension(image.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!image.OpenReadStream().CanRead)
                {
                    return false;
                }

                //------------------------------------------
                // Check whether the image size exceeding the limit or not
                //------------------------------------------ 
                if (image.Length < ImageMinimumBytes || image.Length > 10 * 1024 * 1024)
                {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                image.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using var bitmap = new System.Drawing.Bitmap(image.OpenReadStream());
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                image.OpenReadStream().Position = 0;
            }

            return true;
        }
    }
}
