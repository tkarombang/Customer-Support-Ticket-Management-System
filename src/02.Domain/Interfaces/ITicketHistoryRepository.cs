using TicketManagement.Domain.Entities;

namespace TicketManagement.Domain.Interfaces;

public interface ITicketHistoryRepository
{
    /// <summary>Untuk halaman Ticket Histories global (REQ-8.1) — filter dinamis.</summary>
    IQueryable<TicketHistory> GetFilterableQuery();

    Task AddAsync(TicketHistory history);
}