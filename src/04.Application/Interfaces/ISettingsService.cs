using TicketManagement.Shared.Dtos.Settings;

namespace TicketManagement.Application.Interfaces
{
    public interface ISettingsService
    {
        Task<GeneralSettingDto> GetGeneralAsync();
        Task UpdateGeneralAsync(GeneralSettingDto dto, Guid updatedBy);

        Task<SlaSettingDto> GetSlaAsync();
        Task UpdateSlaAsync(SlaSettingDto dto, Guid updatedBy);

        Task<IEnumerable<IntegrationResponseDto>> GetIntegrationsAsync();
        Task<IntegrationResponseDto> CreateIntegrationAsync(CreateIntegrationDto dto);
        Task<IntegrationResponseDto> UpdateIntegrationAsync(Guid id, UpdateIntegrationDto dto);
    }
}
