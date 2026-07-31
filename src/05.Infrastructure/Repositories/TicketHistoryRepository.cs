using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Infrastructure.Persistence;

namespace TicketManagement.Infrastructure.Repositories;

public class TicketHistoryRepository(ApplicationDbContext context) : ITicketHistoryRepository
{
    public IQueryable<TicketHistory> GetFilterableQuery() =>
        context.TicketHistories
            .Include(h => h.Ticket)
            .Include(h => h.ChangedByUser)
            .AsNoTracking();

    public async Task AddAsync(TicketHistory history)
    {
        context.TicketHistories.Add(history);
        await context.SaveChangesAsync();
    }
}