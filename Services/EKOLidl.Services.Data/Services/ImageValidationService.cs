namespace EKOLidl.Services.Data.Services
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using EKOLidl.Services.Data.Services.Interfaces;
    using Microsoft.AspNetCore.Http;

    public class ImageValidationService : IFileValidationService
    {
        private const int ImageMinimumBytes = 512;

        /// <summary>
        /// Check if the input is valid image.
        /// </summary>
        /// <param name="image">Image from the input.</param>
        /// <returns></returns>
        public async Task<bool> CheckAsync(IFormFile image)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (image.ContentType.ToLower() != "image/jpg" &&
                image.ContentType.ToLower() != "image/jpeg" &&
                image.ContentType.ToLower() != "image/pjpeg" &&
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
                if (image.Length < ImageMinimumBytes || image.Length > 5 * 1024 * 1024)
                {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                await image.OpenReadStream().ReadAsync(buffer, 0, ImageMinimumBytes);
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
