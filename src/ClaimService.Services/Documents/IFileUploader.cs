using Microsoft.AspNetCore.Http;

namespace ClaimService.Services.Documents
{
    public interface IFileUploader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderName">Name of folder in s3 bucket where file will be uploaded</param>
        /// <param name="files">List of file stream</param>
        /// <returns></returns>
        Task<List<string>> UploadFilesAsync(string folderName, List<IFormFile> files);
    }
}
