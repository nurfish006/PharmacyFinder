using Microsoft.AspNetCore.Http;

namespace PharmacyFinder.Core.Interfaces
{
    // Handles file upload and storage operations
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        Task<bool> DeleteFileAsync(string filePath);
        string GetFileUrl(string filePath);
    }
}