using System.Text.Json;
using TicketManagement.Application.Interfaces;
using TicketManagement.Base.Exceptions;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.Settings;

namespace TicketManagement.Application.Services
{
    public class SettingsService(
        IAppSettingRepository appSettingRepository,
        IIntegrationConfigRepository integrationRepository,
        ICredentialEncryptor credentialEncryptor) 
        : ISettingsService
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


        public async Task<IEnumerable<IntegrationResponseDto>> GetIntegrationsAsync()
        {
            var integrations = await integrationRepository.GetAllAsync();
            return integrations.Select(MapToDto);
        }

        public async Task<IntegrationResponseDto> CreateIntegrationAsync(CreateIntegrationDto dto)
        {
            var config = new IntegrationConfig
            {
                Name = dto.Name,
                WebhookUrl = dto.WebhookUrl,
                ApiKeyEncrypted = string.IsNullOrEmpty(dto.ApiKey)
                    ? null
                    : credentialEncryptor.Encrypt(dto.ApiKey), // NFR-8: wajib dienkripsi
                IsActive = false
            };

            var created = await integrationRepository.AddAsync(config);
            return MapToDto(created);
        }

        public async Task<IntegrationResponseDto> UpdateIntegrationAsync(Guid id, UpdateIntegrationDto dto)
        {
            var config = await integrationRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("IntegrationConfig", id);

            config.Name = dto.Name;
            config.WebhookUrl = dto.WebhookUrl;
            config.IsActive = dto.IsActive;

            // Hanya update ApiKey kalau user mengisi field baru (tidak kosong)
            if (!string.IsNullOrEmpty(dto.ApiKey))
                config.ApiKeyEncrypted = credentialEncryptor.Encrypt(dto.ApiKey);

            await integrationRepository.UpdateAsync(config);
            return MapToDto(config);
        }

        private static IntegrationResponseDto MapToDto(IntegrationConfig config) => new()
        {
            IntegrationId = config.Id,
            Name = config.Name,
            WebhookUrl = config.WebhookUrl,
            HasApiKey = !string.IsNullOrEmpty(config.ApiKeyEncrypted),
            IsActive = config.IsActive,
            CreatedDate = config.CreatedDate
        };
    }

}
