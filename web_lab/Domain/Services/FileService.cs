using Microsoft.AspNetCore.Http;

namespace Domain.Services
{
    public class FileService : IFileService
    {
        public readonly string _uploadProfilePictureDirectoryPath;
        public readonly string _uploadCatalogItemImageDirectoryPath;
        public FileService(string uploadDirectoryPath, string uploadCatalogItemImageDirectoryPath)
        {
            _uploadProfilePictureDirectoryPath = uploadDirectoryPath;
            _uploadCatalogItemImageDirectoryPath = uploadCatalogItemImageDirectoryPath;
        }

        public string GetProfilePictureStoragePath()
        {
            return _uploadProfilePictureDirectoryPath;
        }

        public string GetCatalogItemImageStoragePath()
        {
            return _uploadCatalogItemImageDirectoryPath;
        }
        public async Task<string?> SaveCatalogItemImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty");
            }

            if (!Directory.Exists(_uploadCatalogItemImageDirectoryPath))
            {
                Directory.CreateDirectory(_uploadCatalogItemImageDirectoryPath);
            }

            var fileName = $"image_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadCatalogItemImageDirectoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public async Task<string?> SaveProfilePictureAsync(IFormFile file, string userName)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty");
            }

            if (!Directory.Exists(_uploadProfilePictureDirectoryPath))
            {
                Directory.CreateDirectory(_uploadProfilePictureDirectoryPath);
            }

            var fileName = $"{userName}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadProfilePictureDirectoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
