namespace EKOLidl.Services.Data.Services
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using EKOLidl.Services.Data.Services.Interfaces;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Accepts JPEG, PNG and WebP photos between 512 B and 5 MB, checked by extension and by file signature.
    /// </summary>
    public class ImageValidationService : IFileValidationService
    {
        public const long MaxImageBytes = 5 * 1024 * 1024;

        private const int ImageMinimumBytes = 512;

        private static readonly byte[] JpegSignature = { 0xFF, 0xD8, 0xFF };

        private static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        /// <summary>
        /// Check if the input is a valid image.
        /// </summary>
        /// <param name="image">Image from the input.</param>
        /// <returns><c>true</c> when the file is an allowed image.</returns>
        public async Task<bool> CheckAsync(IFormFile image)
        {
            if (image == null || image.Length < ImageMinimumBytes || image.Length > MaxImageBytes)
            {
                return false;
            }

            string extension = Path.GetExtension(image.FileName)?.ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".webp")
            {
                return false;
            }

            byte[] header = new byte[12];
            await using (Stream stream = image.OpenReadStream())
            {
                int read = await stream.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false);
                if (read < header.Length)
                {
                    return false;
                }
            }

            return extension switch
            {
                ".jpg" or ".jpeg" => StartsWith(header, JpegSignature),
                ".png" => StartsWith(header, PngSignature),
                _ => IsWebP(header),
            };
        }

        private static bool StartsWith(byte[] header, byte[] signature) =>
            header.AsSpan().StartsWith(signature);

        // RIFF....WEBP
        private static bool IsWebP(byte[] header) =>
            header[0] == 'R' && header[1] == 'I' && header[2] == 'F' && header[3] == 'F'
            && header[8] == 'W' && header[9] == 'E' && header[10] == 'B' && header[11] == 'P';
    }
}
