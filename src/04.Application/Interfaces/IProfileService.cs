using TicketManagement.Shared.Dtos.Profile;

namespace TicketManagement.Application.Interfaces;

public interface IProfileService
{
    Task<UserResponseDtoForProfile> GetAsync(Guid userId);
    Task UpdateAsync(Guid userId, UpdateProfileDto dto);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    Task<IEnumerable<ActivityLogDto>> GetActivityLogAsync(Guid userId);
}

