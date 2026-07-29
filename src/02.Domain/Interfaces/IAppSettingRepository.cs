using TicketManagement.Domain.Entities;

namespace TicketManagement.Domain.Interfaces;

public interface IAppSettingRepository
{
    Task<AppSetting?> GetByKeyAsync(string key);
    Task<IEnumerable<AppSetting>> GetByKeyPrefixAsync(string prefix); // misal "Sla." untuk ambil semua SLA setting sekaligus
    Task UpsertAsync(AppSetting setting); // insert kalau belum ada, update kalau sudah ada
}