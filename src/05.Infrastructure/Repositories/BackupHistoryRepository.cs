using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Infrastructure.Persistence;

namespace TicketManagement.Infrastructure.Repositories
{
    public class BackupHistoryRepository(ApplicationDbContext context) : IBackupHistoryRepository
    {
        public async Task AddAsync(BackupHistory history)
        {
            context.BackupHistories.Add(history);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BackupHistory>> GetAllAsync() =>
            await context.BackupHistories
                .Include(h => h.TriggeredByUser)
                .OrderByDescending(h => h.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
    }
}
