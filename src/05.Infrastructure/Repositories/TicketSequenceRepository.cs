using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Infrastructure.Persistence;

namespace TicketManagement.Infrastructure.Repositories;

public class TicketSequenceRepository(ApplicationDbContext context) : ITicketSequenceRepository
{
    public async Task<int> GetNextSequenceAsync()
    {
        // Transaction + row lock untuk mencegah race condition
        // saat 2 tiket dibuat bersamaan (dapat nomor sama).
        await using var transaction = await context.Database.BeginTransactionAsync();

        var sequence = await context.TicketSequences
            .FromSqlRaw("SELECT * FROM TicketSequences WITH (UPDLOCK, ROWLOCK) WHERE Id = 1")
            .FirstAsync();

        sequence.LastSequence += 1;
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return sequence.LastSequence;
    }
}