using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.Shared.Dtos.Settings;

namespace TicketManagement.Application.Interfaces
{
    public interface IBackupService
    {
        Task<BackupHistoryResponseDto> TriggerManualBackupAsync(Guid triggeredBy);
        Task RestoreAsync(Stream backupFileStream, string originalFileName);
        Task<IEnumerable<BackupHistoryResponseDto>> GetHistoryAsync();
    }
}
