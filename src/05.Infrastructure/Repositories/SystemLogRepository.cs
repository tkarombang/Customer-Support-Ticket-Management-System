using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Infrastructure.Persistence;

namespace TicketManagement.Infrastructure.Repositories;

public class SystemLogRepository(ApplicationDbContext context) : ISystemLogRepository
{
    public async Task AddAsync(SystemLog log)
    {
        context.SystemLogs.Add(log);
        await context.SaveChangesAsync();
    }

    public IQueryable<SystemLog> GetFilterableQuery() =>
        context.SystemLogs.Include(l => l.User).AsNoTracking();

    public async Task<IEnumerable<SystemLog>> GetByUserIdAsync(Guid userId, int take = 10) =>
        await context.SystemLogs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Timestamp)
            .Take(take)
            .AsNoTracking()
            .ToListAsync();
}