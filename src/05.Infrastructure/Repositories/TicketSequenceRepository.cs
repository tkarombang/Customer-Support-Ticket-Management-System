using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Infrastructure.Persistence;

namespace TicketManagement.Infrastructure.Repositories;

public class TicketSequenceRepository(ApplicationDbContext context) : ITicketSequenceRepository
{
    public async Task<int> GetNextSequenceAsync(int year)
    {
        // Transaction + row lock untuk mencegah race condition
        // saat 2 tiket dibuat bersamaan (dapat nomor sama).
        await using var transaction = await context.Database.BeginTransactionAsync();

        var sequence = await context.TicketSequences
            .FromSqlInterpolated($"SELECT * FROM TicketSequences WITH (UPDLOCK, ROWLOCK) WHERE Id = {year}")
            .FirstOrDefaultAsync();

        if (sequence is null)
        {
            sequence = new TicketSequence { Year = year, LastSequence = 0 };
            context.TicketSequences.Add(sequence);
            await context.SaveChangesAsync();
        }

        sequence.LastSequence += 1;
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return sequence.LastSequence;
    }
}