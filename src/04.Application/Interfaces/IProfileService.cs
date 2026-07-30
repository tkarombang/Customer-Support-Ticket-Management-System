using TicketManagement.Shared.Dtos.Profile;

namespace TicketManagement.Application.Interfaces;

public interface IProfileService
{
    Task<UserResponseDtoForProfile> GetAsync(Guid userId);
    Task UpdateAsync(Guid userId, UpdateProfileDto dto);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    Task<IEnumerable<ActivityLogDto>> GetActivityLogAsync(Guid userId);
}

// DTO kecil khusus tampilan profile diri sendiri
public class UserResponseDtoForProfile
{
    public required string Username { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public required string Role { get; set; }
    public DateTime CreatedDate { get; set; }
}