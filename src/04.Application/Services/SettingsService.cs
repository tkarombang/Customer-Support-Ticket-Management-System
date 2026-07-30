using System.Text.Json;
using TicketManagement.Application.Interfaces;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.Settings;

namespace TicketManagement.Application.Services;

public class SettingsService(IAppSettingRepository appSettingRepository) : ISettingsService
{
    private const string GeneralKey = "General.Config";
    private const string SlaKey = "Sla.Config";

    public async Task<GeneralSettingDto> GetGeneralAsync()
    {
        var setting = await appSettingRepository.GetByKeyAsync(GeneralKey);
        return setting?.SettingValue is null
            ? new GeneralSettingDto() // default kalau belum pernah disimpan
            : JsonSerializer.Deserialize<GeneralSettingDto>(setting.SettingValue)!;
    }

    public async Task UpdateGeneralAsync(GeneralSettingDto dto, Guid updatedBy)
    {
        await appSettingRepository.UpsertAsync(new AppSetting
        {
            SettingKey = GeneralKey,
            SettingValue = JsonSerializer.Serialize(dto),
            IsEncrypted = false,
            UpdatedBy = updatedBy
        });
    }

    public async Task<SlaSettingDto> GetSlaAsync()
    {
        var setting = await appSettingRepository.GetByKeyAsync(SlaKey);
        return setting?.SettingValue is null
            ? new SlaSettingDto()
            : JsonSerializer.Deserialize<SlaSettingDto>(setting.SettingValue)!;
    }

    public async Task UpdateSlaAsync(SlaSettingDto dto, Guid updatedBy)
    {
        await appSettingRepository.UpsertAsync(new AppSetting
        {
            SettingKey = SlaKey,
            SettingValue = JsonSerializer.Serialize(dto),
            IsEncrypted = false,
            UpdatedBy = updatedBy
        });
    }
}