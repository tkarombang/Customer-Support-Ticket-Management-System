using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Infrastructure.Persistence;

namespace TicketManagement.Infrastructure.Repositories;

public class AppSettingRepository(ApplicationDbContext context) : IAppSettingRepository
{
    public async Task<AppSetting?> GetByKeyAsync(string key) =>
        await context.AppSettings.FindAsync(key);

    public async Task<IEnumerable<AppSetting>> GetByKeyPrefixAsync(string prefix) =>
        await context.AppSettings
            .Where(s => s.SettingKey.StartsWith(prefix))
            .AsNoTracking()
            .ToListAsync();

    public async Task UpsertAsync(AppSetting setting)
    {
        var existing = await context.AppSettings.FindAsync(setting.SettingKey);
        if (existing is null)
        {
            context.AppSettings.Add(setting);
        }
        else
        {
            existing.SettingValue = setting.SettingValue;
            existing.IsEncrypted = setting.IsEncrypted;
            existing.UpdatedDate = DateTime.UtcNow;
            existing.UpdatedBy = setting.UpdatedBy;
        }
        await context.SaveChangesAsync();
    }
}