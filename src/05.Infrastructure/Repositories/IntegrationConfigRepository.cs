using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Infrastructure.Persistence;

namespace TicketManagement.Infrastructure.Repositories;

public class IntegrationConfigRepository(ApplicationDbContext context) : IIntegrationConfigRepository
{
    public async Task<IEnumerable<IntegrationConfig>> GetAllAsync() =>
        await context.IntegrationConfigs.AsNoTracking().ToListAsync();

    public async Task<IntegrationConfig?> GetByIdAsync(Guid id) =>
        await context.IntegrationConfigs.FindAsync(id);

    public async Task<IntegrationConfig> AddAsync(IntegrationConfig config)
    {
        context.IntegrationConfigs.Add(config);
        await context.SaveChangesAsync();
        return config;
    }

    public async Task UpdateAsync(IntegrationConfig config)
    {
        context.IntegrationConfigs.Update(config);
        await context.SaveChangesAsync();
    }
}