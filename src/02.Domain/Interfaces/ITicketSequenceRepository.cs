namespace TicketManagement.Domain.Interfaces;

public interface ITicketSequenceRepository
{
    /// <summary>
    /// Increment counter secara atomic dan return nilai baru.
    /// Implementasi harus pakai row-lock/transaction untuk hindari race condition
    /// saat 2 tiket dibuat bersamaan.
    /// </summary>
    Task<int> GetNextSequenceAsync(int year);
}