using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Infrastructure.Persistence;

namespace TicketManagement.Infrastructure.Repositories
{
    public class TicketAttachmentRepository(ApplicationDbContext context)
    : ITicketAttachmentRepository
    {
        public IQueryable<TicketAttachment> GetFilterableQuery() =>
            context.TicketAttachments
                .Include(a => a.Ticket)
                .AsNoTracking();

        public async Task AddAsync(TicketAttachment attachment)
        {
            context.TicketAttachments.Add(attachment);
            await context.SaveChangesAsync();
        }
    }
}
