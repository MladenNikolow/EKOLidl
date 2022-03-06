namespace EKOLidl.Services.Data.Services.Interfaces
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IStorageService
    {
        /// <summary>
        ///     Upload a file and returns the full path to retrieve the file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<string> UploadFileAsync(IFormFile file);

        /// <summary>
        ///     Upload list of files and returns the full path to retrieve the files.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> UploadFilesAsync(IEnumerable<IFormFile> files);
    }
}
