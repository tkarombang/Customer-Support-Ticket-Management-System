using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Infrastructure.Persistence;

namespace TicketManagement.Infrastructure.Repositories;

public class TicketRepository(ApplicationDbContext context) : ITicketRepository
{
    public async Task<Ticket?> GetByIdAsync(Guid id) =>
        await context.Tickets
            .Include(t => t.AssignedAgent)
            .Include(t => t.Histories)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Ticket>> GetAllAsync() =>
        await context.Tickets
            .Include(t => t.AssignedAgent)
            .AsNoTracking() // read-only list, tidak perlu tracking (NFR-5)
            .ToListAsync();

    public async Task<Ticket> AddAsync(Ticket ticket)
    {
        context.Tickets.Add(ticket);
        await context.SaveChangesAsync();
        return ticket;
    }

    public async Task UpdateAsync(Ticket ticket)
    {
        context.Tickets.Update(ticket);
        await context.SaveChangesAsync();
    }

    public async Task<bool> TicketNumberExistsAsync(string ticketNumber) =>
        await context.Tickets.AnyAsync(t => t.TicketNumber == ticketNumber);

    //public async Task<int> GetNextTicketSequenceAsync()
    //{
    //    var year = DateTime.UtcNow.Year;
    //    var lastTicket = await context.Tickets
    //        .Where(y => y.TicketNumber.StartsWith($"TKT-{year}"))
    //        .OrderByDescending(t => t.Id)
    //        .Select(t => t.TicketNumber)
    //        .FirstOrDefaultAsync();

    //    if (lastTicket is null) return 0;

    //    // "TKT-00005" -> ambil angka setelah "TKT-"
    //    var numberPart = lastTicket.Split('-').Last();
    //    return int.TryParse(numberPart, out var sequence) ? sequence + 1 : 1;
    //}
}