using Microsoft.AspNetCore.Http;

namespace TicketManagement.Application.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>Simpan file ke wwwroot/uploads, return path relatif untuk disimpan di DB.</summary>
        Task<(string FilePath, long FileSizeBytes)> SaveAsync(IFormFile file, string subFolder);
    }

}