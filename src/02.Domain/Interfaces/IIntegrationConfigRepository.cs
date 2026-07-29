using TicketManagement.Domain.Entities;

namespace TicketManagement.Domain.Interfaces;

public interface IIntegrationConfigRepository
{
    Task<IEnumerable<IntegrationConfig>> GetAllAsync();
    Task<IntegrationConfig?> GetByIdAsync(Guid id);
    Task<IntegrationConfig> AddAsync(IntegrationConfig config);
    Task UpdateAsync(IntegrationConfig config);
}