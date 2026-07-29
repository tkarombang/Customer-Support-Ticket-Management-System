using TicketManagement.Domain.Entities;

namespace TicketManagement.Domain.Interfaces;

public interface ISystemLogRepository
{
    Task AddAsync(SystemLog log);

    /// <summary>
    /// Untuk halaman System Logs (Settings) dengan filter.
    /// </summary>
    IQueryable<SystemLog> GetFilterableQuery();

    /// <summary>
    /// Untuk Profile Activity Log — hanya log milik user tertentu.
    /// </summary>
    Task<IEnumerable<SystemLog>> GetByUserIdAsync(Guid userId, int take = 10);
}