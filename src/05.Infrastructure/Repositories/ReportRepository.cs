using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Infrastructure.Persistence;

namespace TicketManagement.Infrastructure.Repositories;

public class ReportRepository(ApplicationDbContext context) : IReportRepository
{
    public IQueryable<Ticket> GetFilterableQuery() =>
        context.Tickets
            .Include(t => t.AssignedAgent)
            .AsNoTracking(); // NFR-5: read-only report query
}