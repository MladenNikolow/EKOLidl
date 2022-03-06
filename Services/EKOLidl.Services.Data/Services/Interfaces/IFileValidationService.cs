namespace EKOLidl.Services.Data.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IFileValidationService
    {
        /// <summary>
        /// Validate file.
        /// </summary>
        /// <param name="file">File from the input.</param>
        /// <returns></returns>
        public Task<bool> CheckAsync(IFormFile file);
    }
}
