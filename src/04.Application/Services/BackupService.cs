using DocumentFormat.OpenXml.Spreadsheet;
using TicketManagement.Application.Interfaces;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Domain.Enums;
using TicketManagement.Shared.Dtos.Settings;

namespace TicketManagement.Application.Services
{
    public class BackupService(
    IBackupRepository backupRepository,
    IBackupHistoryRepository backupHistoryRepository,
    ISystemLogService systemLogService)
    : IBackupService
    {
        public async Task<BackupHistoryResponseDto> TriggerManualBackupAsync(Guid triggeredBy)
        {
            var fileName = $"backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak";
            BackupHistory history;

            try
            {
                var filePath = await backupRepository.BackupDatabaseAsync(fileName);
                var fileInfo = new FileInfo(filePath);

                history = new BackupHistory
                {
                    FileName = fileName,
                    FilePath = filePath,
                    FileSizeBytes = fileInfo.Exists ? fileInfo.Length : null,
                    Type = "Manual",
                    Status = "Success",
                    TriggeredBy = triggeredBy
                };
            }
            catch (Exception ex)
            {
                history = new BackupHistory
                {
                    FileName = fileName,
                    FilePath = "-",
                    Type = "Manual",
                    Status = "Failed",
                    TriggeredBy = triggeredBy
                };

                await backupHistoryRepository.AddAsync(history);
                throw new Base.Exceptions.ValidationException("Backup", $"Backup gagal: {ex.Message}");
            }

            await backupHistoryRepository.AddAsync(history);

            await systemLogService.LogAsync(triggeredBy, SystemLogAction.BackupDatabase,
                history.Status == "Success" ? $"Backup Berhasil: {fileName}" : "Backup Gagal");

            return MapToDto(history);
        }

        public async Task RestoreAsync(Stream backupFileStream, string originalFileName, Guid restoredBy)
        {
            var backupFolder = GetBackupFolder();

            // Simpan file upload ke folder sementara dulu sebelum di-restore
            var tempPath = Path.Combine(backupFolder, $"restore_{Guid.NewGuid()}.bak");

            await using (var fileStream = File.Create(tempPath))
            {
                await backupFileStream.CopyToAsync(fileStream);
            }

            try
            {
                await backupRepository.RestoreDatabaseAsync(tempPath);

                await systemLogService.LogAsync(restoredBy, SystemLogAction.RestoreDatabase, $"Restore database dari file {originalFileName}");
            }
            finally
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }

        public async Task<IEnumerable<BackupHistoryResponseDto>> GetHistoryAsync()
        {
            var histories = await backupHistoryRepository.GetAllAsync();
            return histories.Select(MapToDto);
        }

        private static BackupHistoryResponseDto MapToDto(BackupHistory h) => new()
        {
            BackupId = h.Id,
            FileName = h.FileName,
            FileSizeBytes = h.FileSizeBytes,
            Type = h.Type,
            Status = h.Status,
            CreatedDate = h.CreatedDate,
            TriggeredByName = h.TriggeredByUser?.Name
        };


        // HELPERS PATH Backup
        private static string GetBackupFolder()
        {
            var folder = Path.Combine(AppContext.BaseDirectory, "App_Data", "backups");
            Directory.CreateDirectory(folder);
            return folder;
        }
    }
}