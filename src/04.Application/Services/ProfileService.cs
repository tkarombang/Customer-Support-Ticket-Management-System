using TicketManagement.Application.Interfaces;
using TicketManagement.Base.Exceptions;
using TicketManagement.Domain.Enums;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.Profile;

namespace TicketManagement.Application.Services;

public class ProfileService(
    IUserRepository userRepository,
    ISystemLogRepository systemLogRepository,
    ISystemLogService systemLogService)
    : IProfileService
{
    public async Task<UserResponseDtoForProfile> GetAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        return new UserResponseDtoForProfile
        {
            Username = user.Username,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            JobTitle = user.JobTitle,
            Address = user.Address,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role.ToString(),
            Status = user.Status.ToString(),
            CreatedDate = user.CreatedDate
        };
    }

    public async Task UpdateAsync(Guid userId, UpdateProfileDto dto)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        user.Name = dto.Name;
        user.PhoneNumber = dto.PhoneNumber;
        user.JobTitle = dto.JobTitle;
        user.Address = dto.Address;
        user.UpdatedDate = DateTime.UtcNow;

        await userRepository.UpdateAsync(user);

        await systemLogService.LogAsync(userId, SystemLogAction.UpdateProfile, "Memperbaharui Informasi Profil");

        //await systemLogRepository.AddAsync(new Domain.Entities.SystemLog
        //{
        //    UserId = userId,
        //    Action = Domain.Enums.SystemLogAction.UpdateProfile,
        //    Description = "Memperbarui informasi profil"
        //});
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            throw new ValidationException("OldPassword", "Password lama tidak sesuai.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedDate = DateTime.UtcNow;

        await userRepository.UpdateAsync(user);

        await systemLogService.LogAsync(userId, SystemLogAction.ChangePassword, "Mengubah password akun");

        //await systemLogRepository.AddAsync(new Domain.Entities.SystemLog
        //{
        //    UserId = userId,
        //    Action = Domain.Enums.SystemLogAction.ChangePassword,
        //    Description = "Mengubah password akun"
        //});
    }

    public async Task<IEnumerable<ActivityLogDto>> GetActivityLogAsync(Guid userId)
    {
        var logs = await systemLogRepository.GetByUserIdAsync(userId, take: 10);
        return logs.Select(l => new ActivityLogDto
        {
            Action = l.Action.ToString(),
            Description = l.Description,
            IpAddress = l.IpAddress,
            Timestamp = l.Timestamp
        });
    }
}