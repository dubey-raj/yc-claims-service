using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ClaimService.Services.Documents
{
    public class LocalFileUploadService : IFileUploader
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<List<string>> UploadFilesAsync(string folderName, List<IFormFile> files)
        {
            var uploadedFilePaths = new List<string>();
            
            foreach (var file in files) 
            {
                var uploadsFolder = Path.Combine(_env.ContentRootPath, "claim-docs", folderName);
                Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, file.FileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                uploadedFilePaths.Add(filePath);
            }

            return uploadedFilePaths;
        }
    }

}
