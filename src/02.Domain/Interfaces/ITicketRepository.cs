using TicketManagement.Domain.Entities;

namespace TicketManagement.Domain.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id);
    Task<IEnumerable<Ticket>> GetAllAsync();
    Task<Ticket> AddAsync(Ticket ticket);
    Task UpdateAsync(Ticket ticket);
    Task<bool> TicketNumberExistsAsync(string ticketNumber);
    Task<int> GetNextTicketSequenceAsync(); // pakai TicketSequence, bukan urutan Id
}