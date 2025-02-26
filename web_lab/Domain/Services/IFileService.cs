using Microsoft.AspNetCore.Http;

namespace Domain.Services
{
    public interface IFileService
    {
        Task<string?> SaveProfilePictureAsync(IFormFile file, string userName);
        Task<string?> SaveCatalogItemImageAsync(IFormFile file);
        string GetProfilePictureStoragePath();
    }
}
