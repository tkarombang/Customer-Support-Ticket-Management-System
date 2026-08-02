using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TicketManagement.Application.Interfaces;
using TicketManagement.Base.Exceptions;

namespace TicketManagement.Application.Services
{
    public class FileStorageService(IWebHostEnvironment env) : IFileStorageService
    {
        private static readonly string[] AllowedExtensions =
            [".png", ".jpg", ".jpeg", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".zip"];
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB (REQ-2.12)

        public async Task<(string FilePath, long FileSizeBytes)> SaveAsync(IFormFile file, string subFolder)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new ValidationException("File", $"Tipe file '{extension}' tidak diizinkan.");

            if (file.Length > MaxFileSizeBytes)
                throw new ValidationException("File", "Ukuran file maksimal 10MB.");

            var webRoot = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var folder = Path.Combine(webRoot, "uploads", subFolder);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Nama file di-generate GUID (bukan nama asli) — cegah path traversal/overwrite (NFR, Security)
            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(folder, storedFileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/{subFolder}/{storedFileName}";
            return (relativePath, file.Length);
        }
    }

}
