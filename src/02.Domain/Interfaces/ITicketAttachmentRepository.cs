using TicketManagement.Domain.Entities;

namespace TicketManagement.Domain.Interfaces
{
    public interface ITicketAttachmentRepository
    {
        IQueryable<TicketAttachment> GetFilterableQuery();
        Task AddAsync(TicketAttachment attachment);
    }
}
