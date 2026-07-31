using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Domain.Interfaces
{
    public interface IBackupHistoryRepository
    {
        Task AddAsync(BackupHistory history);
        Task<IEnumerable<BackupHistory>> GetAllAsync();
    }
}
