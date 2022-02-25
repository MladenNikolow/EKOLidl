namespace EKOLidl.Services.Data.BlobContainerService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public interface IBlobContainerService
    {
        Task<IEnumerable<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string filesPath);
    }
}
